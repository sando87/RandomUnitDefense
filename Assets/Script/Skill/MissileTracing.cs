using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MissileTracing : MonoBehaviour
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    [SerializeField]
    public BaseObject Target = null;
    public Action<BaseObject> EventHit { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoStartMoving());
    }

    void Update()
    {
        transform.position += transform.right * 3 * Time.deltaTime;
    }

    private IEnumerator CoStartMoving()
    {
        SpritesAnimator.Play(transform.position, IntroSprites);

        yield return new WaitForSeconds(2);

        StartCoroutine(MyUtils.CoRotateTowards2DLerp(transform, Target.transform, 1000));


        yield return new WaitUntil(() => Vector2.Distance(transform.position, Target.transform.position) < 0.1f);

        SpritesAnimator.Play(transform.position, OutroSprites);
        EventHit?.Invoke(Target);
        Destroy(gameObject);
    }

}
