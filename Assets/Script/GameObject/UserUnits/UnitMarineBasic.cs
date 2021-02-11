using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineBasic : UnitUser
{
    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private GameObject BuffEffect = null;

    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
        GetComponent<MotionSingleAttack>().EventFired = OnAttack;
        StartCoroutine(RepeatBuff());
    }
    public override void Release()
    {
        base.Release();
    }

    public override string SkillDescription
    {
        get
        {
            return "주변 유닛 공격 속도 20% 증가(패시브)";
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

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return Yielders.GetWaitForSeconds(0.5f);
            UnitUser[] units = DetectAround<UnitUser>(Property.SkillRange);
            if(units != null && units.Length > 0)
            {
                foreach (UnitUser unit in units)
                    ApplyAttackSpeedUpBuff(unit);
            }
        }
    }

    private void ApplyAttackSpeedUpBuff(UnitBase target)
    {
        BuffAttackSpeedUP buff = target.BuffCtrl.FindBuff<BuffAttackSpeedUP>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 중복 불가...
        else
        {
            GameObject buffEffectObj = Instantiate(BuffEffect, target.transform);
            target.BuffCtrl.AddBuff(new BuffAttackSpeedUP(buffEffectObj));
        }
    }

    class BuffAttackSpeedUP : BuffBase
    {
        //1초간 아군 공속 20% 증소
        private GameObject BuffEffect;
        public BuffAttackSpeedUP(GameObject buffEffect) { Duration = 1; BuffEffect = buffEffect; }
        public override void StartBuff(UnitBase target)
        {
            BuffEffect.SetActive(true);
            target.BuffValues.AttackSpeed += 20;
        }
        public override void EndBuff(UnitBase target)
        {
            target.BuffValues.AttackSpeed -= 20;
            Destroy(BuffEffect);
        }
    }

}
