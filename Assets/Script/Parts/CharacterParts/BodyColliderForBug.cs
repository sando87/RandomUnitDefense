using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IsGround 판단 기준이 캐릭터 기준(즉 캐릭터가 90도 회전하면 x축 방향으로 IsGround 유무를 판단)
// 벽에 붙어서 이동하는 일벌레 같은 경우 이런방식으로 isGround를 확인해야 한다.

public class BodyColliderForBug : BodyCollider
{
    protected override void UpdateState()
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
        Vector3 cornerFT = Collider.ForwardUp(1, ObstacleHeightRate);
        if (Physics.Raycast(new Ray(cornerFT - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            mObstacledColliders.Add(hit.collider);
        }
        Vector3 cornerFB = Collider.ForwardDown(1, ObstacleHeightRate);
        if (Physics.Raycast(new Ray(cornerFB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            mObstacledColliders.Add(hit.collider);
        }

        mGroundColliders.Clear();
        dir = -mBaseObject.transform.up;

        Vector3 cornerLB = Collider.BackDown(GroundWidthRate, 1);
        if (Physics.Raycast(new Ray(cornerLB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            if (Vector3.Dot(hit.normal, dir) < 0)
            {
                mGroundColliders.Add(hit.collider);
            }
        }
        if (Physics.Raycast(new Ray(cornerLB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Props))
        {
            ThinGround ground = hit.collider.GetComponent<ThinGround>();
            if (ground != null)
            {
                if (Vector3.Dot(hit.normal, dir) < 0)
                {
                    mGroundColliders.Add(hit.collider);
                }
            }
        }

        Vector3 cornerB = Collider.Down();
        if (Physics.Raycast(new Ray(cornerB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            if (Vector3.Dot(hit.normal, dir) < 0)
            {
                mGroundColliders.Add(hit.collider);
            }
        }
        if (Physics.Raycast(new Ray(cornerB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Props))
        {
            ThinGround ground = hit.collider.GetComponent<ThinGround>();
            if (ground != null)
            {
                if (Vector3.Dot(hit.normal, dir) < 0)
                {
                    mGroundColliders.Add(hit.collider);
                }
            }
        }

        Vector3 cornerRB = Collider.ForwardDown(GroundWidthRate, 1);
        if (Physics.Raycast(new Ray(cornerRB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Platforms))
        {
            if (Vector3.Dot(hit.normal, dir) < 0)
            {
                mGroundColliders.Add(hit.collider);
            }
        }
        if (Physics.Raycast(new Ray(cornerRB - (dir * Thickness * 0.5f), dir), out hit, Thickness, 1 << LayerID.Props))
        {
            ThinGround ground = hit.collider.GetComponent<ThinGround>();
            if (ground != null)
            {
                if (Vector3.Dot(hit.normal, dir) < 0)
                {
                    mGroundColliders.Add(hit.collider);
                }
            }
        }
    }
}
