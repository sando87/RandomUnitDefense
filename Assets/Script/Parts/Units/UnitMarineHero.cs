using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineHero : UnitBase
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField][Range(0, 1)] float _SkillCastRate = 0.2f;
    float SkillCastRate { get { return _SkillCastRate * mBaseObj.BuffProp.Percentage; } }
    [SerializeField] float _SlowDuration = 3.0f;
    float SlowDuration { get { return _SlowDuration * mBaseObj.BuffProp.SkillDuration; } }
    [SerializeField][Range(0, 100)] int _MoveSlowness = 20;

    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;

    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
    }

    // public override string SkillDescription
    // {
    //     get
    //     {
    //         return "기본 공격시 20%확률로 미사일 발사\n(피격된 유닛 20% 이속감소)";
    //     }
    // }

    private void OnAttack(int idx)
    {
        BaseObject target = mMotionAttack.Target;
        // 20% 확률로 스킬샷 발동 부분
        float rate = Mathf.Clamp(SkillCastRate, 0, 1);
        bool isSkillShoot = UnityEngine.Random.Range(0, 1.0f) < rate;
        if (isSkillShoot && idx == 0)
            ShootMagicGun(target);
        else
            ShootSimpleGun(target);
    }

    private void ShootSimpleGun(BaseObject target)
    {
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
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
            col.GetBaseObject().Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            ApplySlowDeBuff(col.GetBaseObject());
        }
    }
    private void ApplySlowDeBuff(BaseObject target)
    {
        DeBuffSlow buff = target.BuffCtrl.FindBuff<DeBuffSlow>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 결국 마린 여러마리가 공격해도 slow효과는 중복되지 않는 개념...
        else
            target.BuffCtrl.AddBuff(new DeBuffSlow(SlowDuration, _MoveSlowness));
    }

    class DeBuffSlow : BuffBase
    {
        //적 이동속도 20% 감소 디버프(duration시간만큼 지속)
        private int slowness = 0;
        public DeBuffSlow(float duration, int _slowness) { Duration = duration; slowness = _slowness; }
        public override void StartBuff(BaseObject target)
        {
            target.Renderer.SetColor(Color.blue);
            target.BuffProp.MoveSpeed -= slowness;
        }
        public override void EndBuff(BaseObject target)
        {
            target.Renderer.SetColor(Color.white);
            target.BuffProp.MoveSpeed += slowness;
        }
    }

}
