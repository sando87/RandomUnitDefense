﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레벨업에 따른 검기 범위 사거리 뎀지 증가

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
        int curLevel = mBaseObj.SpecProp.Level;
        BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        spec.damage = InGameSystem.Instance.SaveTestInfo.units[7].damages[curLevel - 1];
        spec.damagesPerUp[curLevel - 1] = InGameSystem.Instance.SaveTestInfo.units[7].damagesPerUp[curLevel - 1];
        
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        MotionAttackA.EventFired = OnAttack;
        MotionAttackB.EventFired = OnAttack;

        StartCoroutine(CoMotionSwitcherMelee(new MotionBase[] { MotionAttackA, MotionAttackB }, DetectArea));
        StartCoroutine(KeepBuff(BuffEffectPrefab));
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

}
