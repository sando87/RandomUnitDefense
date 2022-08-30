using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 유닛 선택
// 유닛 이동
// 같은 유닛 모두 선택
// 영역 안의 모든 유닛 선택
// 선택 모두 취소

public class InGameInput : MonoBehaviour
{
    [SerializeField] float _LongPressDurtaion = 1.0f;

    private Camera mWorldCam = null;
    private bool mIsDownNow = false;
    private bool mIsDragging = false;
    private UserInput mDownObject = null;
    private Vector3 mWorldDownPosition = Vector3.zero;
    private UserInput mSelectedObject = null;

    public bool Lock { get; set; } = false;

    void Awake()
    {
        mWorldCam = Camera.main;
    }
    void Start()
    {
        InputWrapper.Instance.EventDownTriggered += OnDownTriggered;
        InputWrapper.Instance.EventUpTriggered += OnUpTriggered;
    }

    void ResetAllState()
    {
        StopAllCoroutines();
        mIsDownNow = false;
        mIsDragging = false;
        mDownObject = null;
        mWorldDownPosition = Vector3.zero;
    }

    private void OnDownTriggered(InputType obj)
    {
        if(Lock || IsOverUI())
            return;

        mIsDownNow = true;
        if(MyUtils.RaycastScreenToWorld(
            mWorldCam, 
            InputWrapper.Instance.MousePosition(), 
            1 << LayerID.Player | 1 << LayerID.ThemeBackground, 
            out RaycastHit hit))
        {
            if(hit.collider.gameObject.layer == LayerID.Player)
            {
                DeSelecteAll();
                mDownObject = hit.collider.GetComponentInBaseObject<UserInput>();
                mWorldDownPosition = hit.point;
                SelecteObject(mDownObject);

                StopAllCoroutines();
                StartCoroutine(CoDragging());
                StartCoroutine(CheckLongPress());
            }
            else
            {
                DeSelecteAll();
                mDownObject = null;
                mWorldDownPosition = hit.point;
                StopAllCoroutines();
                StartCoroutine(CoDragging());
            }
        }
    }
    private void OnUpTriggered(InputType obj)
    {
        if(mIsDragging)
        {
            MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit);
            Vector3 worldUpPosition = hit.point;

            if(mDownObject != null)
            {
                MoveUnit(mDownObject, worldUpPosition);
            }
            else
            {
                Vector2 center = (mWorldDownPosition + worldUpPosition) * 0.5f;
                Vector2 size = mWorldDownPosition - worldUpPosition;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                Rect worldArea = new Rect();
                worldArea.size = size;
                worldArea.center = center;
                SelectAllUnitsInArea(worldArea);
            }
        }

        ResetAllState();
    }


    IEnumerator CoDragging()
    {
        mIsDragging = false;
        Vector2 pressedPos = InputWrapper.Instance.MousePosition();
        yield return new WaitUntil(() => IsMovedFromPosition(pressedPos));

        while (mIsDownNow)
        {
            mIsDragging = true;
            if(mDownObject != null)
            {
                MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit);
                DrawDestination(mDownObject, hit.point);
            }
            else
            {
                Vector2 curScreenPos = InputWrapper.Instance.MousePosition();
                DrawSelectArea(pressedPos, curScreenPos);
            }
        
            yield return null;
        }
        mIsDragging = false;
    }
    IEnumerator CheckLongPress()
    {
        float time = 0;

        Vector2 pressedPos = InputWrapper.Instance.MousePosition();
        while (time < _LongPressDurtaion)
        {
            yield return null;

            if (!mIsDownNow || IsMovedFromPosition(pressedPos))
            {
                yield break;
            }

            time += Time.deltaTime;
        }

        OnLongPress();
        ResetAllState();
    }
    private void OnLongPress()
    {
        SelectAllSameUnit(mDownObject);
    }

    bool IsMovedFromPosition(Vector2 downScreenPos)
    {
        Vector2 delta = InputWrapper.Instance.MousePosition() - downScreenPos;
        if (Mathf.Abs(delta.x) > Screen.width * 0.01f
            || Mathf.Abs(delta.y) > Screen.height * 0.01f)
        {
            return true;
        }
        return false;
    }


    private void SelectAllUnitsInArea(Rect worldArea)
    {
        LOG.trace(worldArea);
    }
    private void SelecteObject(UserInput obj)
    {
        LOG.trace(obj.GetBaseObject().gameObject.name);
        mSelectedObject = obj;
        mSelectedObject.OnSelect();
    }
    private void SelectAllSameUnit(UserInput obj)
    {
        LOG.trace(obj.GetBaseObject().gameObject.name);
    }
    private void MoveUnit(UserInput obj, Vector3 dest)
    {
        LOG.trace(dest);
        mSelectedObject.OnMove(dest);
    }
    private void DeSelecteAll()
    {
        LOG.trace();
        if (mSelectedObject != null)
        {
            mSelectedObject.OnDeSelect();
            mSelectedObject = null;
        }
    }
    private void DrawDestination(UserInput obj, Vector3 worldDest)
    {
        LOG.trace();
    }
    private void DrawSelectArea(Vector2 startScreenPos, Vector2 endScreenPos)
    {
        LOG.trace();
    }
    private bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
