﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionSingleAttack : MotionBase
{
    [SerializeField] private AudioClip AttackSound = null;
    [SerializeField] private AnimationClip ReferenceAnim = null;
    [SerializeField] private int AnimCount = 1;
    [Range(0, 1)][SerializeField] private float FirePoint = 0.3f; //0 ~ 1

    public Action<UnitBase> EventFired { get; set; }
    private UnitMob Target = null;
    private float nextAttackTime = 0;

    public override bool IsReady()
    {
        if (nextAttackTime > Time.realtimeSinceStartup)
            return false;

        if(Target != null && Target.CurrentState != UnitState.Death)
        {
            if ((Target.transform.position - transform.position).magnitude < Unit.Property.AttackRange)
                return true;
            else
                Target = null;
        }

        UnitMob[] mobs = Unit.DetectAround<UnitMob>(Unit.Property.AttackRange);
        if (mobs == null)
            return false;

        Target = mobs[0];
        return true;
    }

    public override void OnEnter()
    {
        Unit.TurnHead(Target.transform.position);

        //대상의 위치에 따라 재생되는 attack 애니메이션을 다르게 해줘야 한다.
        float deg = Unit.CalcVerticalDegree(Target.transform.position);
        int stepDegree = 180 / AnimCount;
        int animIndex = (int)deg / stepDegree;
        Unit.Anim.SetTrigger("attack" + (animIndex + 1));

        SetAnimSpeed();
        float animSpeed = Unit.Anim.GetFloat("attackSpeed");
        float animPlayTime = ReferenceAnim.length / animSpeed;
        Invoke("OnFired", animPlayTime * FirePoint);
        Invoke("OnAnimationEnd", animPlayTime);
    }
    public override void OnLeave()
    {
        CancelInvoke("OnFired");
        CancelInvoke("OnAnimationEnd");
    }

    private void OnFired()
    {
        RGame.Get<RSoundManager>().PlaySFX(AttackSound);
        if (Target == null)
            return;

        nextAttackTime = Time.realtimeSinceStartup + (1 / Unit.Property.AttackSpeed);
        EventFired?.Invoke(Target);
    }
    private void OnAnimationEnd()
    {
        Unit.FSM.ChangeState(UnitState.Idle);
    }
    private void SetAnimSpeed()
    {
        float timePerAttack = 1 / Unit.Property.AttackSpeed;
        if (timePerAttack < ReferenceAnim.length)
            Unit.Anim.SetFloat("attackSpeed", ReferenceAnim.length / timePerAttack);
        else
            Unit.Anim.SetFloat("attackSpeed", 1);
    }
}

