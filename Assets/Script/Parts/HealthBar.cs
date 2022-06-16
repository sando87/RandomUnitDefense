using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthBarSize { Small, Medium, Big }

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject HealthInnerBar;

    private Health mHP = null;

    void Start()
    {
        mHP = this.GetBaseObject().Health;
        mHP.EventDamaged += OnDamaged;
        
        HideBar();
    }

    private void OnDamaged(float validDamage, BaseObject attacker)
    {
        if(validDamage > 0)
        {
            UpdateHealthBar(mHP.CurrentHealthRate);
        }
    }

    public void UpdateHealthBar(float rate)
    {
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
}
