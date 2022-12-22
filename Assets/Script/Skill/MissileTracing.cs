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
    public Action<Vector3> EventHit { get; set; } = null;
    public bool IsAttackable = false;

    float mMoveSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoStartMoving());
    }

    void Update()
    {
        transform.position += transform.right * mMoveSpeed * Time.deltaTime;
    }

    private IEnumerator CoStartMoving()
    {
        SpritesAnimator.Play(transform.position, IntroSprites);

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.6f, 0.8f));
        //yield return new WaitUntil(() => IsAttackable);

        Vector3 offset = MyUtils.Random(Vector3.zero, 0.3f);
        StartCoroutine(MyUtils.CoRotateTowards2DLerp(transform, Target.Body.transform, 3.14f * 2, offset));
        
        Vector3 lastDest = Target.Body.Center + offset;

        while(transform.position.y > lastDest.y)
        {
            if(Target != null)
                lastDest = Target.Body.Center + offset;

            yield return null;
            mMoveSpeed += (10 * Time.deltaTime);
        }

        SpritesAnimator.Play(transform.position, OutroSprites);
        EventHit?.Invoke(lastDest); // 위치 주변 적을 타격입히는방식으로 변경 필요...

        GetComponent<SpriteRenderer>().enabled = false;
        StopAllCoroutines();
        enabled = false;
        Destroy(gameObject, 1);
    }

}
