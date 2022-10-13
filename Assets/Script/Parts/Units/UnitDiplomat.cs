using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 피격시 범위 피해
// 레벨업에 따른 개수 증가

public class UnitDiplomat : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _SplshRange = 0.1f;
    [SerializeField] float _SkillRange = 3.0f;
    
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] BuffBase BuffEffect = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    private MotionActionSingle mMotionAttack = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));

        // StartCoroutine(RepeatBuff());
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
        float damage = mBaseObj.SpecProp.Damage;
        float splashRange = SplshRange + ((mBaseObj.SpecProp.Level - 1) * 0.1f);
        float projSize = proj.transform.localScale.x + ((mBaseObj.SpecProp.Level - 1) * 0.2f);
        proj.transform.DOScale(projSize, 0.3f);

        proj.transform.CoMoveTo(target.Body.transform, 0.3f, () =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            Collider[] cols = target.DetectAround(splashRange, 1 << LayerID.Enemies);
            foreach(Collider col in cols)
            {
                col.GetBaseObject().Health.GetDamaged(damage, mBaseObj);
            }

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
