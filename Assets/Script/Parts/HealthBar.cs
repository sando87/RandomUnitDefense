using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HealthBarSize { Small, Medium, Big }

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject HealthInnerBar;
    [SerializeField] Transform HealthBarLeftPivot;
    [SerializeField] GameObject SplitBarSmallPrefab;
    [SerializeField] GameObject SplitBarBigPrefab;
    [SerializeField] int _HPStepForSplit = 100;
    [SerializeField] int SmallBarCountForBig = 10;
    [SerializeField] float HpTotalWorldWidth = 1;

    public int HealthStepForSplit { get; set; }

    private Health mHP = null;

    void Awake()
    {
        mHP = this.GetBaseObject().Health;
        mHP.EventDamaged += OnDamaged;
    }

    void Start()
    {
        InitHealthStep();
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
        HealthInnerBar.GetComponent<Image>().fillAmount = rateClamp;
        CancelInvoke();
        Invoke("HideBar", 5);
    }

    public void HideBar()
    {
        gameObject.SetActive(false);
    }

    void InitHealthStep()
    {
        int step = _HPStepForSplit;
        while(((float)step / mHP.MaxHP) < 0.03f)
        {
            step *= SmallBarCountForBig;
        }
        HealthStepForSplit = step;
    }
    Color GetSplitBarColor(bool isBig)
    {
        int idx = (int)Mathf.Log(HealthStepForSplit / _HPStepForSplit, SmallBarCountForBig);
        if(isBig)
            idx++;

        if(idx == 0)
            return Color.black;
        else if (idx == 1)
            return Color.gray;
        else if(idx == 2)
            return Color.green;
        else if (idx == 3)
            return Color.blue;
        else if (idx == 4)
            return Color.yellow;
        else if (idx == 5)
            return Color.cyan;
        else if (idx == 6)
            return Color.magenta;

        return Color.white;
    }

    void InitSplitBars()
    {
        foreach(Transform child in HealthBarLeftPivot)
            Destroy(child.gameObject);

        int count = 1;
        float curHP = HealthStepForSplit;
        float BigPartHP = HealthStepForSplit * SmallBarCountForBig;
        while(curHP < mHP.MaxHP)
        {
            float rate = curHP / mHP.MaxHP;
            float localPosX = rate * HpTotalWorldWidth;

            if(count % SmallBarCountForBig == 0)
            {
                GameObject splitBigBar = Instantiate(SplitBarBigPrefab, HealthBarLeftPivot);
                splitBigBar.transform.localPosition = new Vector3(localPosX, 0, 0);
                splitBigBar.GetComponentInChildren<Image>().color = GetSplitBarColor(true);
            }
            else
            {
                GameObject splitSmallBar = Instantiate(SplitBarSmallPrefab, HealthBarLeftPivot);
                splitSmallBar.transform.localPosition = new Vector3(localPosX, 0, 0);
                splitSmallBar.GetComponentInChildren<Image>().color = GetSplitBarColor(false);
                float widthRate = Mathf.Min(1.0f, BigPartHP / mHP.MaxHP);
                splitSmallBar.transform.localScale = new Vector3(widthRate, 1, 1);
            }
            
            count++;
            curHP += HealthStepForSplit;
        }

    }
}
