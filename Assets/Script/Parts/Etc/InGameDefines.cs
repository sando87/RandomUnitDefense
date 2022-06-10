using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectAxis
{
    Square, Vertical, Horizon
}

public interface IDamagable
{
    void OnDamaged(int damage, BaseObject attacker);
}

public interface IMapEditorObject
{
    void OnInitMapEditor();
}

