using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 스킬 사거리 20%증가

public class UnitDiplomat : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _SkillRange = 3.0f;
    
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] BuffBase BuffEffect = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    private MotionActionSingle mMotionAttack = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));
        StartCoroutine(RepeatBuff());
    }

    void OnAttack(int idx)
    {
        ShootProjectail(mMotionAttack.Target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        Vector3 dir = target.Body.Center - firePosition;
        dir.z = 0;

        SpritesAnimator proj = SpritesAnimator.Play(firePosition, ProjSprites, true);
        proj.transform.right = dir.normalized;
        proj.transform.CoMoveTo(target.Body.transform, 0.3f, () =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);

            Destroy(proj.gameObject);
        });
    }

    

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(SkillRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                col.GetBaseObject().BuffCtrl.ApplyBuff(BuffEffect, 1);
        }
    }
}
