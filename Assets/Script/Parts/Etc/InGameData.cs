using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InGameData : MonoBehaviour
{
    private static InGameData mInst = null;
    public static InGameData Instance 
    {
        get
        {
            if(mInst == null)
            {
                GameObject obj = new GameObject();
                mInst = obj.AddComponent<InGameData>();
            }
            return mInst;
        }
    }

    void Awake()
    {
        mInst = this;
        DontDestroyOnLoad(gameObject);
    }

    public int RemainLife = 3;
    public bool IsGuidePopupShowed = false;
    public bool IsGetMarineSuit = false;
    public bool IsGetBioSuit = false;
    public string[] MapFileNames = null;
    private GameObject mPlayer = null;

    public void ResetData()
    {
        RemainLife = 3;
        // IsGetBioSuit = false;
        // IsGetMarineSuit = false;
        // IsGuidePopupShowed = false;
        MapFileNames = null;
        if(mPlayer != null)
        {
            Destroy(mPlayer);
            mPlayer = null;
        }
    }

    public void KeepPlayerObject(GameObject obj)
    {
        if (mPlayer != null)
        {
            Destroy(mPlayer);
            mPlayer = null;
        }

        mPlayer = obj;
        mPlayer.transform.SetParent(transform);
    }
    public GameObject GetKeptedPlayer()
    {
        return mPlayer;
    }
}
