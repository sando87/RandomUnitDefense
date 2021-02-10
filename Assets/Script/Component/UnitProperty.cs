using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct SpecProperty
{
    //기본적인 속성
    [SerializeField] public Sprite unitPhoto;          //UI에 사용되는 유닛 이미지
    [SerializeField] public float totalHP;             //체력
    [SerializeField] public float armor;               //방어력
    [SerializeField] public float moveSpeed;           //이동 속도
    [SerializeField] public UpgradeType attackType;    //공격(무기) 타입
    [SerializeField] public float attackDamageBasic;   //기본 공격력
    [SerializeField] public float damagePerUpgrade;    //업당 기본공격 증가량
    [SerializeField] public float attackSpeed;         //기본 공격 속도
    [SerializeField] public float attackRange;         //기본 공격 범위
    [SerializeField] public float cooltime;            //스킬 쿨타임
    [SerializeField] public float skillRange;          //스킬 범위
    [SerializeField] public float skillDuration;       //스킬 지속시간

    //기타.. 여러가지 스킬 기획에 따른 속성값 추가 필요...
}

public class UnitProperty : MonoBehaviour
{
    [SerializeField] private SpecProperty Spec = new SpecProperty();

    private BuffProperty Buff = null;
    private RGameSystemManager GameMgr = null;
    public UnitBase Owner { get; set; }
    public SpecProperty BasicSpec { get { return Spec; } }

    public void Init(UnitBase owner)
    {
        Name = gameObject.name;
        Level = 1;
        Owner = owner;
        Buff = Owner.BuffValues;
        GameMgr = RGame.Get<RGameSystemManager>();
    }

    public string Name { get; set; }
    public int Level { get; set; }
    public float AttackDamage
    {
        get
        {
            float attack = Spec.attackDamageBasic + (GameMgr.GetPower(Spec.attackType) * Spec.damagePerUpgrade * Level);
            return attack * (1 + Buff.AttackDamage.Rate);
        }
    }
    public float TotalHP { get { return Spec.totalHP * (1 + Buff.TotalHP.Rate); } }
    public float Armor { get { return Spec.armor * (1 + Buff.Armor.Rate); } }
    public float MoveSpeed { get { return Spec.moveSpeed * (1 + Buff.MoveSpeed.Rate); } }
    public float AttackSpeed { get { return Spec.attackSpeed * (1 + Buff.AttackSpeed.Rate); } }
    public float AttackRange { get { return Spec.attackRange * (1 + Buff.AttackRange.Rate); } }
    public float Cooltime { get { return Spec.cooltime * (1 + Buff.Cooltime.Rate); } }
    public float SkillRange { get { return Spec.skillRange * (1 + Buff.SkillRange.Rate); } }
    public float SkillDuration { get { return Spec.skillDuration * (1 + Buff.SkillDuration.Rate); } }

}
