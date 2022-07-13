using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 바닥에 불길을 생성하여 적을 불태운다.

public class UnitFlamer : UnitBase
{
    [SerializeField] private Sprite[] BurnSprites = null;

    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField][Range(0, 1)] float _SkillRange = 0.2f;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] float _SkillDuration = 3.0f;
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    private MotionActionSingle mMotionAttack = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
    }

    private void OnAttack(int idx)
    {
        StartCoroutine(CoAttack());
    }

    IEnumerator CoAttack()
    {
        Vector3 firePoint = mMotionAttack.Target.transform.position;
        SpritesAnimator.Play(firePoint, BurnSprites);

        while(true)
        {
            Collider[] cols = Physics.OverlapSphere(firePoint, SkillRange, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
                }
            }

            yield return null;

            if(!mBaseObj.MotionManager.IsCurrentMotion(mMotionAttack))
                break;
        }
    }
}
