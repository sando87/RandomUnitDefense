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

        // CurrentHP = Property.TotalHP;
        // HPBar = GetComponent<HealthBar>();
        // HPBar.Init(Height, HealthBarSize.Medium);
        // FSM.ChangeState(UnitState.Move);
    }
    public override void Release()
    {
        base.Release();
    }

    public void GetDamaged(SpecProperty attacker)
    {
        // if (CurrentState == UnitState.Death)
        //     return;

        // float hitDamage = attacker.AttackDamage - Property.Armor;
        // CurrentHP -= Mathf.Max(hitDamage, 0);
        // HPBar.UpdateHealthBar(CurrentHP / Property.TotalHP);
        // if (CurrentHP <= 0)
        // {
        //     CurrentHP = 0;
        //     FSM.ChangeState(UnitState.Death);
        // }
    }
}
