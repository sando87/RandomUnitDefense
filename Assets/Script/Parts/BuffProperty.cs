using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffProperty : MonoBehaviour
{
    public Percent TotalHP;
    public Percent Armor;
    public Percent MoveSpeed;
    public Percent AttackDamage;
    public Percent AttackSpeed;
    public Percent AttackRange;
    public Percent Cooltime;
    public Percent SkillDamage;
    public Percent SkillRange;
    public Percent SplshRange;
    public Percent SkillDuration;
    public Percent Percentage;

    public void AddBuffProp(BuffProperty prop)
    {
        TotalHP += prop.TotalHP;
        Armor += prop.Armor;
        MoveSpeed += prop.MoveSpeed;
        AttackDamage += prop.AttackDamage;
        AttackSpeed += prop.AttackSpeed;
        AttackRange += prop.AttackRange;
        Cooltime += prop.Cooltime;
        SkillDamage += prop.SkillDamage;
        SkillRange += prop.SkillRange;
        SplshRange += prop.SplshRange;
        SkillDuration += prop.SkillDuration;
        Percentage += prop.Percentage;
    }
    public void RemoveBuffProp(BuffProperty prop)
    {
        TotalHP -= prop.TotalHP;
        Armor -= prop.Armor;
        MoveSpeed -= prop.MoveSpeed;
        AttackDamage -= prop.AttackDamage;
        AttackSpeed -= prop.AttackSpeed;
        AttackRange -= prop.AttackRange;
        Cooltime -= prop.Cooltime;
        SkillDamage -= prop.SkillDamage;
        SkillRange -= prop.SkillRange;
        SplshRange -= prop.SplshRange;
        SkillDuration -= prop.SkillDuration;
        Percentage -= prop.Percentage;
    }
}

