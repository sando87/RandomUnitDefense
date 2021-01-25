using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBaseComponent 
{
    private RBaseComponent() { }
    public RBaseComponent(RGameObject owner)
    {
        Owner = owner;
    }

    public RGameObject Owner { get; set; } 
    public static uint Identifier { get; set; }

}
