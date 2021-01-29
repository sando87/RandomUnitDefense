using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RUiUnitCard : MonoBehaviour
{
    public bool Owned
    {
        get { return !transform.GetChild(1).gameObject.activeSelf; }
        set { transform.GetChild(1).gameObject.SetActive(!value); }
    }
    public bool StartingMember
    {
        get { return transform.GetChild(0).gameObject.activeSelf; }
        set { transform.GetChild(0).gameObject.SetActive(value); }
    }
    public string GetDetail()
    {
        string textString = "Name : "+ name + "\n" +
            "Damage: "+ Damage + " + 2\n" +
            "Weapon: Missile\n" +
            "Grade : Normal";
        return textString;
    }
    public int Damage { get; set; } = 10;
}
