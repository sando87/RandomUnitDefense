using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자가 키보드나 조이스틱으로 입력한 함수를 그대로 Warpper하여 전달한다.
public class UserInput : CharacterInput
{
    public event System.Action EventSelected;
    public event System.Action EventDeSelected;
    public event System.Action<Vector3> EventMove;

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
    }
}
