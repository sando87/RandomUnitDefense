using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinGround : MonoBehaviour
{

    private BoxCollider mBodyCollider = null;
    public BoxCollider Collider
    {
        get
        {
            if (mBodyCollider == null)
            {
                mBodyCollider = GetComponent<BoxCollider>();
            }
            return mBodyCollider;
        }
    }

    void Start()
    {
    }

}
