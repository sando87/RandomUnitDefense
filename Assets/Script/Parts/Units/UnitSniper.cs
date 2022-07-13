using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitSniper : UnitBase
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 3;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField][Range(0, 1)] float _CriticalPercent = 0.2f;
    float CriticalPercent { get { return _CriticalPercent * mBaseObj.BuffProp.Percentage; } }
    [SerializeField][Range(1, 10)] float _CriticalDamage = 3.0f;
    float CriticalDamage { get { return _CriticalDamage; } }

    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] LaserSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] private Sprite[] AimingSprites = null;

    private SpritesAnimator mAimingEffect = null;
    private MotionActionLoop mMotionAiming = null;
    private MotionActionSingle mMotionShoot = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAiming = GetComponent<MotionActionLoop>();
        mMotionAiming.EventStart = OnAttackBeamStart;
        mMotionAiming.EventUpdate = OnAttackBeamUpdate;
        mMotionAiming.EventEnd = OnAttackBeamEnd;

        mMotionShoot = GetComponent<MotionActionSingle>();
        mMotionShoot.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAiming, 1 / AttackSpeed, AttackRange));
    }

    private void OnAttackBeamStart(BaseObject target)
    {
        if(mAimingEffect != null)
        {
            Destroy(mAimingEffect.gameObject);
            mAimingEffect = null;
        }
        mAimingEffect = SpritesAnimator.Play(target.Body.Center, AimingSprites, false);
        mAimingEffect.transform.SetParent(target.transform);
        mAimingEffect.EventEnd = () => mBaseObj.MotionManager.SwitchMotion(mMotionShoot);
    }
    private void OnAttackBeamUpdate(BaseObject target)
    {
        if (IsOutOfRange(target))
        {
            mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
        }
    }
    private bool IsOutOfRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (AttackRange * 1.2f);
    }
    private void OnAttackBeamEnd()
    {
        if (mAimingEffect != null)
        {
            Destroy(mAimingEffect.gameObject);
            mAimingEffect = null;
        }
        StopAllCoroutines();
    }

    private void OnAttack(int idx)
    {
        BaseObject target = mMotionShoot.Target;
        ShootProjectail(target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 dir = target.Body.Center - mBaseObj.Body.Center;
        dir.z = 0;

        SpritesAnimator trail = SpritesAnimator.Play(mBaseObj.Body.Center, LaserSprites);
        trail.transform.right = dir.normalized;

        SpritesAnimator.Play(target.Body.Center, OutroSprites);

        float damage = mBaseObj.SpecProp.Damage;
        int percent = (int)(CriticalPercent * 100.0f);
        if (UnityEngine.Random.Range(0, 100) < percent)
        {
            damage *= CriticalDamage;
        }

        target.Health.GetDamaged(damage, mBaseObj);
    }
}
