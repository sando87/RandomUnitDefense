using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionAppear : MotionBasic
{
    [SerializeField] private GameObject AppearParticle = null;
    private GameObject particle;

    public override void OnEnter()
    {
        base.OnEnter();
        particle = Instantiate(AppearParticle, mBaseObject.Body.Center, Quaternion.identity, transform);
        mBaseObject.CharacterInput.Lock = true;
        mBaseObject.MotionManager.Lock = true;
        this.ExDelayedCoroutine(0.5f, () => 
        {
            mBaseObject.MotionManager.Lock = false;
            SwitchMotionToIdle();
        });
    }
    public override void OnLeave()
    {
        mBaseObject.CharacterInput.Lock = false;
        Destroy(particle);
        base.OnLeave();
    }
}

