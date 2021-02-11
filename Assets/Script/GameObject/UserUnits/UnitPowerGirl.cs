using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPowerGirl : UnitUser
{
    [SerializeField] private SimpleMissile SimpleMissile = null;

    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
        GetComponent<MotionMultiAttack>().EventFired = OnAttack;
        GetComponent<MotionRandomShooting>().EventFired = OnSkill;
    }
    public override void Release()
    {
        base.Release();
    }

    public override string SkillDescription
    {
        get
        {
            return "일정 시간마다 총기 난사";
        }
    }

    private void OnAttack(UnitBase target, int firePointIndex)
    {
        UnitMob enemy = target as UnitMob;
        SimpleMissile missile = Instantiate(SimpleMissile, Center, Quaternion.identity);
        missile.EventHit = OnHitMissile;
        missile.Launch(enemy);
    }

    private void OnSkill(UnitBase[] targets)
    {
        foreach (UnitBase target in targets)
        {
            UnitMob mob = target as UnitMob;
            mob.GetDamaged(Property);
        }
    }

    private void OnHitMissile(UnitBase target)
    {
        UnitMob mob = target as UnitMob;
        mob.GetDamaged(Property);
        //ApplySlowDeBuff(mob);
    }
    private void ApplySlowDeBuff(UnitBase target)
    {
        DeBuffSlow buff = target.BuffCtrl.FindBuff<DeBuffSlow>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 결국 마린 여러마리가 공격해도 slow효과는 중복되지 않는 개념...
        else
            target.BuffCtrl.AddBuff(new DeBuffSlow(Property.SkillDuration));
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
