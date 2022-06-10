using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Crushable : MonoBehaviour
{
    BodyCollider mBody = null;
    CharacterPhysics mPhy = null;

    void Start() 
    {
        mBody = this.GetBaseObject().Body;
        mPhy = this.GetBaseObject().CharacterPhy;
    }

    void Update() 
    {
        if(mPhy.Velocity.y < 0)
        {
            Collider target = GetCrushedCharacter();
            if(target != null)
            {
                IDamagable health = target.GetDamagableObject();
                if(health != null)
                {
                    health.OnDamaged(100, this.GetBaseObject());
                }
            }
        }
    }

    private Collider GetCrushedCharacter()
    {
        Vector3 dir = mPhy.Velocity.normalized;
        float dist = mPhy.Velocity.magnitude * Time.deltaTime;
        int targetLayerMask = 1 << LayerID.Enemies | 1 << LayerID.Player;
        Bounds bounds = mBody.Collider.GetWorldBounds2D();

        RaycastHit hit = new RaycastHit();
        if(Physics.BoxCast(bounds.center, bounds.extents, dir, out hit, Quaternion.identity, dist, targetLayerMask))
        {
            if (hit.normal.y > 0.5f)
            {
                return hit.collider;
            }
        }
        return null;
    }

}
