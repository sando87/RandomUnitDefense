﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HealthBarSize { Small, Medium, Big }

public class HealthBar : MonoBehaviour
{
    private GameObject HeathBar;
    private GameObject HealthBarGreen;
    private GameObject HealthBarRed;
    private Vector3 localScale = new Vector3(1, 1, 1);

    public void Init(float offHeight, HealthBarSize barSize)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/HealthBar");
        HeathBar = Instantiate(prefab, transform);
        HealthBarGreen = HeathBar.transform.Find("Green").gameObject;
        HealthBarRed = HeathBar.transform.Find("Red").gameObject;


        HealthBarGreen.SetActive(false);
        HealthBarRed.SetActive(true);

        float size = barSize == HealthBarSize.Small ? 0.2f : (barSize == HealthBarSize.Medium ? 0.35f : 0.7f);
        HeathBar.transform.localScale = new Vector3(size, size, 1);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float height = offHeight + sr.size.y * size * 0.5f;
        HeathBar.transform.localPosition = new Vector3(0, height, 0);

        HeathBar.gameObject.SetActive(false);
    }


    public void UpdateHealthBar(float rate)
    {
        HeathBar.gameObject.SetActive(true);
        float rateClamp = Mathf.Clamp(rate, 0, 1);
        localScale.x = rateClamp;
        HealthBarGreen.transform.localScale = localScale;
        HealthBarRed.transform.localScale = localScale;
        CancelInvoke();
        Invoke("HideBar", RSystemConfig.HealthBarDisplaySec);
    }

    public void HideBar()
    {
        HeathBar.gameObject.SetActive(false);
    }
}
