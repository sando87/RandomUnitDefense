﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class MotionMove : MotionBase
{
    public System.Action EventArrived = null;

    public override void OnInit()
    {
        base.OnInit();

        UserInput userInput = mBaseObject.CharacterInput as UserInput;
        if(userInput != null)
        {
            userInput.EventMove += OnMoveInput;
        }
    }

    private void OnMoveInput(Vector3 destination)
    {
        Destination = destination;
        SwitchMotionToThis();
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        mBaseObject.Body.TurnHeadTo(Destination);

        Vector3 diff = (Destination - mBaseObject.transform.position).ZeroZ();
        mBaseObject.transform.position += (diff.normalized * mBaseObject.SpecProp.MoveSpeed * Time.deltaTime);
        if(diff.magnitude < 0.01f || Vector3.Dot(diff, (Destination - mBaseObject.transform.position)) < 0)
        {
            EventArrived?.Invoke();
            SwitchMotionToIdle();
        }
    }
    public override void OnLeave()
    {
        base.OnLeave();
    }
}

