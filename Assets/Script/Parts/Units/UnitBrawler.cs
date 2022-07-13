using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// return "3회공격시마다 (0.1/0.2/0.3)범위의 (1/1.5/2)초 스턴";

public class UnitBrawler : UnitBase
{
    [SerializeField] Transform HitPoint = null;
    [SerializeField] private Sprite[] HitSprites = null;

    [SerializeField] float _AttackRange = 0.1f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _StunDuration = 1;
    float StunDuration { get { return _StunDuration * mBaseObj.BuffProp.SkillDuration; } }
    
    private int mStunableCounter = 0;
    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = GetComponent<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionAttack(mMotionAttack, 1 / AttackSpeed));
    }

    protected IEnumerator CoMotionAttack(MotionBase motion, float motionCooltime)
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

                BaseObject target = null;
                Collider[] cols = GetEnemies();
                if (cols.Length <= 0)
                    continue;
                else
                    target = cols[0].GetBaseObject();

                motion.Target = target;
                mBaseObj.MotionManager.SwitchMotion(motion);

                yield return new WaitUntil(() => !mBaseObj.MotionManager.IsCurrentMotion(motion));
                break;
            }
        }
    }

    private void OnAttack(int idx)
    {
        mStunableCounter = (mStunableCounter + 1) % 3;
        if(mStunableCounter == 0)
        {
            SpritesAnimator.Play(HitPoint.position, HitSprites);
        }

        Collider[] cols = GetEnemies();
        foreach (Collider col in cols)
        {
            BaseObject target = col.GetBaseObject();
            if (target.Health != null)
            {
                target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            }

            if(mStunableCounter == 0)
            {
                MotionStun motionStun = target.MotionManager.FindMotion<MotionStun>();
                if(motionStun != null)
                {
                    motionStun.StunDuration = StunDuration;
                    target.MotionManager.SwitchMotion(motionStun);
                }
            }
        }
    }

    private Collider[] GetEnemies()
    {
        return Physics.OverlapSphere(HitPoint.position, AttackRange, 1 << LayerID.Enemies);
    }
}
