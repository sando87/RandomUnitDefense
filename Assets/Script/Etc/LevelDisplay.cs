using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelDisplay : MonoBehaviour
{
    [SerializeField] Sprite[] LevelImages = null;
    
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = LevelImages[this.GetBaseObject().SpecProp.Level - 1];
    }
}
