using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitSniper : UnitBase
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] LaserSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] private Sprite[] AimingSprites = null;

    private SpritesAnimator mAimingEffect = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        GetComponent<MotionActionLoop>().EventStart = OnAttackBeamStart;
        GetComponent<MotionActionLoop>().EventEnd = OnAttackBeamEnd;
        GetComponent<MotionActionSingle>().EventFired = OnAttack;
    }

    // public override string SkillDescription
    // {
    //     get
    //     {
    //         return "일정 확률로 크리티컬";
    //     }
    // }

    private void OnAttackBeamStart(BaseObject target)
    {
        if(mAimingEffect != null)
        {
            Destroy(mAimingEffect.gameObject);
            mAimingEffect = null;
        }
        mAimingEffect = SpritesAnimator.Play(target.Body.Center, AimingSprites, false);
        mAimingEffect.transform.SetParent(target.transform);
        mAimingEffect.EventEnd = OnEndAiming;
    }
    private void OnEndAiming()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionActionSingle>();
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

    private void OnAttack(Collider[] targets)
    {
        BaseObject target = targets[0].GetBaseObject();
        ShootProjectail(target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 dir = target.Body.Center - mBaseObj.Body.Center;
        dir.z = 0;

        SpritesAnimator trail = SpritesAnimator.Play(mBaseObj.Body.Center, LaserSprites);
        trail.transform.right = dir.normalized;

        SpritesAnimator.Play(target.Body.Center, OutroSprites);

        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
    }
}
