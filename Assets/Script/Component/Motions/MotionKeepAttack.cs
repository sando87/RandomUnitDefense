using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionKeepAttack : MotionBase
{
    [SerializeField] private AudioClip AttackSound = null;
    [SerializeField] private AnimationClip ReferenceAnim = null;
    [SerializeField] private int AnimCount = 1;
    [Range(0, 1)][SerializeField] private float FirePoint = 0.3f; //0 ~ 1

    public Action<UnitBase> EventStart { get; set; }
    public Action<UnitBase> EventUpdate { get; set; }
    public Action EventEnd { get; set; }
    private UnitMob Target = null;

    public override bool IsReady()
    {
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
            EventUpdate?.Invoke(Target);
        }
    }
    public override void OnLeave()
    {
        base.OnLeave();
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
}

