using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarine : UnitUser, IUserInputReciever
{
    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;

    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
        GetComponent<MotionSingleAttack>().EventFired = OnAttack;
    }
    public override void Release()
    {
        base.Release();
    }

    public override string SkillDescription
    {
        get
        {
            return "기본 공격시 20%확률로 미사일 발사(피격된 유닛 20% 이속감소)";
        }
    }

    private void OnAttack(UnitBase target)
    {
        // 20% 확률로 스킬샷 발동 부분
        bool isSkillShoot = UnityEngine.Random.Range(0, 5) == 0;
        if (isSkillShoot)
            ShootMagicGun(target);
        else
            ShootSimpleGun(target);
    }

    private void ShootSimpleGun(UnitBase target)
    {
        UnitMob enemy = target as UnitMob;
        enemy.GetDamaged(Property);
        Vector3 pos = Utils.Random(enemy.Center, 0.1f);
        GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }
    private void ShootMagicGun(UnitBase target)
    {
        UnitMob enemy = target as UnitMob;
        MagicGun missile = Instantiate(MagicGunMissile, Center, Quaternion.identity);
        missile.Destination = enemy.transform.position;
        missile.EventHit = OnHitMissile;
        missile.Launch();
    }
    private void OnHitMissile(UnitBase[] targets)
    {
        foreach(UnitBase target in targets)
        {
            UnitMob mob = target as UnitMob;
            mob.GetDamaged(Property);
            ApplySlowDeBuff(mob);
        }
    }
    private void ApplySlowDeBuff(UnitBase target)
    {
        DeBuffSlow buff = target.BuffCtrl.FindBuff<DeBuffSlow>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프있을 경우 갱신만. => 결국 마린 여러마리가 공격해도 slow효과는 중복되지 않는 개념...
        else
            target.BuffCtrl.AddBuff(new DeBuffSlow(Property.SkillDuration));
    }

    public void OnClick()
    {
        RGame.Get<RGameSystemManager>().HUDObject.Show(this);
    }

    public void OnDragAndDrop(Vector3 dropWorldPos)
    {
    }

    public void OnDragging(Vector3 draggingWorldPos)
    {
    }

    class DeBuffSlow : BuffBase
    {
        //적 이동속도 20% 감소 디버프(duration시간만큼 지속)
        public DeBuffSlow(float duration) { Duration = duration; }
        public override void StartBuff(UnitBase target)
        {
            target.SR.color = Color.blue;
            target.BuffValues.MoveSpeed -= 20;
        }
        public override void EndBuff(UnitBase target)
        {
            target.SR.color = Color.white;
            target.BuffValues.MoveSpeed += 20;
        }
    }

}
