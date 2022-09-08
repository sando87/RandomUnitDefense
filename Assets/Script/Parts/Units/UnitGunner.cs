using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitGunner : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 2;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }

    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    [SerializeField] float _SkillDamage = 1;
    float SkillDamage { get { return _SkillDamage * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] float _SkillRange = 2;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] float _SlowDuration = 1.0f;
    float SlowDuration { get { return _SlowDuration * mBaseObj.BuffProp.SkillDuration; } }

    [SerializeField] BuffBase BuffPrefab = null;

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
        StartCoroutine(CoMotionSwitcher(mAttackMotion, 1 / AttackSpeed, AttackRange));
        StartCoroutine(CoMotionSwitcher(mLaserMotion, 0, SkillRange));
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
        proj.transform.CoMoveTo(target.Body.transform, 0.3f, () =>
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
                target.Health.GetDamaged(SkillDamage, mBaseObj);
            }
            target.BuffCtrl.ApplyBuff(BuffPrefab);
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
