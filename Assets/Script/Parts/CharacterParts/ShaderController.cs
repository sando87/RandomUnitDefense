using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    private BaseObject mBaseObject = null;
    private Renderer mRenderer = null;
    private MaterialPropertyBlock mBlock = null;
    private int mPropertyToIDColor = 0;
    private int mPropertyToIDAlpha = 0;
    private int mPropertyToIDBrightness = 0;// AddAllIn1Shader 사용해야하고 Contrast & Brightness 항목을 켜줘야한다.

    public bool Lock { get { return !mRenderer.enabled; } set { mRenderer.enabled = !value; } }
    public int SortingLayerID { get { return mRenderer.sortingLayerID; } set { mRenderer.sortingLayerID = value; } }
    public int SortingOrder { get { return mRenderer.sortingOrder; } set { mRenderer.sortingOrder = value; } }

    void Start()
    {
        mBaseObject = this.GetBaseObject();

        mRenderer = GetComponent<Renderer>();

        mPropertyToIDColor = Shader.PropertyToID("_Color");
        mPropertyToIDAlpha = Shader.PropertyToID("_Alpha");
        mPropertyToIDBrightness = Shader.PropertyToID("_Brightness");

        mBlock = new MaterialPropertyBlock();
        mRenderer.GetPropertyBlock(mBlock);
        mBlock.SetColor(mPropertyToIDColor, Color.white);
        mBlock.SetFloat(mPropertyToIDAlpha, 1);
        mBlock.SetFloat(mPropertyToIDBrightness, 0);
    }

    public Color GetColor()
    {
        Color color = mBlock.GetColor(mPropertyToIDColor);
        color.a = 1;
        return color;
    }
    public void SetColor(Color color)
    {
        color.a = 1;
        mBlock.SetColor(mPropertyToIDColor, color);
        mRenderer.SetPropertyBlock(mBlock);
    }
    public float GetAlpha()
    {
        return mBlock.GetFloat(mPropertyToIDAlpha);
    }
    public void SetAlpha(float alpha)
    {
        mBlock.SetFloat(mPropertyToIDAlpha, alpha);
        mRenderer.SetPropertyBlock(mBlock);
    }
    public float GetBrightness()
    {
        return mBlock.GetFloat(mPropertyToIDBrightness);
    }
    public void SetBrightness(float brightness)
    {
        // AddAllIn1Shader 사용해야하고 Contrast & Brightness 항목을 켜줘야한다.
        mBlock.SetFloat(mPropertyToIDBrightness, brightness);
        mRenderer.SetPropertyBlock(mBlock);
    }


}
