using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] LevelImages;

    public void SetLevel(int level)
    {
        int imgIndex = level - 1;
        GetComponent<SpriteRenderer>().sprite = LevelImages[imgIndex];
    }
}
