using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseRenderer : MonoBehaviour
{
    private BaseObject mBaseObject = null;
    private ShaderController[] mRenderers = null;

    public bool Lock { get { return mRenderers[0].Lock; } set { foreach(var rd in mRenderers) rd.Lock = value; } }
    public int SortingLayerID { get { return mRenderers[0].SortingLayerID; } set { foreach(var rd in mRenderers) rd.SortingLayerID = value; } }
    public int SortingOrder { get { return mRenderers[0].SortingOrder; } set { foreach(var rd in mRenderers) rd.SortingOrder = value; } }

    void Start()
    {
        mBaseObject = this.GetBaseObject();

        mRenderers = GetComponentsInChildren<ShaderController>();
    }


    public void SetColor(Color color)
    {
        foreach(var rd in mRenderers)
        {
            rd.SetColor(color);
        }
    }
    public void SetAlpha(float alpha)
    {
        foreach(var rd in mRenderers)
        {
            rd.SetAlpha(alpha);
        }
    }
    public void SetBrightness(float brightness)
    {
        foreach(var rd in mRenderers)
        {
            rd.SetBrightness(brightness);
        }
    }


    public void FadeOut(float duration)
    {
        float alpha = 1;
        DOTween.To(() => alpha, (_alpha) => { SetAlpha(_alpha); alpha = _alpha; }, 0, duration);
    }
    public void FadeIn(float duration)
    {
        float alpha = 0;
        DOTween.To(() => alpha, (_alpha) => { SetAlpha(_alpha); alpha = _alpha; }, 1, duration);
    }

    // 하얗게 반짝거리는 효과
    public void StartTwinkle()
    {
        SetBrightness(1);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            SetBrightness(0);
        });
    }
    
    // 보였다 안보였다 깜빡거리는 효과
    public void StartBlink(float duration)
    {
        int blinkCount = 10;
        float interval = duration / blinkCount;
        float alpha = 0;
        DOTween.To(() => alpha, (_a) => { alpha = _a; SetAlpha((int)_a); }, 1.9f, interval).From(0).SetEase(Ease.Linear).SetLoops(blinkCount);
    }

    // 특정색으로 깜빡거리는 효과
    public void StartColor(Color color, float interval = 0.1f, int count = 2)
    {
        StartCoroutine(CoStartColor(color, interval, count));
    }
    IEnumerator CoStartColor(Color targetColor, float interval, int count)
    {
        for(int i = 0; i < count; ++i)
        {
            SetColor(targetColor);
            yield return new WaitForSeconds(interval);
            SetColor(Color.white);
            yield return new WaitForSeconds(interval);
        }
    }
    

    public void ShakeObject(float strength, float duration)
    {
        transform.DOShakePosition(duration, strength);
    }

}
