using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMob : UnitBase
{
    public HealthBar HPBar { get; set; }
    private float CurrentHP = 0;

    public override void Init()
    {
        base.Init();

        CurrentHP = Spec.Current.TotalHP;
        HPBar = GetComponent<HealthBar>();
        HPBar.Init(Height, HealthBarSize.Medium);
        FSM.ChangeState(UnitState.Move);
    }
    public override void Release()
    {
        base.Release();
    }

    public void GetDamaged(UnitSpec attacker)
    {
        if (CurrentState == UnitState.Death)
            return;

        CurrentHP -= attacker.Current.AttackDamageBasic;
        HPBar.UpdateHealthBar(CurrentHP / Spec.Current.TotalHP);
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            FSM.ChangeState(UnitState.Death);
        }
    }
}
