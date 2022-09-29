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
    public Percent SkillSpeed;
    public Percent SkillDamage;
    public Percent SkillRange;
    public Percent SkillDuration;
    public Percent SplshRange;
    public Percent Percentage;

    public void AddBuffProp(BuffProperty prop)
    {
        TotalHP += prop.TotalHP;
        Armor += prop.Armor;
        MoveSpeed += prop.MoveSpeed;
        AttackDamage += prop.AttackDamage;
        AttackSpeed += prop.AttackSpeed;
        AttackRange += prop.AttackRange;
        SkillSpeed += prop.SkillSpeed;
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
        SkillSpeed -= prop.SkillSpeed;
        SkillDamage -= prop.SkillDamage;
        SkillRange -= prop.SkillRange;
        SplshRange -= prop.SplshRange;
        SkillDuration -= prop.SkillDuration;
        Percentage -= prop.Percentage;
    }
    public void MultiplyBuffProp(float rate)
    {
        TotalHP *= rate;
        Armor *= rate;
        MoveSpeed *= rate;
        AttackDamage *= rate;
        AttackSpeed *= rate;
        AttackRange *= rate;
        SkillSpeed *= rate;
        SkillDamage *= rate;
        SkillRange *= rate;
        SplshRange *= rate;
        SkillDuration *= rate;
        Percentage *= rate;
    }
    public string ToPropInfo()
    {
        List<string> item = new List<string>();
        
        if(TotalHP > 0) item.Add(string.Format(TotalHP + $":{TotalHP}%"));
        if(Armor > 0) item.Add(string.Format(Armor + $":{Armor}%"));
        if(MoveSpeed > 0) item.Add(string.Format(MoveSpeed + $":{MoveSpeed}%"));
        if(AttackDamage > 0) item.Add(string.Format(AttackDamage + $":{AttackDamage}%"));
        if(AttackSpeed > 0) item.Add(string.Format(AttackSpeed + $":{AttackSpeed}%"));
        if(AttackRange > 0) item.Add(string.Format(AttackRange + $":{AttackRange}%"));
        if(SkillSpeed > 0) item.Add(string.Format(SkillSpeed + $":{SkillSpeed}%"));
        if(SkillDamage > 0) item.Add(string.Format(SkillDamage + $":{SkillDamage}%"));
        if(SkillRange > 0) item.Add(string.Format(SkillRange + $":{SkillRange}%"));
        if(SplshRange > 0) item.Add(string.Format(SplshRange + $":{SplshRange}%"));
        if(SkillDuration > 0) item.Add(string.Format(SkillDuration + $":{SkillDuration}%"));
        if(Percentage > 0) item.Add(string.Format(Percentage + $":{Percentage}%"));

        return String.Join(",", item);
    }
}

