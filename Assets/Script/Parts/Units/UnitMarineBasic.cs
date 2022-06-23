using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineBasic : UnitBase
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField] float _BuffRange = 3.0f;
    float BuffRange { get { return _BuffRange * mBaseObj.BuffProp.SkillRange; } }

    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private GameObject BuffEffect = null;

    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
    }

    void OnEnable() 
    {
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
        StartCoroutine(RepeatBuff());
    }

    private void OnAttack(Collider[] targets)
    {
        BaseObject target = targets[0].GetBaseObject();
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);

        Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
        GameObject obj = Instantiate(HitParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(BuffRange, 1 << mBaseObj.gameObject.layer);
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
