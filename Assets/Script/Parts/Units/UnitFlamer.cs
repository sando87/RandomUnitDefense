using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitFlamer : UnitBase
{
    [SerializeField] Transform HitPoint = null;
    [SerializeField] private Sprite[] BurnSprites = null;

    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.SkillRange; } }
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        GetComponent<MotionActionSingle>().EventFired = OnAttack;
    }

    private void OnAttack(int idx)
    {
        StartCoroutine(CoAttack());
    }

    IEnumerator CoAttack()
    {
        SpritesAnimator effect = SpritesAnimator.Play(HitPoint.position, BurnSprites, true);
        while(true)
        {
            Collider[] cols = Physics.OverlapSphere(HitPoint.position, AttackRange, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
                }
            }

            yield return null;

            if(!mBaseObj.MotionManager.IsCurrentMotion<MotionActionSingle>())
                break;
        }
        Destroy(effect.gameObject);
    }
}
