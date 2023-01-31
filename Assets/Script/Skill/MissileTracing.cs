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
    public bool StartTracing = false;
    public float TimeToDest = 1;

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

        // yield return new WaitForSeconds(UnityEngine.Random.Range(0.6f, 0.8f));
        yield return new WaitUntil(() => StartTracing);

        Vector3 offset = MyUtils.Random(Vector3.zero, 0.3f);
        Vector3 dest = Target.transform.position + offset;
        StartCoroutine(MyUtils.CoRotateTowards2DLerp(transform, dest, 3.14f * 2));

        yield return new WaitUntil(() => Vector2.Dot((dest - transform.position), transform.right) < 0.1f);
        
        float distanceToDest = (dest - transform.position).magnitude;
        mMoveSpeed = distanceToDest / TimeToDest;

        yield return new WaitUntil(() => transform.position.y <= dest.y);

        SpritesAnimator.Play(transform.position, OutroSprites);
        EventHit?.Invoke(transform.position); // 위치 주변 적을 타격입히는방식으로 변경 필요...

        GetComponent<SpriteRenderer>().enabled = false;
        StopAllCoroutines();
        enabled = false;
        Destroy(gameObject, 1);
    }

}
