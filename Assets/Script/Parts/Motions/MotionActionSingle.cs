using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MotionActionSingle : MotionBase
{
    [SerializeField] Transform[] _FirePositions = null; //스킬이 발사되는 위치(무기나 손의 위치)
    [SerializeField][Range(0, 1)] float[] _FireTimePoints = null; //실제 스킬이 발사되는 시간적 위치(전체 애니메이션을 1로 했을때)

    public Action<int> EventFired { get; set; }

    public override void OnEnter()
    {
        mBaseObject.Body.TurnHeadTo(Target.transform.position);
        int vertIdx = InGameUtils.GetVerticalIndex(mBaseObject.transform.position, Target.transform.position, _FirePositions.Length);
        SetAnimParamVerticalDegreeIndex(vertIdx);
        mBaseObject.FirePosition.MovePosition(_FirePositions[vertIdx]);

        base.OnEnter();

        StartCoroutine(CoUpdate());
    }
    IEnumerator CoUpdate()
    {
        for (int i = 0; i < _FireTimePoints.Length; ++i)
        {
            float firePoint = _FireTimePoints[i];
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

