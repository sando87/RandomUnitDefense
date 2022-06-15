using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MotionActionSingle : MotionBase
{
    [SerializeField] bool _IsSkillType = false;

    [SerializeField] bool _Delayable = true;
    [SerializeField][ShowIf("_Delayable")] float _Delaytime = 5.0f;

    [SerializeField] bool _Detectable = false;
    [SerializeField][ShowIf("_Detectable")] float _Range = 3.0f;

    [SerializeField] int _AnimCount = 1;
    [Range(0, 1)][SerializeField] float[] _FirePoints = null;

    public Action<Collider[]> EventFired { get; set; }
    private Collider[] mTargets = null;

    public override void OnInit()
    {
        base.OnInit();

        if(_Delayable)
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

            mTargets = cols;
        }

        return true;
    }

    public override void OnEnter()
    {
        if(mTargets != null && mTargets.Length > 0)
        {
            BaseObject target = mTargets[0].GetBaseObject();
            mBaseObject.Body.TurnHeadTo(target.transform.position);
            SetAnimParamVerticalDegreeIndex(target.transform.position, _AnimCount);
        }

        base.OnEnter();

        StartCoroutine(CoUpdate());
    }
    IEnumerator CoUpdate()
    {
        foreach(float firePoint in _FirePoints)
        {
            yield return new WaitUntil(() => firePoint < NormalizedTime);
            DoAttack();
        }
        yield return new WaitUntil(() => 1.0f <= NormalizedTime);
        SwitchMotionToIdle();
    }
    public override void OnLeave()
    {
        base.OnLeave();
        mTargets = null;
        StopAllCoroutines();
    }

    private void DoAttack()
    {
        if (_Delayable)
            SetCooltime(Delaytime);

        EventFired?.Invoke(mTargets);
    }

    private float Delaytime
    {
        get
        {
            if (_IsSkillType)
            {
                return _Delaytime * mBaseObject.BuffProp.Cooltime;
            }
            else
            {
                return 1 / ((1 / _Delaytime) * mBaseObject.BuffProp.AttackSpeed);
            }
        }
    }

    private float DetectRange
    {
        get
        {
            if (_IsSkillType)
            {
                return _Range * mBaseObject.BuffProp.SkillRange;
            }
            else
            {
                return _Range * mBaseObject.BuffProp.AttackRange;
            }
        }
    }
}

