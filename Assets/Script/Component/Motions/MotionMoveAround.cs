using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionMoveAround : MotionBasic
{
    [SerializeField] float _MoveSpeed = 3.0f;

    private Vector3[] WayPoints = null;
    private int WayPointIndex = 0;

    public override void OnInit()
    {
        base.OnInit();
        WayPoints = RGame.Get<RGameSystemManager>().GetWayPoints();
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

            Vector3 dir = dest - transform.position;
            dir.z = 0;
            dir.Normalize();
            while (true)
            {
                float moveSpeed = _MoveSpeed * mBaseObject.BuffProp.MoveSpeed;
                Vector3 nextPos = transform.position + (dir * moveSpeed * Time.deltaTime);
                Vector3 nextDir = dest - nextPos;
                nextDir.z = 0;
                if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
                {
                    transform.position = dest;
                    break;
                }
                else
                {
                    transform.position = nextPos;
                }
                yield return null;
            }

            WayPointIndex = (WayPointIndex + 1) % WayPoints.Length;
            yield return null;
        }
    }
}

