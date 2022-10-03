﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthBarSize { Small, Medium, Big }

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject HealthInnerBar;
    [SerializeField] Transform HealthBarLeftPivot;
    [SerializeField] GameObject SplitBarSmallPrefab;
    [SerializeField] GameObject SplitBarBigPrefab;
    [SerializeField] float HealthStepForSplit = 20.0f;
    [SerializeField] int SmallBarCountForBig = 10;
    [SerializeField] float HpTotalWorldWidth = 1;

    private Health mHP = null;

    void Awake()
    {
        mHP = this.GetBaseObject().Health;
        mHP.EventDamaged += OnDamaged;
    }

    void Start()
    {
        InitSplitBars();
        
        transform.forward = Vector3.forward;
        
        //HideBar();
    }

    void Update()
    {
        transform.forward = Vector3.forward;
    }

    private void OnDamaged(float validDamage, BaseObject attacker)
    {
        if(mHP.IsDead)
        {
            HideBar();
        }
        else if(validDamage > 0)
        {
            ShowHealthBar();
        }
    }

    public void ShowHealthBar()
    {
        float rate = mHP.CurrentHealthRate;
        gameObject.SetActive(true);
        float rateClamp = Mathf.Clamp(rate, 0, 1);
        HealthInnerBar.transform.localScale = new Vector3(rateClamp, 1, 1);
        CancelInvoke();
        Invoke("HideBar", 5);
    }

    public void HideBar()
    {
        gameObject.SetActive(false);
    }

    void InitSplitBars()
    {
        foreach(Transform child in HealthBarLeftPivot)
            Destroy(child.gameObject);

        int count = 1;
        float curHP = HealthStepForSplit;
        while(curHP < mHP.MaxHP)
        {
            float rate = curHP / mHP.MaxHP;
            float localPosX = rate * HpTotalWorldWidth;


            if(count % SmallBarCountForBig == 0)
            {
                GameObject splitBigBar = Instantiate(SplitBarBigPrefab, HealthBarLeftPivot);
                splitBigBar.transform.localPosition = new Vector3(localPosX, 0, 0);
            }
            else
            {
                GameObject splitSmallBar = Instantiate(SplitBarSmallPrefab, HealthBarLeftPivot);
                splitSmallBar.transform.localPosition = new Vector3(localPosX, 0, 0);
            }
            
            count++;
            curHP += HealthStepForSplit;
        }

    }
}
