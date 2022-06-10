using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        mBaseObject.Health.EventDamaged += OnDamaged;
    }

    private void OnDamaged(float damage, BaseObject attacker)
    {
        if(damage > 0 && !mBaseObject.Health.IsDead)
        {
            mBaseObject.Renderer.StartTwinkle();
        }
    }
}
