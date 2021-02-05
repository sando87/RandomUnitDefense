using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase
{
    public abstract string BuffID { get; }
    public abstract bool IsOver();

    public virtual void OnStartEffect(RGameObject target) { }
    public virtual void OnUpdateEffect(RGameObject target) { }
    public virtual void OnEndEffect(RGameObject target) { }
}
