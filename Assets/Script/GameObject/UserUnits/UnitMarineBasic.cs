using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineBasic : UnitUser
{
    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private GameObject BuffEffect = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mBaseObj.MotionManager.FindMotion<MotionSingleAttack>().EventFired = OnAttack;
        StartCoroutine(RepeatBuff());
    }

    // public override string SkillDescription
    // {
    //     get
    //     {
    //         return "주변 유닛 공격 속도 20% 증가(패시브)";
    //     }
    // }

    private void OnAttack(Collider[] targets)
    {
        BaseObject target = targets[0].GetBaseObject();
        target.Health.GetDamaged(mBaseObj);

        Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
        GameObject obj = Instantiate(HitParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return Yielders.GetWaitForSeconds(0.5f);
            Collider[] cols = mBaseObj.DetectAround(mBaseObj.SpecProp.SkillRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                ApplyAttackSpeedUpBuff(col.GetBaseObject());
        }
    }

    private void ApplyAttackSpeedUpBuff(BaseObject target)
    {
        BuffAttackSpeedUP buff = target.BuffCtrl.FindBuff<BuffAttackSpeedUP>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 중복 불가...
        else
        {
            GameObject buffEffectObj = Instantiate(BuffEffect, target.Renderer.transform);
            target.BuffCtrl.AddBuff(new BuffAttackSpeedUP(buffEffectObj));
        }
    }

    class BuffAttackSpeedUP : BuffBase
    {
        //1초간 아군 공속 20% 증소
        private GameObject BuffEffect;
        public BuffAttackSpeedUP(GameObject buffEffect) { Duration = 1; BuffEffect = buffEffect; }
        public override void StartBuff(BaseObject target)
        {
            BuffEffect.SetActive(true);
            target.BuffProp.AttackSpeed += 20;
        }
        public override void EndBuff(BaseObject target)
        {
            target.BuffProp.AttackSpeed -= 20;
            Destroy(BuffEffect);
        }
    }

}
