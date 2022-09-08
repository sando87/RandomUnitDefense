using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase : MonoBehaviour
{
    [SerializeField][Identifier] long _BuffID = 0;
    [SerializeField] float Duration = 1;
    [SerializeField] BuffProperty BuffProp;

    public long BuffID { get { return _BuffID; } }

    private BaseObject mBaseObject = null;
    private BuffProperty mUnitBuffProp = null;
    //private float mElapsedTime = 0;

    void Awake()
    {
        mBaseObject = this.GetBaseObject();
        mUnitBuffProp = mBaseObject.BuffProp;
    }

    void Start()
    {
        if(Duration > 0)
            StartCoroutine(BuffDestroier());
    }

    void OnEnable()
    {
        BuffON();
    }

    void OnDisable()
    {
        BuffOFF();
    }

    void BuffON()
    {
        mUnitBuffProp.AddBuffProp(BuffProp);
    }
    void BuffOFF()
    {
        mUnitBuffProp.RemoveBuffProp(BuffProp);
    }

    IEnumerator BuffDestroier()
    {
        yield return new WaitForSeconds(Duration);
        Destroy(gameObject);
    }

    public void ResetDuration()
    {
        if (Duration > 0)
        {
            StopAllCoroutines();
            StartCoroutine(BuffDestroier());
        }
    }

}
