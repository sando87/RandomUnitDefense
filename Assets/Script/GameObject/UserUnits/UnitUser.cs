using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitUser : UnitBase, IUserInputReciever
{
    public abstract string SkillDescription { get; }

    public override void Init()
    {
        base.Init();
    }
    public override void Release()
    {
        base.Release();
    }

    public virtual void OnClick()
    {
        //유닛 클릭시 Merge 선택창 UI 띄우기
        RGame.Get<RGameSystemManager>().HUDObject.Show(this);
    }

    public virtual void OnDragAndDrop(Vector3 dropWorldPos)
    {
        //Drag한 지점으로 유닛 이동
        GetComponent<MotionMove>().Destination = dropWorldPos;
        FSM.ChangeState(UnitState.Move);
    }

    public virtual void OnDragging(Vector3 draggingWorldPos)
    {
    }

}
