using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MotionActionSingle : MotionBase
{
    [SerializeField] int _AnimCount = 1;
    [SerializeField][Range(0, 1)] float[] _FirePoints = null;

    public Action<int> EventFired { get; set; }

    public override void OnEnter()
    {
        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        SetAnimParamVerticalDegreeIndex(Target.transform.position, _AnimCount);

        base.OnEnter();

        StartCoroutine(CoUpdate());
    }
    IEnumerator CoUpdate()
    {
        for (int i = 0; i < _FirePoints.Length; ++i)
        {
            float firePoint = _FirePoints[i];
            yield return new WaitUntil(() => firePoint < NormalizedTime);
            EventFired?.Invoke(i);
        }
        yield return new WaitUntil(() => 1.0f <= NormalizedTime);
        SwitchMotionToIdle();
    }
    public override void OnLeave()
    {
        base.OnLeave();
        StopAllCoroutines();
    }
}

