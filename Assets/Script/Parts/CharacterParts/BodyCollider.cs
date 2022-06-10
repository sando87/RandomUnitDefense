using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IsGround 판단 기준이 World 기준(즉 오브젝트가 90도 회전해도 무조건 y축 방향으로 IsGround 유무를 판단)

public class BodyCollider : MonoBehaviour
{
    [SerializeField] BoxCollider TopOfTheHead = null;

    protected const float Thickness = Consts.DistPerPixel * 2.0f * 1.1f;
    protected const float GroundWidthRate = 0.9f;
    protected const float ObstacleHeightRate = 0.8f;

    protected BaseObject mBaseObject = null;
    protected BoxCollider mBodyCollider = null;
    protected Vector3Int mLastTransState = Vector3Int.zero;
    protected int mLastFrameCount = 0;
    protected Vector3 mOriCenter = Vector3.zero;
    protected Vector3 mOriSize = Vector3.zero;
    protected List<Collider> mGroundColliders = new List<Collider>();
    protected List<Collider> mObstacledColliders = new List<Collider>();

    public Collider[] GroundColliders { get { return mGroundColliders.ToArray(); } }
    public Collider[] ObstacledColliders { get { return mObstacledColliders.ToArray(); } }

    public bool IsGrounded 
    { 
        get
        {
            if(IsPositionChanged())
                UpdateState();
            return mGroundColliders != null && mGroundColliders.Count > 0;
        }
    }

    public bool IsObstacled
    {
        get
        {
            if (IsPositionChanged())
                UpdateState();
            return mObstacledColliders != null && mObstacledColliders.Count > 0;
        }
    }

    public bool IsCrushedHead
    {
        get
        {
            if(TopOfTheHead != null)
            {
                return Physics.OverlapBox(TopOfTheHead.bounds.center, TopOfTheHead.bounds.extents, Quaternion.identity, 1 << LayerID.Platforms).Length > 0;
            }
            return false;
        }
    }

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
        UpdateState();
    }

    private bool IsPositionChanged()
    {
        Vector3Int curPos = MyUtils.ToSnapIndex(mBaseObject.transform.position);
        curPos.z = (int)mBaseObject.transform.right.x;
        return mLastTransState.x != curPos.x || mLastTransState.y != curPos.y || mLastTransState.z != curPos.z || mLastFrameCount != Time.frameCount;
    }

    protected virtual void UpdateState()
    {
        mLastTransState = MyUtils.ToSnapIndex(mBaseObject.transform.position);
        mLastTransState.z = (int)mBaseObject.transform.right.x;
        mLastFrameCount = Time.frameCount;

        Bounds curBounds = Collider.GetWorldBounds2D();
        int layerMask = 1 << LayerID.Platforms;
        Vector3 dir = mBaseObject.transform.right;
        RaycastHit hit;

        mObstacledColliders.Clear();
        // Collider[] obsCols = Physics.OverlapBox(Collider.Forward(), new Vector3(BodyCollider.Thickness * 0.5f, curBounds.extents.y * ObstacleHeightRate, 0), Quaternion.identity, layerMask);
        // mObstacledColliders.AddRange(obsCols);
        Vector3 cornerF = Collider.Forward();
        if (Physics.Raycast(new Ray(cornerF - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            mObstacledColliders.Add(hit.collider);
        }
        Vector3 cornerFT = cornerF + new Vector3(0, curBounds.extents.y * ObstacleHeightRate, 0);
        if (Physics.Raycast(new Ray(cornerFT - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            mObstacledColliders.Add(hit.collider);
        }
        Vector3 cornerFB = cornerF + new Vector3(0, -curBounds.extents.y * ObstacleHeightRate, 0);
        if (Physics.Raycast(new Ray(cornerFB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            mObstacledColliders.Add(hit.collider);
        }


        mGroundColliders.Clear();
        dir = new Vector3(0, -1, 0);

        Vector3 cornerLB = curBounds.center + new Vector3(-curBounds.extents.x * GroundWidthRate, -curBounds.extents.y, 0);
        if (Physics.Raycast(new Ray(cornerLB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            if (hit.normal.y > 0)
            {
                mGroundColliders.Add(hit.collider);
            }
        }
        if (Physics.Raycast(new Ray(cornerLB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Props))
        {
            ThinGround ground = hit.collider.GetComponent<ThinGround>();
            if (ground != null)
            {
                if (hit.normal.y > 0)
                {
                    mGroundColliders.Add(hit.collider);
                }
            }
        }

        Vector3 cornerB = curBounds.center + new Vector3(0, -curBounds.extents.y, 0);
        if (Physics.Raycast(new Ray(cornerB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            if (hit.normal.y > 0)
            {
                mGroundColliders.Add(hit.collider);
            }
        }
        if (Physics.Raycast(new Ray(cornerB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Props))
        {
            ThinGround ground = hit.collider.GetComponent<ThinGround>();
            if (ground != null)
            {
                if (hit.normal.y > 0)
                {
                    mGroundColliders.Add(hit.collider);
                }
            }
        }

        Vector3 cornerRB = curBounds.center + new Vector3(curBounds.extents.x * GroundWidthRate, -curBounds.extents.y, 0);
        if (Physics.Raycast(new Ray(cornerRB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            if (hit.normal.y > 0)
            {
                mGroundColliders.Add(hit.collider);
            }
        }
        if (Physics.Raycast(new Ray(cornerRB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Props))
        {
            ThinGround ground = hit.collider.GetComponent<ThinGround>();
            if (ground != null)
            {
                if (hit.normal.y > 0)
                {
                    mGroundColliders.Add(hit.collider);
                }
            }
        }
    }

    public void AdjustHeight(float rate)
    {
        mBodyCollider.center = new Vector3(mOriCenter.x, mOriCenter.y * rate, mOriCenter.z);
        mBodyCollider.size = new Vector3(mOriSize.x, mOriSize.y * rate, mOriSize.z);
    }
    public void AdjustWidth(float rate)
    {
        //mBodyCollider.center = new Vector3(mOriCenter.x, mOriCenter.y, mOriCenter.z);
        mBodyCollider.size = new Vector3(mOriSize.x * rate, mOriSize.y, mOriSize.z);
    }

    public ThinGround FindBottomThinGround()
    {
        RaycastHit hit = new RaycastHit();
        float heightHalf = Collider.size.y * 0.5f;
        Bounds bounds = Collider.GetWorldBounds2D();
        if (Physics.Raycast(new Ray(bounds.Left(0.95f), new Vector3(0, -1, 0)), out hit, heightHalf + Consts.DropDistance, 1 << LayerID.Props)
            || Physics.Raycast(new Ray(bounds.Right(0.95f), new Vector3(0, -1, 0)), out hit, heightHalf + Consts.DropDistance, 1 << LayerID.Props))
        {
            return hit.collider.GetComponent<ThinGround>();
        }

        return null;
    }

    public Moveable FindBottomMovable()
    {
        Bounds bounds = Collider.GetWorldBounds2D();
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(new Ray(bounds.Left(0.95f), new Vector3(0, -1, 0)), out hit, Consts.BlockSize, 1 << LayerID.Platforms)
            || Physics.Raycast(new Ray(bounds.Right(0.95f), new Vector3(0, -1, 0)), out hit, Consts.BlockSize, 1 << LayerID.Platforms))
        {
            return hit.collider.GetComponentInBaseObject<Moveable>();
        }
        return null;
    }
}
