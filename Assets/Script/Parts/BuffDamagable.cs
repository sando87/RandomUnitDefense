using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDamagable : BuffBase
{
    public float Damage = 5;
    public float Interval = 0.5f;

    protected override void BuffON()
    {
        //base.BuffON();
        StartCoroutine(CoDamageValide());
        mBaseObject.Renderer.SetColor(Color.green);
    }
    protected override void BuffOFF()
    {
        //base.BuffOFF();
        StopAllCoroutines();
        mBaseObject.Renderer.SetColor(Color.white);
    }
    
    IEnumerator CoDamageValide()
    {
        while(true)
        {
            if(mBaseObject.Health != null)
            {
                mBaseObject.Health.GetDamaged(Damage, mBaseObject);
            }
            yield return new WaitForSeconds(Interval);
        }
    }
}
