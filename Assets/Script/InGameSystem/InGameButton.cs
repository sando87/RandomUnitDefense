using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InGameButtonClick : UnityEvent { }

public class InGameButton : MonoBehaviour
{
    public InGameButtonClick EventClick;


    void Start()
    {
    }



}
