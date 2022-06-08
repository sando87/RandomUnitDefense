using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGunner : UnitUser
{
    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private GameObject BuffEffect = null;

    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
        GetComponent<MotionSingleAttack>().EventFired = OnAttack;
        GetComponent<MotionKeepAttack>().EventStart = OnAttackBeamStart;
        GetComponent<MotionKeepAttack>().EventEnd = OnAttackBeamEnd;
    }
    public override void Release()
    {
        base.Release();
    }

    public override string SkillDescription
    {
        get
        {
            return "근접 거리의 적 유닛은 Beam공격으로 감속 효과";
        }
    }

    private void OnAttack(UnitBase target)
    {
        UnitMob enemy = target as UnitMob;
        enemy.GetDamaged(Property);
        Vector3 pos = Utils.Random(enemy.Center, 0.1f);
        GameObject obj = Instantiate(HitParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }

    private void OnAttackBeamStart(UnitBase target)
    {
        StartCoroutine(CoAttackBeam(target));
    }
    IEnumerator CoAttackBeam(UnitBase target)
    {
        while (true)
        {
            ApplySlowDeBuff(target);
            yield return new WaitForSeconds(Property.SkillDuration - 0.1f);
        }
    }
    private void OnAttackBeamEnd()
    {
        StopAllCoroutines();
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
            target.SR.color = Color.green;
            target.BuffValues.MoveSpeed -= 20;
        }
        public override void EndBuff(UnitBase target)
        {
            target.SR.color = Color.white;
            target.BuffValues.MoveSpeed += 20;
        }
    }

}
