using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    [SerializeField] int DestLocalIndexX = 3;
    [SerializeField] int DestLocalIndexY = 0;
    [SerializeField] float MoveSpeed = 5;

    private BaseObject mBaseObject = null;

    private Vector3 mStartWorldPos = Vector3.zero;
    private Vector3 mEndWorldPos = Vector3.zero;
    private float mDuration = 0;
    private Vector3 mVelocity = Vector3.zero;

    void Start() 
    {
        mBaseObject = this.GetBaseObject();

        if(MoveSpeed > 0)
        {
            CharacterPhysics phy = mBaseObject.CharacterPhy;
            mStartWorldPos = mBaseObject.transform.position;
            Vector3 offset = new Vector3(DestLocalIndexX, DestLocalIndexY, 0) * Consts.BlockSize;
            mEndWorldPos = mBaseObject.transform.position + offset;
            mVelocity = mEndWorldPos - mStartWorldPos;
            mDuration = mVelocity.magnitude / MoveSpeed;
            mVelocity.Normalize();
            mVelocity *= MoveSpeed;

            StartCoroutine(CoMoving());
        }
    }

    IEnumerator CoMoving()
    {
        CharacterPhysics phy = mBaseObject.CharacterPhy;
        while(true)
        {
            phy.VelocityX = mVelocity.x;
            phy.VelocityY = mVelocity.y;
            yield return new WaitForSeconds(mDuration);

            phy.VelocityX = 0;
            phy.VelocityY = 0;
            yield return new WaitForSeconds(2.0f);
            mVelocity *= -1;
        }
    }

    
}
