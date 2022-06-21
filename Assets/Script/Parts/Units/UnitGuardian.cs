using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitGuardian : UnitBase
{
    [SerializeField] BoxCollider AttackArea = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        GetComponent<MotionActionSingle>().EventFired = OnAttack;
    }

    private void OnAttack(Collider[] targets)
    {
        Collider[] cols = Physics.OverlapBox(AttackArea.bounds.center, AttackArea.bounds.extents, Quaternion.identity, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
                break;
            }
        }
    }
}
