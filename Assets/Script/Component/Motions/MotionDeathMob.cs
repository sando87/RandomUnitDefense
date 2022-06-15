using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionDeathMob : MotionBasic
{
    [SerializeField] private AudioClip DeathSound = null;

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

        RGame.Get<RSoundManager>().PlaySFX(DeathSound);
        RGame.Get<RGameSystemManager>().DeathLineMob();
        mHealth.ShowHideHealthBar(false);
        mBaseObject.Body.Lock = true;
        mBaseObject.MotionManager.Lock = true;
        this.ExDelayedCoroutine(2.0f, () =>
        {
            mBaseObject.Renderer.FadeOut(1);
        });
        this.ExDelayedCoroutine(3.1f, () =>
        {
            Destroy(mBaseObject.gameObject);
        });
    }
}

