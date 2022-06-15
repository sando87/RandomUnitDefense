using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터의 Input을 추상화하여 처리함

public class CharacterInput : MonoBehaviour
{
    private float[] mInputData = new float[MyUtils.CountEnum<InputType>()];

    public event Action<InputType> EventInput;
    public virtual bool Lock { get; set; } = false;
    
    protected void InvokeInputEvnt(InputType type)
    {
        if (Lock) return;

        EventInput?.Invoke(type);
    }
    public virtual float GetInput(InputType type)
    {
        return Lock ? 0 : mInputData[(int)type];
    }
    protected virtual void SetInput(InputType type, float value)
    {
        mInputData[(int)type] = value;
    }


}
