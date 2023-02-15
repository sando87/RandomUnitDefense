using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 유닛 클릭
// 유닛 롱클릭
// 드래깅 시작, 중, 끝

public class InGameInput : SingletonMono<InGameInput>
{
    [SerializeField] float _LongPressDurtaion = 1.0f;
    [SerializeField] float _DoubleClickInterval = 0.5f;

    private Camera mWorldCam = null;
    private bool mIsDownNow = false;
    private bool mIsDragging = false;
    private long mFirstClickTick = 0;
    private BaseObject mDownObject = null;
    private Vector3 mWorldDownPosition = Vector3.zero;

    public bool Lock { get; set; } = false;
    public event System.Action<BaseObject> EventClick;
    public event System.Action<BaseObject> EventDoubleClick;
    public event System.Action<Vector3> EventLongClick;
    public event System.Action<Vector3> EventDragStart;
    public event System.Action<Vector3> EventDragging;
    public event System.Action<Vector3> EventDragEnd;
    public BaseObject DownObject { get { return mDownObject; } }
    public Vector3 DownWorldPos { get { return mWorldDownPosition; } }

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
        if(Lock || InputWrapper.Instance.IsPointerOverUIObject())
            return;

        mIsDownNow = true;
        if(MyUtils.RaycastScreenToWorld(
            mWorldCam, 
            InputWrapper.Instance.MousePosition(), 
            1 << LayerID.Player | 1 << LayerID.Enemies | 1 << LayerID.ThemeBackground, 
            out RaycastHit hit))
        {
            if(hit.collider.gameObject.layer == LayerID.Player
                || hit.collider.gameObject.layer == LayerID.Enemies)
            {
                mDownObject = hit.collider.GetBaseObject();
                mWorldDownPosition = hit.point;

                StopAllCoroutines();
                StartCoroutine(CoDragging());
                StartCoroutine(CheckLongPress());
            }
            else
            {
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
            mIsDragging = false;
            MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit);
            Vector3 worldUpPosition = hit.point;
            EventDragEnd?.Invoke(worldUpPosition);
        }
        else
        {
            if(MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.Player | 1 << LayerID.Enemies | 1 << LayerID.ThemeBackground, out RaycastHit hit))
            {
                BaseObject upObject = hit.collider.GetBaseObject();
                if(MyUtils.IsCooltimeOver(mFirstClickTick, _DoubleClickInterval))
                {
                    EventClick?.Invoke(upObject == mDownObject ? mDownObject : null);
                    mFirstClickTick = DateTime.Now.Ticks;
                }
                else
                {
                    EventDoubleClick?.Invoke(upObject == mDownObject ? mDownObject : null);
                    mFirstClickTick = 0;
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

        mIsDragging = true;
        EventDragStart?.Invoke(mWorldDownPosition);

        while (mIsDownNow)
        {
            MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit);
            EventDragging?.Invoke(hit.point);
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

        if(MyUtils.RaycastScreenToWorld(mWorldCam, InputWrapper.Instance.MousePosition(), 1 << LayerID.ThemeBackground, out RaycastHit hit))
        {
            EventLongClick?.Invoke(hit.point);
        }

        ResetAllState();
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
}
