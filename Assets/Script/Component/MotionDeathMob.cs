using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionDeathMob : MotionBase
{
    public override UnitState State { get { return UnitState.Death; } }
    public override bool IsReady() { return false; }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnEnter()
    {
        RGame.Get<RGameSystemManager>().DeathLineMob();
        ((UnitMob)Unit).HPBar.HideBar();
        Unit.Anim.SetTrigger("death");
        StartCoroutine("MoveAround");
        Destroy(gameObject, 1.0f);
    }
}

