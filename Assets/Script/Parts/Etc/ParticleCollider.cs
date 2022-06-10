using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    private ParticleSystem mParticleSystem = null;
    private List<ParticleCollisionEvent> mCollisionEvents = new List<ParticleCollisionEvent>();

    // Start is called before the first frame update
    void Start()
    {
        mParticleSystem = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        // mCollisionEvents.Clear();
        // int n = ParticlePhysicsExtensions.GetCollisionEvents(mParticleSystem, other, mCollisionEvents);
        // for (int i = 0; i < n; ++i)
        // {
        //     LOG.trace(mCollisionEvents[i].colliderComponent.gameObject.name);
        // }
        // foreach (var ev in mCollisionEvents)
        // {
        //     LOG.trace(ev.colliderComponent == null);
        // }
    }

}
