using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionMoveAround : MotionBase
{
    private Vector3[] WayPoints = null;
    private int WayPointIndex = 0;

    public override void OnInit()
    {
        base.OnInit();
        WayPoints = InGameSystem.Instance.GetWayPoints();
        WayPointIndex = 0;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine("MoveAround");
    }
    public override void OnLeave()
    {
        StopCoroutine("MoveAround");
        base.OnLeave();
    }

    private IEnumerator MoveAround()
    {
        yield return null;

        while (true)
        {
            Vector3 dest = WayPoints[WayPointIndex];
            mBaseObject.Body.TurnHeadTo(dest);

            Vector3 dir = dest - mBaseObject.transform.position;
            dir.z = 0;
            dir.Normalize();
            while (true)
            {
                float moveSpeed = mBaseObject.SpecProp.MoveSpeed;
                Vector3 nextPos = mBaseObject.transform.position + (dir * moveSpeed * Time.deltaTime);
                Vector3 nextDir = dest - nextPos;
                nextDir.z = 0;
                if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
                {
                    mBaseObject.transform.position = dest;
                    break;
                }
                else
                {
                    mBaseObject.transform.position = nextPos;
                }
                yield return null;
            }

            WayPointIndex = (WayPointIndex + 1) % WayPoints.Length;
            yield return null;
        }
    }
}

