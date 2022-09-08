using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class MotionMove : MotionBase
{
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
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        mBaseObject.Body.TurnHeadTo(Destination);

        Vector3 diff = (Destination - mBaseObject.transform.position).ZeroZ();
        mBaseObject.transform.position += (diff.normalized * mBaseObject.SpecProp.MoveSpeed * Time.deltaTime);
        if(diff.magnitude < 0.1f)
        {
            SwitchMotionToIdle();
        }
    }
    public override void OnLeave()
    {
        base.OnLeave();
    }
}

