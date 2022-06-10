using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionKeepAttack : MotionBase
{
    [SerializeField] private AudioClip AttackSound = null;
    [SerializeField] private AnimationClip ReferenceAnim = null;
    [SerializeField] private int AnimCount = 1;

    public Action<UnitBase> EventStart { get; set; }
    public Action<UnitBase> EventUpdate { get; set; }
    public Action EventEnd { get; set; }
    private UnitMob Target = null;
    private float nextAttackTime = 0;

    public override bool IsReady()
    {
        if (nextAttackTime > Time.realtimeSinceStartup)
            return false;

        UnitMob[] mobs = Unit.DetectAround<UnitMob>(Unit.Property.AttackRange);
        if (mobs == null)
            return false;

        Target = mobs[0];
        return true;
    }

    public override void OnEnter()
    {
        SetVerticalDegreeIndex();
        Unit.Anim.SetTrigger("motionB");

        EventStart?.Invoke(Target);

        RGame.Get<RSoundManager>().PlaySFX(AttackSound);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Target == null || Target.CurrentState == UnitState.Death)
        {
            Unit.FSM.ChangeState(UnitState.Idle);
        }
        else if ((Target.transform.position - transform.position).magnitude > Unit.Property.AttackRange)
        {
            Unit.FSM.ChangeState(UnitState.Idle);
        }
        else
        {
            Unit.TurnHead(Target.transform.position);
            SetVerticalDegreeIndex();
            EventUpdate?.Invoke(Target);
        }
    }
    public override void OnLeave()
    {
        base.OnLeave();
        nextAttackTime = Time.realtimeSinceStartup + (1 / Unit.Property.AttackSpeed);
        Unit.Anim.SetInteger("motionB_Num", 0);
        EventEnd?.Invoke();
    }

    private void SetAnimSpeed()
    {
        float timePerAttack = 1 / Unit.Property.AttackSpeed;
        if (timePerAttack < ReferenceAnim.length)
            Unit.Anim.SetFloat("attackSpeed", ReferenceAnim.length / timePerAttack);
        else
            Unit.Anim.SetFloat("attackSpeed", 1);
    }

    private void SetVerticalDegreeIndex()
    {
        //대상의 위치에 따라 재생되는 attack 애니메이션을 다르게 해줘야 한다.
        float deg = Unit.CalcVerticalDegree(Target.transform.position);
        int stepDegree = 180 / AnimCount;
        int animIndex = (int)deg / stepDegree;
        Unit.Anim.SetInteger("motionB_Num", animIndex + 1);
    }
}

