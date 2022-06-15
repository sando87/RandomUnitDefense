using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public class MotionActionLoop : MotionBase
{
    [SerializeField] bool _Delayable = true;
    [SerializeField][ShowIf("_Delayable")] float _Delaytime = 5.0f;

    [SerializeField] bool _Detectable = false;
    [SerializeField][ShowIf("_Detectable")] float _Range = 3.0f;

    [SerializeField] float _Duration = 0;
    [SerializeField] int _AnimCount = 1;

    public Action<BaseObject> EventStart { get; set; }
    public Action<BaseObject> EventUpdate { get; set; }
    public Action EventEnd { get; set; }
    
    private BaseObject mTarget = null;

    private float Delaytime { get { return _Delaytime * mBaseObject.BuffProp.Cooltime; } }
    private float DetectRange { get { return _Range * mBaseObject.BuffProp.SkillRange; } }
    private float Duration { get { return _Duration * mBaseObject.BuffProp.SkillDuration; } }

    public override void OnInit()
    {
        base.OnInit();

        if (_Delayable)
            SetCooltime(Delaytime);
    }

    public override bool OnReady()
    {
        if (_Delayable && IsCooltime())
            return false;

        if (_Detectable)
        {
            Collider[] cols = mBaseObject.DetectAround(DetectRange, mBaseObject.GetLayerMaskAttackable());
            if (cols.Length == 0)
                return false;

            mTarget = cols[0].GetBaseObject();
        }

        return true;
    }

    public override void OnEnter()
    {
        if (mTarget != null)
        {
            mBaseObject.Body.TurnHeadTo(mTarget.transform.position);
            SetAnimParamVerticalDegreeIndex(mTarget.transform.position, _AnimCount);
        }

        base.OnEnter();

        EventStart?.Invoke(mTarget);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if(_Detectable) //감지 모드일 경우 타겟이 멀어지거나 죽거나 하면 idle로 전환
        {
            if(mTarget == null || mTarget.Body.Lock || IsOutOfRange(mTarget))
            {
                SwitchMotionToIdle();
                return;
            }
        }

        if(_Duration > 0) // 지속시간 모드일 경우 일정 시간이 지난 후 idle로 전환
        {
            if(PlayTime > Duration)
            {
                SwitchMotionToIdle();
                return;
            }
        }
        
        mBaseObject.Body.TurnHeadTo(mTarget.transform.position);
        SetAnimParamVerticalDegreeIndex(mTarget.transform.position, _AnimCount);
        EventUpdate?.Invoke(mTarget);
    }
    public override void OnLeave()
    {
        base.OnLeave();

        mTarget = null;
        if (_Delayable)
            SetCooltime(Delaytime);

        EventEnd?.Invoke();
    }
    private bool IsOutOfRange(BaseObject target)
    {
        return (target.transform.position - mBaseObject.transform.position).magnitude > (DetectRange * 1.2f);
    }
}

