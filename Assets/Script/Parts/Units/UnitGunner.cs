using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레벨업에 따른 최대 이속 감소(레이져가 더 두껍게, 강하게 보이도록 필요)

public class UnitGunner : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 2;
    [SerializeField] float _SkillRange = 2;
    [SerializeField] float _SlowDuration = 1.0f;
    [SerializeField][Range(0, 1)] float DotDamageRate = 0.1f;

    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] BuffBase BuffPrefab = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    float SlowDuration { get { return _SlowDuration * mBaseObj.BuffProp.SkillDuration; } }

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
        Vector3 dir = target.Body.Center - firePosition;
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
        mLaserEffectObject = LaserAimming.Play(firePosition, target.Body.gameObject, "GunnerLaser");
        mLaserEffectObject.transform.SetParent(mBaseObj.FirePosition.transform);
        StartCoroutine("CoAttackBeam");
    }
    IEnumerator CoAttackBeam()
    {
        BaseObject target = mLaserMotion.Target;
        while (!IsOutOfSkillRange(target))
        {
            if(target.Health != null)
            {
                float damage = mBaseObj.SpecProp.Damage * DotDamageRate;
                target.Health.GetDamaged(damage, mBaseObj);
            }
            target.BuffCtrl.ApplyBuff(BuffPrefab, 1);
            yield return newWaitForSeconds.Cache(0.1f);
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
