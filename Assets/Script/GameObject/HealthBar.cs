using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBar : RGameObject
{
    public enum HealthBarSize { Small, Medium, Big }

    public GameObject HealthBarPrefab;
    public HealthBarSize BarSize;

    private SpriteRenderer BarSpriteRenderer;
    private GameObject HealthBarBG;
    private GameObject HealthBarGreen;
    private GameObject HealthBarRed;
    private Vector3 localScale = new Vector3(1, 1, 1);

    public float OffsetHeight { get; set; }

    void Start()
    {
        HealthBarBG = Instantiate(HealthBarPrefab, transform);
        HealthBarGreen = HealthBarBG.transform.Find("Green").gameObject;
        HealthBarRed = HealthBarBG.transform.Find("Red").gameObject;

        BarSpriteRenderer = HealthBarBG.GetComponent<SpriteRenderer>();

        HealthBarGreen.SetActive(false);
        HealthBarRed.SetActive(true);
        float size = BarSize == HealthBarSize.Small ? 0.2f : (BarSize == HealthBarSize.Medium ? 0.35f : 0.7f);
        HealthBarBG.transform.localScale = new Vector3(size, size, 1);

        float offHeight = OffsetHeight + BarSpriteRenderer.size.y * HealthBarBG.transform.localScale.y * 0.5f;
        HealthBarBG.transform.localPosition = new Vector3(0, offHeight, 0);

        HealthBarBG.SetActive(false);
    }


    public void UpdateHealthBar(float rate)
    {
        HealthBarBG.SetActive(true);
        float rateClamp = Mathf.Clamp(rate, 0, 1);
        localScale.x = rateClamp;
        HealthBarGreen.transform.localScale = localScale;
        HealthBarRed.transform.localScale = localScale;
        Invoke("HideBar", RSystemConfig.HealthBarDisplaySec);
    }

    void HideBar()
    {
        HealthBarBG.SetActive(false);
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    public override void Release()
    {
        throw new System.NotImplementedException();
    }
}
