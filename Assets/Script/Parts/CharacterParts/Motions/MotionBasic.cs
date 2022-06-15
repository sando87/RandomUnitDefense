﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatorStateSelector
{
    // Inspector에서 변경하면 액션ID와 애니메이션클립이름까지 같이 세팅된다.(유니티 에디터에서 세팅해준 후 건드리지 않는 데이터)
    // StateSelectorAttribute.cs
    [StateSelector][SerializeField] public string StateName = ""; //인스펙터에서 세팅됨(런타임시는 readonly)
    [HideInInspector][SerializeField] public int AnimatorParamActionType = 0; //인스펙터에서 세팅됨(런타임시는 readonly)
    [HideInInspector][SerializeField] public string AnimationClipName = ""; //인스펙터에서 세팅됨(런타임시는 readonly)
    [HideInInspector][SerializeField] public string MotionName = ""; //인스펙터에서 세팅됨(런타임시는 readonly)
    [HideInInspector][SerializeField] public int LayerIndex = 0; //인스펙터에서 세팅됨(런타임시는 readonly)
    [HideInInspector][SerializeField] public float ClipLength = 0; //인스펙터에서 세팅됨(런타임시는 readonly)
}

public class MotionBasic : MonoBehaviour
{
    [SerializeField] AnimatorStateSelector AnimState = new AnimatorStateSelector();

    public int ID { get { return GetInstanceID(); } }
    public AnimActionID AnimActionType { get { return (AnimActionID)AnimState.AnimatorParamActionType; } }
    public int AnimatorLayerIndex { get { return AnimState.LayerIndex; } }

    private List<Tuple<float, Action>> EventAnims = new List<Tuple<float, Action>>();
    public float NormalizedTime { get; private set; } = 0;
    public float PlayTime { get { return AnimState.ClipLength * NormalizedTime; } }

    protected BaseObject mBaseObject = null;
    protected Animator mAnim = null;
    protected CharacterInput mCharacterInput = null;
    protected MotionManager mMotionManager = null;
    private float mCooltime = float.MaxValue;

    public virtual void OnInit()
    {
        mBaseObject = this.GetBaseObject();
        mAnim = mBaseObject.Animator;
        mCharacterInput = mBaseObject.CharacterInput;
        mMotionManager = mBaseObject.MotionManager;
    }
    public virtual bool OnReady()
    {
        return false;
    }
    public virtual void OnEnter()
    {
        SetTrigger(AnimActionType);
        StartCoroutine(OnCoUpdateMotion());
    }
    public virtual void OnUpdate()
    {
    }
    public virtual void OnLeave()
    {
        NormalizedTime = 0;
        EventAnims.Clear();
        StopAllCoroutines();
        CancelInvoke();
    }

    public bool IsCurrent<T>() where T : MotionBasic
    {
        return mMotionManager.IsCurrentMotion<T>();
    }
    public bool IsCurrentThis()
    {
        return mMotionManager.CurrentMotionID == ID;
    }
    public void SwitchMotion<T>() where T : MotionBasic
    {
        mMotionManager.SwitchMotion<T>();
    }
    public void SwitchMotionToThis()
    {
        mMotionManager.SwitchMotion(ID);
    }
    public void SwitchMotionToIdle()
    {
        mMotionManager.SwitchMotion<MotionsIdles>();
    }
    IEnumerator OnCoUpdateMotion()
    {
        float startNormalizedTime = mAnim.GetCurrentAnimatorStateInfo(AnimatorLayerIndex).normalizedTime;

        // Animator에 트리거 주어 동작 변경시 바로 적용되지 않으므로 실제 애니메이션이 변경될때까지 대기하는 루틴
        if (startNormalizedTime > 0)
        {
            yield return new WaitUntil(() => mAnim.GetCurrentAnimatorStateInfo(AnimatorLayerIndex).normalizedTime < startNormalizedTime);
        }

        // 입력으로 넣어준 타이밍이 넘어서면 이벤트 함수를 콜백해준다
        float preNormalizedTime = 0;
        while (true)
        {
            AnimatorStateInfo info = mAnim.GetCurrentAnimatorStateInfo(AnimatorLayerIndex);
            NormalizedTime = info.normalizedTime;
            float _preRate = preNormalizedTime - (int)preNormalizedTime;
            float _curRate = info.normalizedTime - (int)preNormalizedTime;
            foreach(var eventAnim in EventAnims)
            {
                float triggerRate = eventAnim.Item1;
                Action callbackFunc = eventAnim.Item2;
                if (_preRate < triggerRate && triggerRate <= _curRate)
                {
                    callbackFunc.Invoke();
                }
            }

            preNormalizedTime = info.normalizedTime;
            yield return null;
        }
    }

    protected void AddAnimEvent(float triggerRate, Action eventFunc)
    {
        EventAnims.Add(new Tuple<float, Action>(triggerRate, eventFunc));
    }
    protected void SetTrigger(AnimActionID actionID)
    {
        mAnim.SetInteger(AnimParam.ActionType, (int)actionID);
        mAnim.SetTrigger(AnimParam.DoActionTrigger);
    }
    protected void SetAnimParamVerticalDegreeIndex(Vector3 targetPos, int animCount)
    {
        //대상의 위치에 따라 재생되는 attack 애니메이션을 다르게 해줘야 한다.
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        float deg = MyUtils.GetDegree(Vector3.up, dir.normalized);
        deg = Mathf.Abs(deg);
        int stepDegree = 180 / animCount;
        int animIndex = (int)deg / stepDegree;
        mAnim.SetInteger(AnimParam.VerticalDegreeIndex, animIndex + 1);
    }

    protected bool IsCooltime()
    {
        return mCooltime <= Time.realtimeSinceStartup;
    }
    protected void SetCooltime(float cooltimeSeconds)
    {
        mCooltime = Time.realtimeSinceStartup + cooltimeSeconds;
    }

}
