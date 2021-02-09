using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController
{
    public UnitBase Owner { get; private set; }

    private List<BuffBase> CurrentBuffObjects = new List<BuffBase>();

    public void Init(UnitBase owner)
    {
        Owner = owner;
    }

    public void AddBuff(BuffBase buff)
    {
        CurrentBuffObjects.Add(buff);
    }

    public T FindBuff<T>() where T : BuffBase
    {
        foreach (BuffBase buff in CurrentBuffObjects)
        {
            if (buff.GetType() == typeof(T))
                return buff as T;
        }
        return null;
    }
    public T[] FindBuffs<T>() where T : BuffBase
    {
        List<T> rets = new List<T>();
        foreach(BuffBase buff in CurrentBuffObjects)
        {
            if (buff.GetType() == typeof(T))
                rets.Add(buff as T);
        }
        return rets.ToArray();
    }

    public void UpdateBuffObjects()
    {
        BuffBase[] buffs = CurrentBuffObjects.ToArray();
        foreach (BuffBase buff in buffs)
        {
            float prvPlayTime = buff.PlayTime;
            buff.PlayTime += Time.deltaTime;
            if(prvPlayTime <= 0 && 0 < buff.PlayTime)
                buff.StartBuff(Owner);

            if(0 <= buff.PlayTime && buff.PlayTime < buff.Duration)
                buff.UpdateBuff(Owner);

            if(prvPlayTime <= buff.Duration && buff.Duration < buff.PlayTime)
                buff.EndBuff(Owner);

            if (buff.Duration < buff.PlayTime)
                CurrentBuffObjects.Remove(buff);
        }
    }

}

public struct Percent //단위 [%]
{
    private float value;
    private Percent(float val) { value = val; }
    public float Value { get => value; } // return 0 ~ 100%
    public float Rate { get => value * 0.01f; }  // return 0 ~ 1.0
    public static Percent operator +(Percent a, float b) { return new Percent(a.value + b); }
    public static Percent operator -(Percent a, float b) { return new Percent(a.value - b); }
}

public class BuffProperty
{
    public Percent TotalHP;
    public Percent Armor;
    public Percent MoveSpeed;
    public Percent AttackDamage;
    public Percent AttackSpeed;
    public Percent AttackRange;
    public Percent Cooltime;
    public Percent SkillRange;
    public Percent SkillDuration;
}

public class BuffBase
{
    public float Duration { get; set; } = 1;
    public float PlayTime { get; set; } = 0;

    public void RepeatBuff(float afterSec)
    {
        PlayTime = -afterSec;
    }
    public void RenewBuff()
    {
        PlayTime = 0.0001f;
    }

    public virtual void StartBuff(UnitBase target) { }
    public virtual void UpdateBuff(UnitBase target) { }
    public virtual void EndBuff(UnitBase target) { }
}
