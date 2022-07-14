﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitGunner : UnitBase
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
    [SerializeField] float _SlowDuration = 1.0f;
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
        StartCoroutine("CoAttackBeam");
    }
    IEnumerator CoAttackBeam()
    {
        BaseObject target = mLaserMotion.Target;
        while (true)
        {
            ApplySlowDeBuff(target);
            yield return new WaitForSeconds(SlowDuration);
            if(IsOutOfSkillRange(target))
            {
                break;
            }
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
    private void ApplySlowDeBuff(BaseObject target)
    {
        DeBuffSlow buff = target.BuffCtrl.FindBuff<DeBuffSlow>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 결국 마린 여러마리가 공격해도 slow효과는 중복되지 않는 개념...
        else
            target.BuffCtrl.AddBuff(new DeBuffSlow(SlowDuration - 0.1f));
    }

    class DeBuffSlow : BuffBase
    {
        //적 이동속도 20% 감소 디버프(duration시간만큼 지속)
        public DeBuffSlow(float duration) { Duration = duration; }
        public override void StartBuff(BaseObject target)
        {
            target.Renderer.SetColor(Color.green);
            target.BuffProp.MoveSpeed -= 20;
        }
        public override void EndBuff(BaseObject target)
        {
            target.Renderer.SetColor(Color.white);
            target.BuffProp.MoveSpeed += 20;
        }
    }
}
