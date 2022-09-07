using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    void Start()
    {
        int curLevel = this.GetBaseObject().SpecProp.Level;
        for (int i = 0; i < transform.childCount; ++i)
        {
            int starLevel = i + 2;
            transform.GetChild(i).gameObject.SetActive(starLevel <= curLevel);
        }
    }
}
