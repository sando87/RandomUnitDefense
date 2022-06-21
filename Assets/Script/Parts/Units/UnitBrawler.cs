using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitBrawler : UnitBase
{
    [SerializeField] Transform HitPoint = null;
    [SerializeField] private Sprite[] HitSprites = null;

    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.SkillRange; } }
    
    private int mStunableCounter = 0;


    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        GetComponent<MotionActionSingle>().EventFired = OnAttack;
    }

    // public override string SkillDescription
    // {
    //     get
    //     {
    //         return "3회공격시마다 범위 스턴";
    //     }
    // }

    private void OnAttack(Collider[] targets)
    {
        mStunableCounter = (mStunableCounter + 1) % 3;
        if(mStunableCounter == 0)
        {
            SpritesAnimator.Play(HitPoint.position, HitSprites);
        }

        Collider[] cols = Physics.OverlapSphere(HitPoint.position, AttackRange, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            }
        }
    }
}
