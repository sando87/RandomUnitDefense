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
    [SerializeField][Range(1, 10)] float _CriticalDamageMultiplier = 3.0f;
    float CriticalDamageMultiplier { get { return _CriticalDamageMultiplier; } }

    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] private Sprite[] AimingSprites = null;

    private SpritesAnimator mAimingEffect = null;
    private MotionActionLoop mMotionAiming = null;
    private MotionActionSingle mMotionShoot = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAiming = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mMotionAiming.EventStart = OnAttackBeamStart;
        mMotionAiming.EventUpdate = OnAttackBeamUpdate;
        mMotionAiming.EventEnd = OnAttackBeamEnd;

        mMotionShoot = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
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
        mAimingEffect.EventEnd = () => 
        {
            mMotionShoot.Target = target;
            mBaseObj.MotionManager.SwitchMotion(mMotionShoot);
        };
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
    }

    private void OnAttack(int idx)
    {
        BaseObject target = mMotionAiming.Target;
        ShootProjectail(target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        LaserAimming laserObj = LaserAimming.Play(firePosition, target.Body.gameObject, "SniperLaser");
        this.ExDelayedCoroutine(1, () => Destroy(laserObj.gameObject));

        SpritesAnimator.Play(target.Body.Center, OutroSprites);

        float damage = mBaseObj.SpecProp.Damage;
        int percent = (int)(CriticalPercent * 100.0f);
        if (UnityEngine.Random.Range(0, 100) < percent)
        {
            damage *= CriticalDamageMultiplier;
        }

        target.Health.GetDamaged(damage, mBaseObj);
    }
}
