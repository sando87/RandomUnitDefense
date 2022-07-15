using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public class MotionActionLoop : MotionBase
{
    [SerializeField] Transform[] _FirePositions = null;

    public Action<BaseObject> EventStart { get; set; }
    public Action<BaseObject> EventUpdate { get; set; }
    public Action EventEnd { get; set; }
    
    public override void OnEnter()
    {
        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, _FirePositions.Length);
        SetAnimParamVerticalDegreeIndex(vertIdx);
        mBaseObject.FirePosition.MovePosition(_FirePositions[vertIdx]);

        base.OnEnter();

        EventStart?.Invoke(Target);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, _FirePositions.Length);
        SetAnimParamVerticalDegreeIndex(vertIdx);
        mBaseObject.FirePosition.MovePosition(_FirePositions[vertIdx]);
        
        EventUpdate?.Invoke(Target);
    }
    public override void OnLeave()
    {
        base.OnLeave();

        EventEnd?.Invoke();
    }
}

