using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public class MotionActionLoop : MotionBase
{
    [SerializeField] int _AnimCount = 1;

    public Action<BaseObject> EventStart { get; set; }
    public Action<BaseObject> EventUpdate { get; set; }
    public Action EventEnd { get; set; }
    
    public override void OnEnter()
    {
        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        SetAnimParamVerticalDegreeIndex(Target.transform.position, _AnimCount);

        base.OnEnter();

        EventStart?.Invoke(Target);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        SetAnimParamVerticalDegreeIndex(Target.transform.position, _AnimCount);
        
        EventUpdate?.Invoke(Target);
    }
    public override void OnLeave()
    {
        base.OnLeave();

        EventEnd?.Invoke();
    }
}

