using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터의 Input을 추상화하여 처리함
// 실제 Input을 넣어주는 class는 UserInput, AI_Input, RemoteInput 참고
// 추상화한 이유는 특정 캐릭터는 제어하는 로직이 상황에 따라 다르기 때문
// 유저가 직접 제어하는 메인 캐릭터, 컴퓨터가 제어하는 몬스터, 멀티플레이어가 제어하는 캐릭터 등등...

public enum CharacterInputType
{
    MoveHori, Jump, AttackNormal, AttackMelee, Rolling, Throw, MoveVert, Detected, Transformed, TransformAttack, MoveFlying, Attack3, Interact, Pause
}

public class CharacterInput : MonoBehaviour
{
    private float[] mInputData = new float[MyUtils.CountEnum<CharacterInputType>()];

    public event Action<CharacterInputType> EventInput;
    
    protected void InvokeInputEvnt(CharacterInputType type)
    {
        if (Lock) return;

        EventInput?.Invoke(type);
    }
    public virtual float GetInput(CharacterInputType type)
    {
        return Lock ? 0 : mInputData[(int)type];
    }
    protected virtual void SetInput(CharacterInputType type, float value)
    {
        mInputData[(int)type] = value;
    }

    public virtual bool Lock { get; set; } = false;

    public BaseObject TargetDetected = null;
    public Vector3 MoveDir = Vector3.zero;

    public float HorizontalMove { get { return GetInput(CharacterInputType.MoveHori); } }
    public float VerticalMove { get { return GetInput(CharacterInputType.MoveVert); } }
    public bool Jump { get { return GetInput(CharacterInputType.Jump) > 0; } }
    public bool Attack1 { get { return GetInput(CharacterInputType.AttackNormal) > 0; } }
    public bool Attack2 { get { return GetInput(CharacterInputType.AttackMelee) > 0; } }
    public bool Rolling { get { return GetInput(CharacterInputType.Rolling) > 0; } }
    public bool Throw { get { return GetInput(CharacterInputType.Throw) > 0; } }
    public bool Detected { get { return GetInput(CharacterInputType.Detected) > 0; } }
    public bool Transformed { get { return GetInput(CharacterInputType.Transformed) > 0; } }
    public bool TransformAttack { get { return GetInput(CharacterInputType.TransformAttack) > 0; } }

}
