﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{
    public event Action<float, BaseObject> EventDamaged = null;

    public bool IsImmortal { get; set; }
    public float MaxHP { get { return mBaseObject.SpecProp.TotalHP; } }
    public bool IsDead { get { return CurrentHealth <= 0; } }
    public float CurrentHealth { get; private set; } = 0;
    public float CurrentHealthRate { get { return CurrentHealth / MaxHP; } }
    public BaseObject LastAttacker { get; private set; } = null;
    public Vector3 LastHitPoint { get; set; } = Vector3.zero;

    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        CurrentHealth = MaxHP;
    }

    public void GetDamaged(float damage, BaseObject attacker)
    {
        if(IsDead) return;

        float validDamage = Mathf.Max(damage - mBaseObject.SpecProp.Armor, 0);
        if(IsImmortal) validDamage = 0;

        LastAttacker = attacker;
        CurrentHealth -= validDamage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        EventDamaged?.Invoke(damage, attacker);
    }
}
