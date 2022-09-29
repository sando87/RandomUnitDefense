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

public class InGameInput : SingletonMono<InGameInput>
{
    [SerializeField] float _LongPressDurtaion = 1.0f;

    private Camera mWorldCam = null;
    private bool mIsDownNow = false;
    private bool mIsDragging = false;
    private BaseObject mDownObject = null;
    private Vector3 mWorldDownPosition = Vector3.zero;

    public bool Lock { get; set; } = false;
    public event System.Action<BaseObject[]> EventSelectUnits;
    public event System.Action EventDeSelectUnits;
    public event System.Action<Rect> EventDrawSelectArea;
    public event System.Action<BaseObject, Vector3> EventMove;

    protected override void Awake()
    {
        base.Awake();
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
            1 << LayerID.Player | 1 << LayerID.Enemies | 1 << LayerID.ThemeBackground, 
            out RaycastHit hit))
        {
            if(hit.collider.gameObject.layer == LayerID.Player)
            {
                mDownObject = hit.collider.GetBaseObject();
                mWorldDownPosition = hit.point;

                StopAllCoroutines();
                StartCoroutine(CoDragging());
                StartCoroutine(CheckLongPress());
            }
            else if(hit.collider.gameObject.layer == LayerID.Enemies)
            {
                mDownObject = hit.collider.GetBaseObject();
                mWorldDownPosition = hit.point;

                StopAllCoroutines();
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
        if(!mIsDownNow) return;

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
        else if(mDownObject != null)
        {
            if(MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.Player | 1 << LayerID.Enemies, out RaycastHit hit))
            {
                BaseObject upObject = hit.collider.GetBaseObject();
                if (upObject == mDownObject)
                {
                    DeSelecteAll();
                    SelecteObject(mDownObject);
                }
            }
        }

        ResetAllState();
    }


    IEnumerator CoDragging()
    {
        mIsDragging = false;
        Vector2 pressedPos = InputWrapper.Instance.MousePosition();
        yield return new WaitUntil(() => IsMovedFromPosition(pressedPos));

        UserInput downObjRecv = (mDownObject != null) ? mDownObject.GetComponentInChildren<UserInput>() : null;

        while (mIsDownNow)
        {
            mIsDragging = true;
            if(mDownObject != null)
            {
                MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit);
                if(downObjRecv != null)
                    downObjRecv.OnDrawMoveIndicator(hit.point);
            }
            else
            {
                MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit);
                Vector3 worldUpPosition = hit.point;
                Vector2 center = (mWorldDownPosition + worldUpPosition) * 0.5f;
                Vector2 size = mWorldDownPosition - worldUpPosition;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                Rect worldArea = new Rect();
                worldArea.size = size;
                worldArea.center = center;

                DrawSelectArea(worldArea);
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

        if(MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.Player, out RaycastHit hit))
        {
            if(hit.collider.GetBaseObject() == mDownObject)
            {
                OnLongPress(mDownObject);
            }
        }

        ResetAllState();
    }
    private void OnLongPress(BaseObject target)
    {
        SelectAllSameUnit(target);
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
    private bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void SelectAllUnitsInArea(Rect worldArea)
    {
        List<BaseObject> selectedUnits = new List<BaseObject>();
        Collider[] cols = Physics.OverlapBox(worldArea.center, worldArea.size * 0.5f, Quaternion.identity, 1 << LayerID.Player);
        foreach(Collider col in cols)
        {
            selectedUnits.Add(col.GetBaseObject());
        }

        if(selectedUnits.Count > 0)
        {
            EventSelectUnits?.Invoke(selectedUnits.ToArray());
        }
        else
        {
            DeSelecteAll();
        }
    }
    private void SelecteObject(BaseObject obj)
    {
        EventSelectUnits?.Invoke(new BaseObject[] { obj });
    }
    private void SelectAllSameUnit(BaseObject obj)
    {
        DeSelecteAll();
        List<BaseObject> units = InGameSystem.Instance.DetectSameUnit(obj);
        EventSelectUnits?.Invoke(units.ToArray());
    }
    private void MoveUnit(BaseObject obj, Vector3 dest)
    {
        EventMove?.Invoke(obj, dest);
    }
    private void DeSelecteAll()
    {
        EventDeSelectUnits?.Invoke();
    }
    private void DrawSelectArea(Rect worldArea)
    {
        EventDrawSelectArea?.Invoke(worldArea);
    }
}
