using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGameOperator : RManager
{
    public override void Init()
    {
        base.Init();
    }

    public void StartGame()
    {
        UserInputLocked = false;
        StartCoroutine(UserInputInterpreter());
    }
    public void CleanUpGame()
    {
        StopAllCoroutines();
    }


    public bool UserInputLocked { get; set; }
    // Unity System Input을 최초로 받아 게임에서 사용하기 쉬운형태로 변환 후 호출해줌
    // Collider 컴포넌트가 게임오브젝트에 활성화되어 있어야 함
    private IEnumerator UserInputInterpreter()
    {
        IUserInputReciever DownReciever = null;
        Vector3 DownPosition = Vector3.zero;
        bool IsDragged = false;

        while(true)
        {
            if (UserInputLocked)
            {
                if(DownReciever != null)
                {
                    if (IsDragged)
                    {
                        Vector3 worldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        DownReciever?.OnDragAndDrop(worldPt);
                    }
                    else
                    {
                        DownReciever?.OnClick();
                    }
                }
                
                DownReciever = null;
                DownPosition = Vector3.zero;
                IsDragged = false;
                continue;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(worldPt);
                if (hit != null && hit.gameObject.activeSelf)
                {
                    IUserInputReciever reciever = hit.gameObject.GetComponent<IUserInputReciever>();
                    if(reciever != null)
                    {
                        DownReciever = reciever;
                        DownPosition = worldPt;
                        IsDragged = false;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector3 worldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (IsDragged)  // Drag & Drop Event 발생
                {
                    DownReciever?.OnDragAndDrop(worldPt);
                }
                else  // Click Event 발생
                {
                    DownReciever?.OnClick();
                }

                DownReciever = null;
                DownPosition = Vector3.zero;
                IsDragged = false;
            }
            else if (Input.GetMouseButton(0))
            {
                if (DownReciever != null) //현재 Down이 된 상태일때만 진입
                {
                    if (IsDragged) //사용자가 Dragging하는 매프래임마다 진입
                    {
                        Vector3 curWorldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        DownReciever.OnDragging(curWorldPt);
                    }
                    else //사용자가 처음 Drag하는 순간에만 진입
                    {
                        Vector3 curWorldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if ((curWorldPt - DownPosition).magnitude >= RSystemConfig.DragThreshold)
                            IsDragged = true;
                    }
                    
                }
            }

            yield return null;
        }

    }
}
