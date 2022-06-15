using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecProperty : MonoBehaviour
{
    [SerializeField] BasicSpec _Spec = new BasicSpec();

    private BuffProperty mBuff = null;
    private RGameSystemManager mGameSystem = null;

    void Start()
    {
        mGameSystem = RGame.Get<RGameSystemManager>();
        mBuff = this.GetBaseObject().BuffProp;
    }

    public float AttackDamage
    {
        get
        {
            float damage = _Spec.attackDamageBasic;
            damage += mGameSystem.GetUpgradeCount(_Spec.attackType) * _Spec.damagePerUpgrade;
            damage += _Spec.level * 10;
            return damage * mBuff.AttackDamage;
        }
    }
    public float TotalHP { get { return _Spec.totalHP * mBuff.TotalHP; } }
    public float Armor { get { return 0; } }
    public float MoveSpeed { get { return 0; } }
    public float AttackSpeed { get { return 0; } }
    public float AttackRange { get { return 0; } }
    public float Cooltime { get { return 0; } }
    public float SkillRange { get { return 0; } }
    public float SkillDuration { get { return 0; } }
    public int Level { get { return _Spec.level; } set { _Spec.level = value; } }

}


[Serializable]
public struct BasicSpec
{
    //기본적인 속성
    public int level;
    public float totalHP;             //체력
    public float armor;               //방어력
    public float moveSpeed;           //이동 속도
    public UpgradeType attackType;    //공격(무기) 타입
    public float attackDamageBasic;   //기본 공격력
    public float damagePerUpgrade;    //업당 기본공격 증가량
    public float attackSpeed;         //기본 공격 속도
    public float attackRange;         //기본 공격 범위
    public float cooltime;            //스킬 쿨타임
    public float skillRange;          //스킬 범위
    public float skillDuration;       //스킬 지속시간

    //기타.. 여러가지 스킬 기획에 따른 속성값 추가 필요...
}