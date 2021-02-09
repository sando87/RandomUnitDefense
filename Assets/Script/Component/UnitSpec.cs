using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpecProperty
{
    //기본적인 속성
    public float TotalHP;           //체력
    public float Armor;             //방어력
    public float MoveSpeed;         //이동 속도
    public UpgradeType AttackType;  //공격(무기) 타입
    public float AttackDamageBasic; //기본 공격력
    public float DamagePerUpgrade;  //업당 기본공격 증가량
    public float AttackSpeed;       //기본 공격 속도
    public float AttackRange;       //기본 공격 범위

    //스킬 관련 특수 속성들...
    public float DamageUpPercent;       //증뎀
    public float Cooltime;              //스킬 쿨타임
    public float SkillRange;            //스킬 범위
    public float SkillSplashRange;      //스플래쉬 범위
    public float SkillDuration;         //스킬 지속시간
    public float ProjectileCount;       //투사체 개수
    public float CriticalChancePercent; //크리티컬 확률
    public float CriticalDamagePercent; //크리티컬 증뎀
    //기타.. 여러가지 스킬 기획에 대한 속성값을 추가될 수 있음...

    public Sprite UnitPhoto;
}

public class UnitSpec : MonoBehaviour
{
    [SerializeField] private SpecProperty Basic = new SpecProperty();
    private SpecProperty current;

    public SpecProperty Current { get => current; }
    private List<BuffBase> BuffList = new List<BuffBase>();

    public void AddBuff(BuffBase buff)
    {
        buff.Target = GetComponent<UnitBase>();
        BuffList.Add(buff);
    }

    public void UpdateCurrent()
    {
        current = Basic;
        BuffBase[] buffs = BuffList.ToArray();
        foreach (BuffBase buff in buffs)
        {
            BuffState ret = buff.UpdateTargetSpec(Basic, out current);
            if (ret == BuffState.End)
                BuffList.Remove(buff);
        }
    }
}
