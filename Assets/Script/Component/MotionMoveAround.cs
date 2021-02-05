using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionMoveAround : MotionBase
{
    private Vector3[] WayPoints = null;

    public override UnitState State { get { return UnitState.Move; } }
    public override bool IsReady() { return false; }

    public override void OnInit()
    {
        base.OnInit();
        WayPoints = RGame.Get<RGameSystemManager>().GetWayPoints();
    }

    public override void OnEnter()
    {
        Unit.Anim.SetTrigger("move");
        StartCoroutine("MoveAround");
    }
    public override void OnLeave()
    {
        StopCoroutine("MoveAround");
    }

    private IEnumerator MoveAround()
    {
        int idx = 0;
        while (true)
        {
            //dest지점으로 유닛 Smoothly 이동
            Vector3 dest = WayPoints[idx];
            Unit.TurnHead(dest);
            float moveSpeed = Unit.Spec.MoveSpeed;
            Vector3 dir = dest - transform.position;
            dir.z = 0;
            float distance = dir.magnitude;
            dir.Normalize();
            float duration = distance / moveSpeed;
            float time = 0;
            while (time < duration)
            {
                transform.position += (dir * moveSpeed * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = dest;
            idx = (idx + 1) % WayPoints.Length;
            yield return null;
        }
    }
}

