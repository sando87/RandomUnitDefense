using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitGuardian : UnitBase
{
    [SerializeField] BoxCollider AttackArea = null;
    [SerializeField] private GameObject BuffEffectPrefab = null;
    
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField] float _SkillRange = 3.0f;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    
    [SerializeField] MotionActionSingle MotionAttackA = null;
    [SerializeField] MotionActionSingle MotionAttackB = null;
    [SerializeField] MotionActionSingle MotionAttackC = null;

    private BaseObject mTarget = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        MotionAttackA.EventFired = OnAttack;
        MotionAttackB.EventFired = OnAttack;

        StartCoroutine(CoMotionSwitcherMelee(new MotionBase[] { MotionAttackA, MotionAttackB }, 1 / AttackSpeed, AttackArea));
        StartCoroutine(RepeatBuff());
    }
    
    protected IEnumerator CoMotionSwitcherMelee(MotionBase[] motions, float motionCooltime, BoxCollider detectArea)
    {
        while (true)
        {
            if (motionCooltime > 0)
                yield return new WaitForSeconds(motionCooltime);

            while (true)
            {
                yield return null;
                if (!mBaseObj.MotionManager.IsCurrentMotion<MotionIdle>())
                    continue;

                mTarget = null;
                if (detectArea != null)
                {
                    Collider[] cols = mBaseObj.DetectAround(detectArea, 1 << LayerID.Enemies);
                    if (cols.Length <= 0)
                        continue;
                    else
                        mTarget = cols[0].GetBaseObject();
                }

                int motionIdx = UnityEngine.Random.Range(0, motions.Length);
                MotionBase motion = motions[motionIdx];

                motion.Target = mTarget;
                mBaseObj.MotionManager.SwitchMotion(motion);

                yield return new WaitUntil(() => !mBaseObj.MotionManager.IsCurrentMotion(motion));
                break;
            }
        }
    }

    private void OnAttack(int idx)
    {
        if(mTarget != null)
        {
            Health hp = mTarget.Health;
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            }
        }
    }
    
    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(SkillRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                ApplyAttackSpeedUpBuff(col.GetBaseObject());
        }
    }

    private void ApplyAttackSpeedUpBuff(BaseObject target)
    {
        BuffAttackDurationUP buff = target.BuffCtrl.FindBuff<BuffAttackDurationUP>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 중복 불가...
        else
        {
            GameObject buffEffectObj = Instantiate(BuffEffectPrefab, target.Renderer.transform);
            target.BuffCtrl.AddBuff(new BuffAttackDurationUP(buffEffectObj));
        }
    }

    class BuffAttackDurationUP : BuffBase
    {
        //1초간 아군 공속 20% 증소
        private GameObject BuffEffect;
        public BuffAttackDurationUP(GameObject buffEffect) { Duration = 1; BuffEffect = buffEffect; }
        public override void StartBuff(BaseObject target)
        {
            BuffEffect.SetActive(true);
            target.BuffProp.SkillDuration += 20;
        }
        public override void EndBuff(BaseObject target)
        {
            target.BuffProp.SkillDuration -= 20;
            Destroy(BuffEffect);
        }
    }
}
