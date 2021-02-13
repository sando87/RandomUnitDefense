using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionMove : MotionBase
{
    [SerializeField] private AudioClip MoveSound = null;

    public Vector3 Destination { get; set; }
    public override bool IsReady() { return false; }

    public override void OnEnter()
    {
        Unit.TurnHead(Destination);
        Unit.Anim.SetTrigger("move");
        StartCoroutine("MovingLoop");
        RGame.Get<RSoundManager>().PlaySFX(MoveSound);
    }
    public override void OnLeave()
    {
        StopCoroutine("MovingLoop");
    }

    public IEnumerator MovingLoop()
    {
        //dest지점으로 유닛 Smoothly 이동
        Vector3 dir = Destination - transform.position;
        dir.z = 0;
        dir.Normalize();
        while (true)
        {
            Vector3 nextPos = transform.position + (dir * Unit.Property.MoveSpeed * Time.deltaTime);
            Vector3 nextDir = Destination - nextPos;
            nextDir.z = 0;
            if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
            {
                transform.position = Destination;
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

