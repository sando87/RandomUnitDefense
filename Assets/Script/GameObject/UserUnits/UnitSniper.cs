using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitSniper : UnitUser
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] LaserSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] private Sprite[] AimingSprites = null;

    private SpritesAnimator mAimingEffect = null;

    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
        GetComponent<MotionSingleAttack>().EventFired = OnAttack;
        GetComponent<MotionKeepAttack>().EventStart = OnAttackBeamStart;
        GetComponent<MotionKeepAttack>().EventEnd = OnAttackBeamEnd;
    }
    public override void Release()
    {
        base.Release();
    }

    public override string SkillDescription
    {
        get
        {
            return "일정 확률로 크리티컬";
        }
    }

    private void OnAttack(UnitBase target)
    {
        ShootProjectail(target);
    }

    private void OnAttackBeamStart(UnitBase target)
    {
        if(mAimingEffect != null)
        {
            Destroy(mAimingEffect.gameObject);
            mAimingEffect = null;
        }
        mAimingEffect = SpritesAnimator.Play(target.Center, AimingSprites, false);
        mAimingEffect.transform.SetParent(target.transform);
        mAimingEffect.EventEnd = OnEndAiming;
    }
    private void OnEndAiming()
    {
        FSM.ChangeState(UnitState.Attack);
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
    private void ShootProjectail(UnitBase target)
    {
        Vector3 dir = target.Center - Center;
        dir.z = 0;

        SpritesAnimator trail = SpritesAnimator.Play(Center, LaserSprites);
        trail.transform.right = dir.normalized;

        SpritesAnimator.Play(target.Center, OutroSprites);

        UnitMob enemy = target as UnitMob;
        enemy.GetDamaged(Property);
    }
}
