using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePosition : MonoBehaviour
{
    public void MovePosition(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}

