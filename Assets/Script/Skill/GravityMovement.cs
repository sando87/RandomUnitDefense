using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GravityMovement : MonoBehaviour
{
    public Vector3 GravityAcc = Vector3.zero;
    public float Mass = 1;
    public float AirResistCoeff = 0.01f;
    public Vector3 ExtForce = Vector3.zero;
    public Vector3 Velocity = Vector3.zero;

    void Update()
    {
        float velSize = Velocity.magnitude;
        Vector3 airResistForce = (-1) * AirResistCoeff * velSize * velSize * Velocity.normalized;
        Vector3 totalForce = (ExtForce / Time.deltaTime) + (GravityAcc * Mass) + airResistForce;
        Vector3 curAcc = totalForce / Mass;

        Velocity += curAcc * Time.deltaTime;
        transform.position += Velocity * Time.deltaTime;

        ExtForce = Vector3.zero;
    }
}
