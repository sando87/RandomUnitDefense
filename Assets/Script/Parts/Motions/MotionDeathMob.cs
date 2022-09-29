using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionDeathMob : MotionBase
{
    private Health mHealth = null;

    public override void OnInit()
    {
        base.OnInit();
        mHealth = mBaseObject.Health;
        if(mHealth != null)
        {
            mHealth.EventDamaged += OnDeath;
        }
    }

    private void OnDeath(float damage, BaseObject attacker)
    {
        if(mHealth.IsDead)
        {
            SwitchMotionToThis();
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();

        InGameSystem.Instance.DeathLineMob(mBaseObject);
        mBaseObject.Body.Lock = true;
        mBaseObject.MotionManager.Lock = true;
        this.ExDelayedCoroutine(5.0f, () =>
        {
            mBaseObject.Renderer.FadeOut(1);
        });
        this.ExDelayedCoroutine(6.1f, () =>
        {
            Destroy(mBaseObject.gameObject);
        });
    }
}

