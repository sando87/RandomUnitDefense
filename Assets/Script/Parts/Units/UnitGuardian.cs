using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitGuardian : UnitPlayer
{
    [SerializeField] BoxCollider DetectArea = null;
    [SerializeField] BuffBase BuffEffectPrefab = null;
    
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _SkillRange = 3.0f;

    [SerializeField] MotionActionSingle MotionAttackA = null;
    [SerializeField] MotionActionSingle MotionAttackB = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    private BaseObject mTarget = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        MotionAttackA.EventFired = OnAttack;
        MotionAttackB.EventFired = OnAttack;

        StartCoroutine(CoMotionSwitcherMelee(new MotionBase[] { MotionAttackA, MotionAttackB }, DetectArea));
        StartCoroutine(RepeatBuff());
    }
    
    protected IEnumerator CoMotionSwitcherMelee(MotionBase[] motions, BoxCollider detectArea)
    {
        while (true)
        {
            float motionCooltime = 1 / AttackSpeed;
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
                col.GetBaseObject().BuffCtrl.ApplyBuff(BuffEffectPrefab);
        }
    }

}
