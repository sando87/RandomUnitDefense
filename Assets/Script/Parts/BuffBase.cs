using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase : MonoBehaviour
{
    [ContextMenu ("AssignBuffID")]
    void DoAssignBuffID() { _BuffID = DateTime.Now.Ticks; }

    [SerializeField][Identifier] long _BuffID = 0;
    [SerializeField] BuffProperty BuffProp;

    public long BuffID { get { return _BuffID; } }
    public float Duration { get; set; } = 1;

    protected BaseObject mBaseObject = null;
    protected BuffProperty mUnitBuffProp = null;
    private float mElapsedTime = 0;

    void Awake()
    {
        mBaseObject = this.GetBaseObject();
        mUnitBuffProp = mBaseObject.BuffProp;
    }

    void Update()
    {
        mElapsedTime += Time.deltaTime;
        if(Duration < mElapsedTime)
            Destroy(gameObject);
    }

    void OnEnable()
    {
        BuffON();
    }

    void OnDisable()
    {
        BuffOFF();
    }

    protected virtual void BuffON()
    {
        mUnitBuffProp.AddBuffProp(BuffProp);
    }
    protected virtual void BuffOFF()
    {
        mUnitBuffProp.RemoveBuffProp(BuffProp);
    }

    public void ResetDuration()
    {
        mElapsedTime = 0;
    }

}
