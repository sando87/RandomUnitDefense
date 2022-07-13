﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class MotionMove : MotionBase
{
    private Tween mMoveTween = null;

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

        mBaseObject.Body.TurnHeadTo(Destination);

        float dist = (Destination - mBaseObject.transform.position).ZeroZ().magnitude;
        float duration = dist / mBaseObject.SpecProp.MoveSpeed;
        mMoveTween = mBaseObject.transform.DOMove(Destination, duration).OnComplete(() =>
        {
            mMoveTween = null;
            SwitchMotionToIdle();
        });
    }
    public override void OnLeave()
    {
        if (mMoveTween != null)
        {
            mMoveTween.Kill();
            mMoveTween = null;
        }

        base.OnLeave();
    }
}

