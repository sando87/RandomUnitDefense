﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicGun : MonoBehaviour
{
    [SerializeField] private ParticleSystem FireEffect = null;
    [SerializeField] private ParticleSystem HitEffect = null;
    [SerializeField] private float MoveSpeed = 0;
    [SerializeField] private float SplashRange = 0;

    public Transform Target { get; set; }
    public Action<Vector3> EventHit { get; set; }

    // Start is called before the first frame update
    public void Launch()
    {
        FireEffect.gameObject.SetActive(true);
        transform.CoMoveToSpeed(Target, MoveSpeed, () =>
        {
            EventHit?.Invoke(transform.position);
            HitEffect.gameObject.SetActive(true);
            Destroy(gameObject, 1.0f);
        });
        
        //StartCoroutine(MoveToDestination());
    }

    private IEnumerator MoveToDestination()
    {
        Vector3 destination = Target.transform.position;
        Vector3 dir = destination - transform.position;
        dir.z = 0;
        dir.Normalize();
        while (true)
        {
            if(Target != null)
                destination = Target.transform.position;
                
            Vector3 nextPos = transform.position + (dir * MoveSpeed * Time.deltaTime);
            Vector3 nextDir = destination - nextPos;
            nextDir.z = 0;
            if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
            {
                transform.position = destination;
                break;
            }
            else
            {
                transform.position = nextPos;
            }
            yield return null;
        }

        EventHit?.Invoke(transform.position);

        HitEffect.gameObject.SetActive(true);
        Destroy(gameObject, 1.0f);
    }


}
