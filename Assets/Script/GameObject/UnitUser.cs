using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitUser : UnitBase
{
    public override void Init()
    {
        base.Init();
    }
    public override void Release()
    {
        base.Release();
    }

    public abstract string SkillDescription { get; }
}
