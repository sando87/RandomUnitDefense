using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    private const string KeyColor = "_Color";
    private const string KeyBrightness = "_Brightness";
    private const string KeyBlindAmount = "_BlindAmount";

    private BaseObject mBaseObject = null;
    private Renderer mRenderer = null;

    public bool Lock { get { return !mRenderer.enabled; } set { mRenderer.enabled = !value; } }
    public int SortingLayerID { get { return mRenderer.sortingLayerID; } set { mRenderer.sortingLayerID = value; } }
    public int SortingOrder { get { return mRenderer.sortingOrder; } set { mRenderer.sortingOrder = value; } }

    void Start()
    {
        mBaseObject = this.GetBaseObject();

        mRenderer = GetComponent<Renderer>();

        SetColor(Color.white);
        SetBrightness(0);
        SetBlindAmount(1);
    }

    public Color GetColor()
    {
        return mRenderer.material.GetColor(KeyColor);
    }
    public void SetColor(Color color)
    {
        mRenderer.material.SetColor(KeyColor, color);
    }
    public float GetAlpha()
    {
        return GetColor().a;
    }
    public void SetAlpha(float alpha)
    {
        Color color = GetColor();
        color.a = alpha;
        SetColor(color);
    }
    public float GetBrightness()
    {
        return mRenderer.material.GetFloat(KeyBrightness);
    }
    public void SetBrightness(float brightness)
    {
        mRenderer.material.SetFloat(KeyBrightness, brightness);
    }
    public float GetBlindAmount()
    {
        return mRenderer.material.GetFloat(KeyBlindAmount);
    }
    public void SetBlindAmount(float blindAmount)
    {
        mRenderer.material.SetFloat(KeyBlindAmount, blindAmount);
    }


}
