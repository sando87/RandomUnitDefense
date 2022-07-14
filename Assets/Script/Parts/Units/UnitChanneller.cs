using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레이저로 누적데미지를 입힌다.

public class UnitChanneller : UnitBase
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 2;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }

    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    [SerializeField] float _SkillRange = 2;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] float _SkillDamageStep = 0.2f;

    private LaserAimming mLaserEffectObject = null;
    private MotionActionSingle mAttackMotion = null;
    private MotionActionLoop mLaserMotion = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mAttackMotion = GetComponent<MotionActionSingle>();
        mAttackMotion.EventFired = OnAttack;

        mLaserMotion = GetComponent<MotionActionLoop>();
        mLaserMotion.EventStart = OnAttackBeamStart;
        mLaserMotion.EventEnd = OnAttackBeamEnd;
        StartCoroutine(CoMotionSwitcher(mAttackMotion, 1 / AttackSpeed, AttackRange));
        StartCoroutine(CoMotionSwitcher(mLaserMotion, 0, SkillRange));
    }

    private void OnAttack(int idx)
    {
        ShootProjectail(mAttackMotion.Target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 dir = target.Body.Center - mBaseObj.Body.Center;
        dir.z = 0;
        SpritesAnimator.Play(mBaseObj.Body.Center, IntroSprites);

        SpritesAnimator proj = SpritesAnimator.Play(mBaseObj.Body.Center, ProjSprites, true);
        proj.transform.right = dir.normalized;
        proj.transform.DOMove(target.Body.Center, 0.3f).OnComplete(() =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);

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
        mLaserEffectObject = LaserAimming.Play(mBaseObj.Body.Center, target.gameObject);
        StartCoroutine(CoAttackBeam(target));
    }
    IEnumerator CoAttackBeam(BaseObject target)
    {
        float damage = 1;
        while (!IsOutOfSkillRange(target))
        {
            if(target.Health != null)
            {
                target.Health.GetDamaged(damage, mBaseObj);
            }
            yield return newWaitForSeconds.Cache(0.1f);
            damage += _SkillDamageStep;
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
        StopAllCoroutines();
    }
}
