using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public class MotionActionLoop : MotionBase
{
    [SerializeField] Transform[] _FirePositions = null;
    [SerializeField] bool _IsAutoAiming = true;

    public float Duration = -1;

    private int VertAnimCount { get { return _FirePositions == null ? 0 : _FirePositions.Length; } }

    public Action<BaseObject> EventStart { get; set; }
    public Action<BaseObject> EventUpdate { get; set; }
    public Action EventEnd { get; set; }

    private float mTime = 0;
    
    public override void OnEnter()
    {
        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, VertAnimCount);
        SetAnimParamVerticalIndexFloat(vertIdx);
        mBaseObject.FirePosition.MovePosition(GetFirePositionTranform(vertIdx));
        mTime = 0;

        base.OnEnter();

        EventStart?.Invoke(Target);
    }
    public override void OnUpdate()
    {
        if(Duration > 0)
        {
            mTime += Time.deltaTime;
            if(mTime > Duration)
            {
                SwitchMotionToIdle();
                return;
            }
        }
        else
        {
            if (Target == null || Target.Health.IsDead)
            {
                SwitchMotionToIdle();
                return;
            }
        }

        base.OnUpdate();

        if(_IsAutoAiming)
        {
            mBaseObject.Body.TurnHeadTo(Target.transform.position);
            int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, VertAnimCount);
            SetAnimParamVerticalIndexFloat(vertIdx);
            mBaseObject.FirePosition.MovePosition(GetFirePositionTranform(vertIdx));
        }
        
        EventUpdate?.Invoke(Target);
    }
    public override void OnLeave()
    {
        base.OnLeave();
        mTime = 0;

        EventEnd?.Invoke();
    }
    private Transform GetFirePositionTranform(int idx)
    {
        if(_FirePositions == null || idx < 0 || idx >= _FirePositions.Length)
            return transform;
        
        return _FirePositions[idx];
    }
}

