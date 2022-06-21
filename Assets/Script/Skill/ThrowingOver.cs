using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ThrowingOver : MonoBehaviour
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    public BaseObject Target { get; private set; }
    public Action<BaseObject> EventHit { get; set; }

    // Start is called before the first frame update
    public void Launch(BaseObject target)
    {
        Target = target;

        SpritesAnimator.Play(transform.position, IntroSprites);
        transform.DOMoveX(Target.transform.position.x, 2);
        transform.DORotate(new Vector3(0, 0, 1000), 2, RotateMode.FastBeyond360);
        transform.DOMoveY(transform.position.y + 3, 1).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOMoveY(Target.transform.position.y, 1).SetEase(Ease.InQuad).OnComplete(() =>
            {
                SpritesAnimator.Play(transform.position, OutroSprites);
                EventHit?.Invoke(Target);
                Destroy(gameObject);
            });
        });
    }
}
