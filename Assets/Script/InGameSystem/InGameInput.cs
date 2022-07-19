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
        Ray ray = mWorldCam.ScreenPointToRay(InputWrapper.Instance.MousePosition());
        RaycastHit[] hits = Physics.RaycastAll(ray, 20, 1 << LayerID.Player);
        foreach(RaycastHit hit in hits)
        {
            UserInput userInput = hit.collider.GetComponentInBaseObject<UserInput>();
            if(userInput != null)
            {
                mDownObject = userInput;
                break;
            }
        }

        if(Physics.Raycast(ray, out RaycastHit hitOnBackground, 20, 1 << LayerID.ThemeBackground))
        {
            mDownPosition = hitOnBackground.point;
        }
    }
    private void OnUpTriggered(InputType obj)
    {
        Ray ray = mWorldCam.ScreenPointToRay(InputWrapper.Instance.MousePosition());
        Physics.Raycast(ray, out RaycastHit hitOnBackground, 20, 1 << LayerID.ThemeBackground);
        Vector3 worldPt = hitOnBackground.point;
        Vector3 diff = (worldPt - mDownPosition).ZeroZ();
        if (diff.magnitude < 0.1f)
        {
            Click();
        }
        else
        {
            if(mDownObject != null)
            {
                SelecteObject(null);
                mDownObject.OnMove(worldPt.ZeroZ());
            }
        }

        mDownObject = null;
        mDownPosition = Vector3.zero;
    }

    private void Click()
    {
        // 클릭시 먼저 WorldUI버튼을 클릭했는지 확인
        Ray ray = mWorldCam.ScreenPointToRay(InputWrapper.Instance.MousePosition());
        RaycastHit[] hits = Physics.RaycastAll(ray, 20, 1 << LayerID.WorldUI);
        foreach (RaycastHit hit in hits)
        {
            InGameButton worldUIBtn = hit.collider.GetComponent<InGameButton>();
            if (worldUIBtn != null)
            {
                worldUIBtn.EventClick?.Invoke();
                SelecteObject(null);
                return;
            }
        }

        // 그다음 캐릭터 클릭했는지 확인
        hits = Physics.RaycastAll(ray, 20, 1 << LayerID.Player);
        foreach (RaycastHit hit in hits)
        {
            UserInput unit = hit.collider.GetComponentInBaseObject<UserInput>();
            if (unit != null)
            {
                SelecteObject(unit);
                return;
            }
        }

        // 마지막으로 지면 클릭시 모두 해제
        SelecteObject(null);
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
