using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] LevelImages = null;
    private int Level = 1;

    public int GetLevel() { return Level; }
    public void SetLevel(int level)
    {
        Level = level;
        int imgIndex = level - 1;
        GetComponent<SpriteRenderer>().sprite = LevelImages[imgIndex];
    }
}
