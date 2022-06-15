using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MotionsMovement : MotionBasic 
{
    [SerializeField] float MoveSpeed = 6.0f;

    private Coroutine mCoMoving = null;

    public override void OnInit()
    {
        base.OnInit();
    }

    void Update()
    {
        // 캐릭터 좌우로 움직임.
        float inputHori = mCharacterInput.GetInput(InputType.KeyA);
        if (inputHori != 0 && mCoMoving == null && IsCurrent<MotionsIdles>())
        {
            mCoMoving = StartCoroutine(CoMoving());
        }
    }

    IEnumerator CoMoving()
    {
        SwitchMotionToThis();

        while(true)
        {
            float inputHori = mCharacterInput.GetInput(InputType.KeyA);
            if (inputHori == 0)
                break;

            float degY = (inputHori > 0) ? 0 : ((inputHori < 0) ? 180 : (mBaseObject.transform.rotation.eulerAngles.y));
            mBaseObject.transform.rotation = Quaternion.Euler(0, degY, 0);

            yield return null;
        }

        SwitchMotion<MotionsIdles>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnLeave()
    {
        base.OnLeave();
        mCoMoving = null;
    }
}
