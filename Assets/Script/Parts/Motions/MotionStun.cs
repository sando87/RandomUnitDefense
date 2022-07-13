using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionStun : MotionBase
{
    public float StunDuration { get; set; } = 1;

    public override void OnEnter()
    {
        base.OnEnter();
        this.ExDelayedCoroutine(StunDuration, () => SwitchMotionToIdle());
    }
}

