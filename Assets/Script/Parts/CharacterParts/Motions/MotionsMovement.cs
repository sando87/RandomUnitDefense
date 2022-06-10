using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MotionsMovement : MotionBasic 
{
    [SerializeField] float MoveSpeed = 6.0f;

    private CharacterPhysics mCharacterPhy = null;
    private Coroutine mCoMoving = null;

    public override void OnInitMotion()
    {
        base.OnInitMotion();

        mCharacterPhy = mBaseObject.CharacterPhy;
    }

    void Update()
    {
        // 캐릭터 좌우로 움직임.
        float inputHori = mCharacterInput.HorizontalMove;
        if (inputHori != 0 && mCoMoving == null && mBaseObject.Body.IsGrounded && IsCurrent<MotionsIdles>())
        {
            mCoMoving = StartCoroutine(CoMoving());
        }
    }

    IEnumerator CoMoving()
    {
        SwitchMotionThis();
        float time = 0;

        while(true)
        {
            float inputHori = mCharacterInput.HorizontalMove;
            if (inputHori == 0 || !mBaseObject.Body.IsGrounded)
                break;

            if (!mBaseObject.Body.IsObstacled)
                mCharacterPhy.VelocityX = inputHori * MoveSpeed;

            float degY = (inputHori > 0) ? 0 : ((inputHori < 0) ? 180 : (mBaseObject.transform.rotation.eulerAngles.y));
            mBaseObject.transform.rotation = Quaternion.Euler(0, degY, 0);

            // 지면 이동시 주기적으로 먼지 파티클 생성
            if(mBaseObject.gameObject.layer == LayerID.Player)
            {
                if (time <= 0)
                {
                    time = 0.3f;
                    GameObject vfx = ObjectPooling.Instance.InstantiateVFX("SmokeMove", mBaseObject.Body.Foot, Quaternion.identity);
                    float inversed = (inputHori > 0) ? 1 : ((inputHori < 0) ? -1 : (mBaseObject.transform.localScale.x));
                    vfx.transform.localScale = new Vector3(inversed, 1, 1);
                    vfx.ReturnAfter(1);
                }
                else
                {
                    time -= Time.deltaTime;
                }
            }

            yield return null;
        }

        SwitchMotion<MotionsIdles>();
    }

    public override void OnEnterMotion()
    {
        base.OnEnterMotion();
    }

    public override void OnExitMotion()
    {
        base.OnExitMotion();
        mCharacterPhy.VelocityX = 0;
        mCoMoving = null;
    }
}
