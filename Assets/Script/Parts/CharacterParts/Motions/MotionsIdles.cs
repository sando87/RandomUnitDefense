using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MotionsIdles : MotionBasic 
{
    public override void OnInitMotion()
    {
        base.OnInitMotion();
    }

    public override void OnEnterMotion()
    {
        base.OnEnterMotion();
    }

    public override void OnUpdateMotion()
    {
        base.OnUpdateMotion();

        if (mBaseObject.CharacterInput.VerticalMove < 0)
        {
            if (mBaseObject.CharacterPhy.Velocity.y == 0
            && mBaseObject.Body.IsGrounded
            && IsDroppable()
            && IsOnTheThinGround())
            {
                mBaseObject.transform.position -= new Vector3(0, Consts.DropDistance, 0);
            }
        }
    }

    public override void OnExitMotion()
    {
        base.OnExitMotion();
    }

    private bool IsDroppable()
    {
        Bounds bounds = mBaseObject.Body.Collider.GetWorldBounds2D();
        float heightHalf = bounds.extents.y;
        if (Physics.Raycast(new Ray(bounds.Left(0.95f), new Vector3(0, -1, 0)), heightHalf + Consts.DropDistance, 1 << LayerID.Platforms)
            || Physics.Raycast(new Ray(bounds.Right(0.95f), new Vector3(0, -1, 0)), heightHalf + Consts.DropDistance, 1 << LayerID.Platforms))
        {
            return false;
        }

        return true;
    }
    private bool IsOnTheThinGround()
    {
        ThinGround thinGround = mBaseObject.Body.FindBottomThinGround();
        if(thinGround != null)
        {
            return thinGround.Collider.size.y < Consts.DropDistance;
        }
        return true;
    }
}
