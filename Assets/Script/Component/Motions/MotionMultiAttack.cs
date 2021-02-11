using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionMultiAttack : MotionBase
{
    [SerializeField] private AnimationClip ReferenceAnim = null;
    [SerializeField] private int AnimCount = 1;
    [SerializeField] private List<float> FirePoints = new List<float>();

    public Action<UnitBase, int> EventFired { get; set; }
    private UnitMob Target = null;
    private float NextAttackTime = 0;
    private float PlayTime = 0;
    private int FirePointIndex = 0;
    private float AnimTotalPlayTime = 0;

    public override bool IsReady()
    {
        if (NextAttackTime > Time.realtimeSinceStartup)
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
        PlayTime = 0;
        FirePointIndex = 0;
        Unit.TurnHead(Target.transform.position);
        NextAttackTime = Time.realtimeSinceStartup + (1 / Unit.Property.AttackSpeed);

        //대상의 위치에 따라 재생되는 attack 애니메이션을 다르게 해줘야 한다.
        float deg = Unit.CalcVerticalDegree(Target.transform.position);
        int stepDegree = 180 / AnimCount;
        int animIndex = (int)deg / stepDegree;
        Unit.Anim.SetTrigger("attack" + (animIndex + 1));

        SetAnimSpeed();
        AnimTotalPlayTime = ReferenceAnim.length / Unit.Anim.GetFloat("attackSpeed");
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        PlayTime += Time.deltaTime;
        if (AnimTotalPlayTime <= PlayTime)
        {
            //Attack 애니메이션 종료
            Unit.FSM.ChangeState(UnitState.Idle);
        }
        else
        {
            //PlayTime을 증가시키면서 FirePoint를 감지해 콜백해주는 코드
            if (FirePointIndex < FirePoints.Count)
            {
                float nextFireTime = AnimTotalPlayTime * FirePoints[FirePointIndex];
                if (nextFireTime < PlayTime)
                {
                    if (Target != null)
                        EventFired?.Invoke(Target, FirePointIndex);
                    FirePointIndex++;
                }
            }
        }
        
    }
    public override void OnLeave()
    {
        FirePointIndex = 0;
        PlayTime = 0;
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

