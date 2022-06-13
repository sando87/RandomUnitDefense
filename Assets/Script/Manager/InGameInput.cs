using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameInput : MonoBehaviour
{
    private Camera mWorldCam = null;
    private UserInput mDownObject = null;
    private Vector3 mDownPosition = Vector3.zero;
    private UserInput mSelectedObject = null;

    public bool Lock { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        mWorldCam = Camera.main;
        InputWrapper.Instance.EventDownTriggered += OnDownTriggered;
        InputWrapper.Instance.EventUpTriggered += OnUpTriggered;
    }

    private void OnDownTriggered(InputType obj)
    {
        if(Lock || IsOverUI())
            return;

        mDownObject = null;
        Vector3 worldPt = Camera.main.ScreenToWorldPoint(InputWrapper.Instance.MousePosition());
        RaycastHit[] hits = MyUtils.RaycastAllFromTo(mWorldCam.transform.position, worldPt, 1 << LayerID.Player);
        foreach(RaycastHit hit in hits)
        {
            UserInput userInput = hit.collider.GetComponentInBaseObject<UserInput>();
            if(userInput != null)
            {
                mDownObject = userInput;
                break;
            }
        }

        mDownPosition = worldPt;
    }
    private void OnUpTriggered(InputType obj)
    {
        Vector3 worldPt = Camera.main.ScreenToWorldPoint(InputWrapper.Instance.MousePosition());
        if ((worldPt - mDownPosition).magnitude < 0.1f)
        {
            SelecteObject(mDownObject);
        }
        else
        {
            if(mDownObject != null)
            {
                SelecteObject(null);
                mDownObject.OnMove(worldPt);
            }
        }

        mDownObject = null;
        mDownPosition = Vector3.zero;
    }

    private void SelecteObject(UserInput obj)
    {
        if (mSelectedObject != null)
        {
            mSelectedObject.OnDeSelect();
            mSelectedObject = null;
        }

        mSelectedObject = obj;
        if(mSelectedObject != null)
        {
            mSelectedObject.OnSelect();
        }
    }
    private bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
