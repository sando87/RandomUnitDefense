using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class MotionForced : MotionBase
{
    [SerializeField] float _Mass = 1;
    [SerializeField] float _RegVel = 15;
    [SerializeField] float _DelayHold = 0.01f;

    public Vector3 ExtForce { get; set; } = Vector3.zero;

    private Vector3 mVelocity = Vector3.zero;
    private Vector3 mForceDir = Vector3.zero;

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        mVelocity = ExtForce / _Mass;
        mVelocity.z = 0;
        mForceDir = mVelocity.normalized;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        Vector3 diff = (Destination - mBaseObject.transform.position).ZeroZ();
        mBaseObject.transform.position += (mVelocity * Time.deltaTime);
        mVelocity -= (mForceDir * _RegVel * Time.deltaTime);
        
        if(mVelocity.magnitude < 0.1f || Vector3.Dot(mVelocity, mForceDir) < 0)
        {
            this.ExDelayedCoroutine(_DelayHold, () => SwitchMotionToIdle());   
        }
    }
    public override void OnLeave()
    {
        base.OnLeave();
    }
}

