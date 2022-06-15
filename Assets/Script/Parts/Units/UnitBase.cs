using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    protected BaseObject mBaseObj = null;

    public long ResourceID { get; set; } = 0;

    protected virtual void Awake()
    {
        mBaseObj = this.GetBaseObject();
    }
}
