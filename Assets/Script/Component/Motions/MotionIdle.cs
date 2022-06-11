using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionIdle : MotionBasic
{
    public override void OnEnter()
    {
        base.OnEnter();
        SetTrigger(AnimActionID.Idle);
    }
}

