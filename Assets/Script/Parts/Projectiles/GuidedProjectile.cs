using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GuidedProjectile : ProjectileBase
{
    public Transform Target = null;
    public float MoveVelocity = 1;
    public float RotateVelocity = 1;

    void Start()
    {
        StartCoroutine(MyUtils.CoRotateTowards2DLerp(transform, Target, RotateVelocity));
    }

    void Update()
    {
        transform.position += (transform.right * MoveVelocity * Time.deltaTime);
    }



}
