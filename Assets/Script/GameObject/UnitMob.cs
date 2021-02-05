﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMob : UnitUserBase
{
    public HealthBar HPBar = null;
    private float CurrentHP = 0;

    public override void Init()
    {
        base.Init();

        CurrentHP = Spec.TotalHP;
        HPBar.Init(Spec.CharacterHeight, HealthBarSize.Medium);
        FSM.ChangeState(UnitState.Move);
    }
    public override void Release()
    {
        base.Release();
    }

    public void GetDamaged(UnitSpec attacker)
    {
        CurrentHP -= attacker.AttackDamage;
        HPBar.UpdateHealthBar(CurrentHP / Spec.TotalHP);
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            FSM.ChangeState(UnitState.Death);
        }
    }
}
