using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicGun : MonoBehaviour
{
    [SerializeField] private ParticleSystem FireEffect = null;
    [SerializeField] private ParticleSystem HitEffect = null;
    [SerializeField] private float MoveSpeed = 0;
    [SerializeField] private float SplashRange = 0;

    public Vector3 Destination { get; set; }
    public Action<UnitMob[]> EventHit { get; set; }

    // Start is called before the first frame update
    public void Launch()
    {
        FireEffect.gameObject.SetActive(true);
        StartCoroutine(MoveToDestination());
    }

    private IEnumerator MoveToDestination()
    {
        Vector3 dir = Destination - transform.position;
        dir.z = 0;
        dir.Normalize();
        while (true)
        {
            Vector3 nextPos = transform.position + (dir * MoveSpeed * Time.deltaTime);
            Vector3 nextDir = Destination - nextPos;
            nextDir.z = 0;
            if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
            {
                transform.position = Destination;
                break;
            }
            else
            {
                transform.position = nextPos;
            }
            yield return null;
        }

        UnitMob[] units = RGame.Get<RGameSystemManager>().DetectAroundUnit<UnitMob>(Destination, SplashRange);
        if(units != null && units.Length > 0)
            EventHit?.Invoke(units);

        HitEffect.gameObject.SetActive(true);
        Destroy(gameObject, 1.0f);
    }


}
