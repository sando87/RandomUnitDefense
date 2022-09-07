using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자가 키보드나 조이스틱으로 입력한 함수를 그대로 Warpper하여 전달한다.
public class UserInput : CharacterInput
{
    [SerializeField] GameObject MoveIndicatorPrefab = null;

    public event System.Action EventSelected;
    public event System.Action EventDeSelected;
    public event System.Action<Vector3> EventMove;

    private BaseObject mBaseObject = null;
    private GameObject mIndicator = null;

    void Awake()
    {
        mBaseObject = this.GetBaseObject();
    }

    public void OnSelect()
    {
        if(Lock) return;
        EventSelected?.Invoke();
    }
    public void OnDeSelect()
    {
        if (Lock) return;
        EventDeSelected?.Invoke();
    }
    public void OnMove(Vector3 destination)
    {
        if (Lock) return;
        
        EventMove?.Invoke(destination);

        if (mIndicator != null)
        {
            Destroy(mIndicator);
            mIndicator = null;
        }
    }
    public void OnDrawMoveIndicator(Vector3 destination)
    {
        if (Lock) return;
        if(mIndicator == null)
        {
            mIndicator = Instantiate(MoveIndicatorPrefab, mBaseObject.transform.position, Quaternion.identity);
        }
        Vector3 diff = destination - mBaseObject.transform.position;
        diff.z = 0;
        mIndicator.transform.localScale = new Vector3(diff.magnitude, 1, 1);
        mIndicator.transform.right = diff.normalized;
        mIndicator.transform.position = mBaseObject.transform.position;
    }
    void OnDisable()
    {
        if (mIndicator != null)
        {
            Destroy(mIndicator);
            mIndicator = null;
        }
    }
}
