using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public UserUnit Owner = null;
    public float remainSec = 0;
    public string BuffID = "";

    public abstract void OnEffect(MobSpec mobSpec, float remainSec);
}
