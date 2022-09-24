using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레이저로 누적데미지를 입힌다.

public class UnitChanneller : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 2;
    [SerializeField] float _SkillRange = 2;
    [SerializeField] float _AccDamageStep = 0.2f;
    [SerializeField] [Range(0, 1)] float _DotDamageRate = 0.1f;

    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    private LaserAimming mLaserEffectObject = null;
    private MotionActionSingle mAttackMotion = null;
    private MotionActionLoop mLaserMotion = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mAttackMotion = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mAttackMotion.EventFired = OnAttack;

        mLaserMotion = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mLaserMotion.EventStart = OnAttackBeamStart;
        mLaserMotion.EventEnd = OnAttackBeamEnd;
        StartCoroutine(CoMotionSwitcher(mAttackMotion, () => AttackSpeed, () => AttackRange));
        StartCoroutine(CoMotionSwitcher(mLaserMotion, () => 0, () => SkillRange));
    }

    private void OnAttack(int idx)
    {
        ShootProjectail(mAttackMotion.Target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        Vector3 dir = target.Body.Center - mBaseObj.Body.Center;
        dir.z = 0;
        SpritesAnimator.Play(firePosition, IntroSprites);

        SpritesAnimator proj = SpritesAnimator.Play(firePosition, ProjSprites, true);
        proj.transform.right = dir.normalized;
        float damage = mBaseObj.SpecProp.Damage;
        proj.transform.CoMoveTo(target.Body.transform, 0.3f, () =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            target.Health.GetDamaged(damage, mBaseObj);

            Destroy(proj.gameObject);
        });
    }

    private void OnAttackBeamStart(BaseObject target)
    {
        if(mLaserEffectObject != null)
        {
            Destroy(mLaserEffectObject.gameObject);
            mLaserEffectObject = null;
        }
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        mLaserEffectObject = LaserAimming.Play(firePosition, target.Body.gameObject, "ChannellerLaser");
        mLaserEffectObject.transform.SetParent(mBaseObj.FirePosition.transform);
        StartCoroutine("CoAttackBeam");
    }
    IEnumerator CoAttackBeam()
    {
        BaseObject target = mLaserMotion.Target;
        float accDamage = 0;
        while (!IsOutOfSkillRange(target))
        {
            if(target.Health != null)
            {
                float damage = (mBaseObj.SpecProp.Damage + accDamage) * _DotDamageRate;
                target.Health.GetDamaged(damage, mBaseObj);
            }
            yield return newWaitForSeconds.Cache(0.1f);
            accDamage += _AccDamageStep;
        }
        mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
    }
    private bool IsOutOfSkillRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (SkillRange * 1.2f);
    }
    private void OnAttackBeamEnd()
    {
        if (mLaserEffectObject != null)
        {
            Destroy(mLaserEffectObject.gameObject);
            mLaserEffectObject = null;
        }
        StopCoroutine("CoAttackBeam");
    }
}
