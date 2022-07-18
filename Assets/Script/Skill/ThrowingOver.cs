using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ThrowingOver : MonoBehaviour
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    public Action EventHit { get; set; }

    // Start is called before the first frame update
    public void Launch(Vector3 dest)
    {
        SpritesAnimator.Play(transform.position, IntroSprites);
        transform.DOMoveX(dest.x, 2).SetEase(Ease.Linear);
        transform.DORotate(new Vector3(0, 0, 1000), 2, RotateMode.FastBeyond360);
        transform.DOMoveY(transform.position.y + 3, 1).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOMoveY(dest.y, 1).SetEase(Ease.InQuad).OnComplete(() =>
            {
                SpritesAnimator.Play(transform.position, OutroSprites);
                EventHit?.Invoke();
                Destroy(gameObject);
            });
        });
    }
}
