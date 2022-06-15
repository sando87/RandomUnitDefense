using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineHero : UnitUser
{
    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mBaseObj.MotionManager.FindMotion<MotionSingleAttack>().EventFired = OnAttack;
    }

    // public override string SkillDescription
    // {
    //     get
    //     {
    //         return "기본 공격시 20%확률로 미사일 발사\n(피격된 유닛 20% 이속감소)";
    //     }
    // }

    private void OnAttack(Collider[] targets)
    {
        BaseObject target = targets[0].GetBaseObject();
        // 20% 확률로 스킬샷 발동 부분
        bool isSkillShoot = UnityEngine.Random.Range(0, 5) == 0;
        if (isSkillShoot)
            ShootMagicGun(target);
        else
            ShootSimpleGun(target);
    }

    private void ShootSimpleGun(BaseObject target)
    {
        target.Health.GetDamaged(mBaseObj);
        Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
        GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }
    private void ShootMagicGun(BaseObject target)
    {
        MagicGun missile = Instantiate(MagicGunMissile, mBaseObj.Body.Center, Quaternion.identity);
        missile.Destination = target.Body.Center;
        missile.EventHit = OnHitMissile;
        missile.Launch();
    }
    private void OnHitMissile(Vector3 dest)
    {
        Collider[] cols = InGameUtils.DetectAround(dest, 0.1f, 1 << LayerID.Enemies);
        foreach(Collider col in cols)
        {
            col.GetBaseObject().Health.GetDamaged(mBaseObj);
            ApplySlowDeBuff(col.GetBaseObject());
        }
    }
    private void ApplySlowDeBuff(BaseObject target)
    {
        DeBuffSlow buff = target.BuffCtrl.FindBuff<DeBuffSlow>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 결국 마린 여러마리가 공격해도 slow효과는 중복되지 않는 개념...
        else
            target.BuffCtrl.AddBuff(new DeBuffSlow(mBaseObj.SpecProp.SkillDuration));
    }

    class DeBuffSlow : BuffBase
    {
        //적 이동속도 20% 감소 디버프(duration시간만큼 지속)
        public DeBuffSlow(float duration) { Duration = duration; }
        public override void StartBuff(BaseObject target)
        {
            target.Renderer.SetColor(Color.blue);
            target.BuffProp.MoveSpeed -= 20;
        }
        public override void EndBuff(BaseObject target)
        {
            target.Renderer.SetColor(Color.white);
            target.BuffProp.MoveSpeed += 20;
        }
    }

}
