using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    [SerializeField] float _MoveSpeed = 5;
    [SerializeField] int _PassCount = 3;

    public Action<BaseObject> EventHit { get; set; } = null;

    float moveSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoStartGuideMoving());
    }

    void Update()
    {
        // 초반에 속도가 증가하며 항상 right 방향으로 직진하는 코드
        if(moveSpeed < _MoveSpeed)
            moveSpeed += Time.deltaTime;

        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

    private IEnumerator CoStartGuideMoving()
    {
        // 처음 쏘고 1초동안은 그냥 직진....
        // 1초 기달 후
        yield return new WaitForSeconds(1);

        float eclipseTime = 0;
        while(eclipseTime < 1)
        {
            // 주변 적 타겟 감지
            Collider[] cols = InGameUtils.DetectAround(transform.position, 1, 1 << LayerID.Enemies);
            if(cols.Length > 0)
            {
                eclipseTime = 0;
                BaseObject target = cols[0].GetBaseObject();
                int hitCount = 0;
                while(true)
                {
                    // 타겟을 향해 방향전환 후 이동
                    Vector3 dir = target.Body.Center - transform.position;
                    dir.z = 0;
                    dir.Normalize();
                    transform.right = dir;
                    yield return new WaitUntil(() => Vector3.Dot(dir, (target.Body.Center - transform.position)) < 0);

                    // 타격을 입힘
                    EventHit?.Invoke(target);
                    hitCount++;

                    // 일정 관통횟수 이상 통과시 바로 파괴
                    if(_PassCount <= hitCount)
                    {
                        DestroyMissile();
                        yield break;
                    }

                    // 1초 기달
                    yield return new WaitForSeconds(0.5f);

                    // if 타겟이 죽었다면 break
                    if(target == null || target.Health.IsDead)
                        break;
                }
            }

            yield return null;
            eclipseTime += Time.deltaTime;
        }

        DestroyMissile();

    }

    void DestroyMissile()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

}
