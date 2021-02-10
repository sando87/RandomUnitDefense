using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionMove : MotionBase, IUserInputReciever
{
    private Vector3 DestWorldPos = Vector3.zero;

    public override UnitState State { get { return UnitState.Move; } }
    public override bool IsReady() { return false; }
    public void OnClick() { }
    public void OnDragging(Vector3 worldPos) { }

    public void OnDragAndDrop(Vector3 dropWorldPos)
    {
        DestWorldPos = dropWorldPos;
        Unit.FSM.ChangeState(UnitState.Move);
    }

    public override void OnEnter()
    {
        Unit.TurnHead(DestWorldPos);
        Unit.Anim.SetTrigger("move");
        StartCoroutine("MovingLoop");
    }
    public override void OnLeave()
    {
        StopCoroutine("MovingLoop");
    }

    public IEnumerator MovingLoop()
    {
        //dest지점으로 유닛 Smoothly 이동
        Vector3 dir = DestWorldPos - transform.position;
        dir.z = 0;
        dir.Normalize();
        while (true)
        {
            Vector3 nextPos = transform.position + (dir * Unit.Property.MoveSpeed * Time.deltaTime);
            Vector3 nextDir = DestWorldPos - nextPos;
            nextDir.z = 0;
            if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
            {
                transform.position = DestWorldPos;
                break;
            }
            else
            {
                transform.position = nextPos;
            }
            yield return null;
        }
        Unit.FSM.ChangeState(UnitState.Idle);
    }
}

