using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] int _SpecialSkillCount = 3;

    private BaseObject mBaseObject = null;
    public int RemainSpecialSkillCount { get; set; } = 0;
    public bool IsNormalAttackFired { get; set; } = false;
    public BaseObject ChildPlayerObject { get; set; } = null;
    //public int CurrentSuitLevel { get; set; } = 0;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        RemainSpecialSkillCount = _SpecialSkillCount;
        
        StartRespawnImmortalState();
    }

    public void StartRespawnImmortalState()
    {
        // player가 처음 시작하거나 죽었다 다시 시작할 경우 처음 수초간 깜빡거리며 무적상태진입...
        StartCoroutine(CoStartRespawnImmortalState());
    }

    IEnumerator CoStartRespawnImmortalState()
    {
        mBaseObject.Body.Lock = true;
        yield return null;
        mBaseObject.Renderer.StartBlink(4.0f);
        yield return new WaitForSeconds(4.0f);
        mBaseObject.Body.Lock = false;
    }

}
