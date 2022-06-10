using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterPhysics : MonoBehaviour, IMapEditorObject
{
    [Range(0, 1)][SerializeField] float GravityMod = 1.0f;
    [SerializeField] int OffPixelFromGround = 1;
    [SerializeField] bool _GravityLock = false;

    public bool GravityLock { get { return _GravityLock; } set { _GravityLock = value; } }
    public bool CollisionLock { get; set; } = false;
    public float VelocityX { set { mVelocity.x = value; } }
    public float VelocityY { set { mVelocity.y = value; } }

    public Vector3 Velocity { get { return mVelocity; } }
    public float Gravity { get { return Consts.GalobalGravity * GravityMod; } }
    public float OffsetFromGround { get { return (float)OffPixelFromGround / Consts.PixelPerUnit; } }
    public event Action<Vector3> EventCrushed = null; // 인자로 충돌당시 속도 전달

    private BaseObject mBaseObject = null;
    private BodyCollider mBody = null;
    private Vector3 mVelocity = Vector3.zero;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        mBody = mBaseObject.Body;
    }

    void Update()
    {
        UpdateNextPosition();

        // float vel = curVel.magnitude;
        // Vector2 dir = -curVel.normalized;
        // totalForce += AirResistCoeff * vel * vel * dir ; // 공기저항(현재속도제곱에 비례) 반영
    }

    // bool mmIsHit = false;
    // Vector3 mmCenter;
    // Vector3 mmDir;
    // Vector3 mmExt;
    // float mmDist = 0;
    // Vector3 mmHitPoint;
    // void OnDrawGizmos()
    // {
    //     if(mBaseObject.gameObject.layer != LayerID.Player)
    //         return;
    //     Gizmos.color = Color.red;
    //     if (mmIsHit)
    //     {
    //         Gizmos.DrawRay(mmCenter, mmDir * mmDist);
    //         Gizmos.DrawWireCube(mmCenter + mmDir * mmDist, mmExt * 2);
    //         Gizmos.DrawWireSphere(mmHitPoint, 0.1f);
    //     }
    //     else
    //     {
    //         Gizmos.DrawRay(mmCenter, mmDir * mmDist);
    //     }
    // }

    private void UpdateNextPosition()
    {
        // 중력으로 인한 위치 조정
        if (!mBody.IsGrounded && !GravityLock)
            mVelocity += new Vector3(0, Gravity, 0) * Time.deltaTime;

        if (mVelocity.sqrMagnitude == 0)
            return;

        // 속도에 따른 다음 위치 조정
        Vector3 diff = Velocity * Time.deltaTime;
        Vector3 newDiff = diff;

        if (!CollisionLock)
        {
            // 주변 벽이나 지형에 부딪히면 그에 따른 위치 재조정
            float dist = diff.magnitude;
            Vector3 dir = diff.normalized;

            Bounds curBounds = mBody.Collider.GetWorldBounds2D();
            RaycastHit hit = new RaycastHit();
            int layerMask = 1 << LayerID.Platforms;
            //int layerMask = (1 << LayerID.Platforms | 1 << LayerID.Props);
            float thickness = 0;
            float minHitDist = float.MaxValue;
            bool IsCrushed = false;
            Vector3 previousVelocity = Velocity;

            Vector3 cornerLB = new Vector3(curBounds.min.x + OffsetFromGround, curBounds.min.y + OffsetFromGround, curBounds.center.z);
            Vector3 cornerRB = new Vector3(curBounds.max.x - OffsetFromGround, curBounds.min.y + OffsetFromGround, curBounds.center.z);
            Vector3 cornerLT = new Vector3(curBounds.min.x + OffsetFromGround, curBounds.max.y - OffsetFromGround, curBounds.center.z);
            Vector3 cornerRT = new Vector3(curBounds.max.x - OffsetFromGround, curBounds.max.y - OffsetFromGround, curBounds.center.z);

            // Vector3 extents = mBody.Collider.bounds.extents;
            // if(Physics.BoxCast(mBody.Center, extents, mBaseObject.transform.up, out hit, Quaternion.identity, 5, layerMask))
            // {
            //     mmIsHit = true;

            //     IsCrushed = true;
            //     EventCrushed?.Invoke(previousVelocity);

            //     if (hit.normal.x != 0) mVelocity.x = 0;
            //     if (hit.normal.y != 0) mVelocity.y = 0;

            //     float newCenterX = hit.point.x + hit.normal.x * extents.x;
            //     float newCenterY = hit.point.y + hit.normal.y * extents.y;
            //     SetBodyCenterPosition(new Vector3(newCenterX, newCenterY, 0));
            // }

            if (Physics.Raycast(new Ray(cornerLB - (dir * thickness), dir), out hit, dist + thickness, layerMask))
            {
                float hitDist = (cornerLB - hit.point).sqrMagnitude;
                if (hitDist < minHitDist)
                {
                    IsCrushed = true;
                    minHitDist = hitDist;
                    newDiff = hit.point - cornerLB + (hit.normal * OffsetFromGround);

                    if (hit.normal.x != 0) mVelocity.x = 0;
                    if (hit.normal.y != 0) mVelocity.y = 0;
                }
            }
            if (Physics.Raycast(new Ray(cornerRB - (dir * thickness), dir), out hit, dist + thickness, layerMask))
            {
                float hitDist = (cornerRB - hit.point).sqrMagnitude;
                if (hitDist < minHitDist)
                {
                    IsCrushed = true;
                    minHitDist = hitDist;
                    newDiff = hit.point - cornerRB + (hit.normal * OffsetFromGround);

                    if (hit.normal.x != 0) mVelocity.x = 0;
                    if (hit.normal.y != 0) mVelocity.y = 0;
                }
            }
            if (Physics.Raycast(new Ray(cornerLT - (dir * thickness), dir), out hit, dist + thickness, layerMask))
            {
                float hitDist = (cornerLT - hit.point).sqrMagnitude;
                if (hitDist < minHitDist)
                {
                    IsCrushed = true;
                    minHitDist = hitDist;
                    newDiff = hit.point - cornerLT + (hit.normal * OffsetFromGround);

                    if (hit.normal.x != 0) mVelocity.x = 0;
                    if (hit.normal.y != 0) mVelocity.y = 0;
                }
            }
            if (Physics.Raycast(new Ray(cornerRT - (dir * thickness), dir), out hit, dist + thickness, layerMask))
            {
                float hitDist = (cornerRT - hit.point).sqrMagnitude;
                if (hitDist < minHitDist)
                {
                    IsCrushed = true;
                    minHitDist = hitDist;
                    newDiff = hit.point - cornerRT + (hit.normal * OffsetFromGround);

                    if (hit.normal.x != 0) mVelocity.x = 0;
                    if (hit.normal.y != 0) mVelocity.y = 0;
                }
            }


            // 떨어질수 있는 얇은 블럭 검사
            if (Physics.Raycast(new Ray(cornerLB - (dir * thickness), dir), out hit, dist + thickness, 1 << LayerID.Props))
            {
                if (hit.normal.y > 0)
                {
                    ThinGround ground = hit.collider.GetComponent<ThinGround>();
                    if (ground != null)
                    {
                        IsCrushed = true;
                        newDiff = hit.point - cornerLB + (hit.normal * OffsetFromGround);
                        mVelocity = Vector3.zero;
                    }
                }
            }
            else if (Physics.Raycast(new Ray(cornerRB - (dir * thickness), dir), out hit, dist + thickness, 1 << LayerID.Props))
            {
                if (hit.normal.y > 0)
                {
                    ThinGround ground = hit.collider.GetComponent<ThinGround>();
                    if (ground != null)
                    {
                        IsCrushed = true;
                        newDiff = hit.point - cornerRB + (hit.normal * OffsetFromGround);
                        mVelocity = Vector3.zero;
                    }
                }
            }

            if(IsCrushed)
            {
                EventCrushed?.Invoke(previousVelocity);
            }
        }

        mBaseObject.transform.position += newDiff;
        //mBaseObject.transform.Snap();
    }


    public void AviodOverlap()
    {
        Vector3 adjustedCenter = mBody.Center;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(mBody.Center, Vector3.right, out hit, mBody.Size.x * 0.5f, 1 << LayerID.Platforms))
        {
            adjustedCenter.x = hit.point.x - mBody.Size.x * 0.5f - 0.1f;
        }
        if (Physics.Raycast(mBody.Center, Vector3.left, out hit, mBody.Size.x * 0.5f, 1 << LayerID.Platforms))
        {
            adjustedCenter.x = hit.point.x + mBody.Size.x * 0.5f + 0.1f;
        }
        if (Physics.Raycast(mBody.Center, Vector3.up, out hit, mBody.Size.y * 0.5f, 1 << LayerID.Platforms))
        {
            adjustedCenter.y = hit.point.y - mBody.Size.y * 0.5f - 0.1f;
        }
        if (Physics.Raycast(mBody.Center, Vector3.down, out hit, mBody.Size.y * 0.5f, 1 << LayerID.Platforms))
        {
            adjustedCenter.y = hit.point.y + mBody.Size.y * 0.5f + 0.1f;
        }

        SetBodyCenterPosition(adjustedCenter);
    }

    public void SetBodyCenterPosition(Vector3 bodyCenterPos)
    {
        mBaseObject.transform.position = bodyCenterPos - mBody.OffsetFromBaseObjct;
    }

    public void OnInitMapEditor()
    {
        enabled = false;
    }
}
