using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    BaseObject mBaseObj = null;

    void Awake()
    {
        mBaseObj = this.GetBaseObject();
    }

    public void ApplyBuff(BuffBase buffPrefab, bool isOverlappable = false)
    {
        if(!isOverlappable)
        {
            BuffBase sameBuff = FindBuff(buffPrefab.BuffID);
            if (sameBuff != null)
            {
                sameBuff.ResetDuration(); //동일한 버프가 있을 경우에는 지속지간 갱신만. => 중복 불가...
                return;
            }    
        }

        BuffBase buff = Instantiate(buffPrefab, transform);
        buff.gameObject.SetActive(true);
        buff.transform.localPosition = Vector3.zero;
    }

    public BuffBase FindBuff(long buffID)
    {
        foreach (Transform child in transform)
        {
            BuffBase buff = child.GetComponent<BuffBase>();
            if (buff != null && buff.BuffID == buffID)
                return buff;
        }
        return null;
    }

}

