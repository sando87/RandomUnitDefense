using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MissileTracing : MonoBehaviour
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    public BaseObject Target { get; set; } = null;
    public Action<BaseObject> EventHit { get; set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoStartMoving());
    }

    void Update()
    {
        transform.position += transform.right * 5 * Time.deltaTime;
    }

    private IEnumerator CoStartMoving()
    {
        SpritesAnimator.Play(transform.position, IntroSprites);

        this.ExDelayedCoroutine(0.5f, () =>
        {
            StartCoroutine(MyUtils.CoRotateTowards2DLerp(transform, Target.transform, 3.14f * 2));
        });
        

        Vector3 lastDest = Target.transform.position;

        while(Vector2.Distance(transform.position, lastDest) > 0.1f)
        {
            if(Target != null)
                lastDest = Target.transform.position;

            yield return null;
        }

        SpritesAnimator.Play(transform.position, OutroSprites);
        EventHit?.Invoke(Target);
            
        Destroy(gameObject);
    }

}
