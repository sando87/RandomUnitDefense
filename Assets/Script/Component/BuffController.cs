using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    private List<BuffBase> mCurrentBuffObjects = new List<BuffBase>();
    private UnitBase Owner = null;

    void Start()
    {
        Owner = this.GetBaseObject().UnitBase;
    }

    public void AddBuff(BuffBase buff)
    {
        mCurrentBuffObjects.Add(buff);
    }

    public T FindBuff<T>() where T : BuffBase
    {
        foreach (BuffBase buff in mCurrentBuffObjects)
        {
            if (buff.GetType() == typeof(T))
                return buff as T;
        }
        return null;
    }
    public T[] FindBuffs<T>() where T : BuffBase
    {
        List<T> rets = new List<T>();
        foreach(BuffBase buff in mCurrentBuffObjects)
        {
            if (buff.GetType() == typeof(T))
                rets.Add(buff as T);
        }
        return rets.ToArray();
    }

    public void UpdateBuffObjects()
    {
        BuffBase[] buffs = mCurrentBuffObjects.ToArray();
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
                mCurrentBuffObjects.Remove(buff);
        }
    }

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
