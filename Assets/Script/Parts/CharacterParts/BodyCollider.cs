using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IsGround 판단 기준이 World 기준(즉 오브젝트가 90도 회전해도 무조건 y축 방향으로 IsGround 유무를 판단)

public class BodyCollider : MonoBehaviour
{
    protected BaseObject mBaseObject = null;
    protected BoxCollider mBodyCollider = null;
    protected Vector3 mOriCenter = Vector3.zero;
    protected Vector3 mOriSize = Vector3.zero;

    public BoxCollider Collider
    {
        get
        {
            if(mBodyCollider == null)
            {
                mBodyCollider = GetComponent<BoxCollider>();
            }
            return mBodyCollider;
        }
    }

    public Vector3 Center { get { return Collider.Center(); } }
    public Vector3 Size { get { return Collider.GetWorldBounds2D().size; } }
    public Vector3 Extents { get { return Size * 0.5f; } }
    public Vector3 Foot { get { return Collider.GetWorldBounds2D().Bottom(); } }
    public Vector3 OffsetFromBaseObjct { get { return Center - mBaseObject.transform.position; } }

    public bool Lock
    {
        get { return !Collider.enabled; }
        set { Collider.enabled = !value; }
    }

    private void Start() 
    {
        mBodyCollider = GetComponent<BoxCollider>();
        mOriCenter = mBodyCollider.center;
        mOriSize = mBodyCollider.size;
        mBaseObject = this.GetBaseObject();
    }

    public void AdjustHeight(float rate)
    {
        mBodyCollider.center = new Vector3(mOriCenter.x, mOriCenter.y * rate, mOriCenter.z);
        mBodyCollider.size = new Vector3(mOriSize.x, mOriSize.y * rate, mOriSize.z);
    }
    public void AdjustWidth(float rate)
    {
        mBodyCollider.size = new Vector3(mOriSize.x * rate, mOriSize.y, mOriSize.z);
    }
}
