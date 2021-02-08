using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUser : UnitBase
{
    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
    }
    public override void Release()
    {
        base.Release();
    }
}
