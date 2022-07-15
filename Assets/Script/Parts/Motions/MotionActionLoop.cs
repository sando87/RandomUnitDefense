using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public class MotionActionLoop : MotionBase
{
    [SerializeField] Transform[] _FirePositions = null;

    private int VertAnimCount { get { return _FirePositions == null ? 0 : _FirePositions.Length; } }

    public Action<BaseObject> EventStart { get; set; }
    public Action<BaseObject> EventUpdate { get; set; }
    public Action EventEnd { get; set; }
    
    public override void OnEnter()
    {
        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, VertAnimCount);
        SetAnimParamVerticalDegreeIndex(vertIdx);
        mBaseObject.FirePosition.MovePosition(GetFirePositionTranform(vertIdx));

        base.OnEnter();

        EventStart?.Invoke(Target);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, VertAnimCount);
        SetAnimParamVerticalDegreeIndex(vertIdx);
        mBaseObject.FirePosition.MovePosition(GetFirePositionTranform(vertIdx));
        
        EventUpdate?.Invoke(Target);
    }
    public override void OnLeave()
    {
        base.OnLeave();

        EventEnd?.Invoke();
    }
    private Transform GetFirePositionTranform(int idx)
    {
        if(_FirePositions == null || idx < 0 || idx >= _FirePositions.Length)
            return transform;
        
        return _FirePositions[idx];
    }
}

