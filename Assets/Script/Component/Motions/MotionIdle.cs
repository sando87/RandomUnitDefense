using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionIdle : MotionBase
{
    private MotionBase[] BaseMotions = null;
    public override bool IsReady() { return false; }

    public override void OnInit()
    {
        BaseMotions = Unit.GetComponents<MotionBase>();
    }
    public override void OnEnter()
    {
        Unit.Anim.SetTrigger("idle");
    }
    public override void OnUpdate()
    {
        int cnt = BaseMotions.Length;
        for(int i = 0; i < cnt; ++i)
        {
            MotionBase motion = BaseMotions[i];
            if (motion.State == UnitState.Idle || motion.State == UnitState.Move)
                continue;

            if (motion.IsReady())
            {
                Unit.FSM.ChangeState(motion.State);
                break;
            }
        }
    }
}

