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
        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

    private IEnumerator CoStartGuideMoving()
    {
        // 처음 쏘고 1초동안은 그냥 직진....
        // 1초 기달 후
        transform.DOScale(new Vector3(1.2f, 0.5f, 1), 0.3f);
        // 초반에 속도가 증가하며 항상 right 방향으로 직진하는 코드
        DOTween.To(() => moveSpeed, (_a) => { moveSpeed = _a; }, _MoveSpeed, 0.5f).From(0);
        yield return new WaitForSeconds(0.3f);

        float eclipseTime = 0;
        while(eclipseTime < 1)
        {
            // 주변 적 타겟 감지
            Collider[] cols = InGameUtils.DetectAround(transform.position, 1, 1 << LayerID.Enemies);
            if(cols.Length > 0)
            {
                eclipseTime = 0;
                BaseObject target = cols[0].GetBaseObject();
                while(true)
                {
                    Vector3 newPos = target != null ? target.transform.position : transform.position;
                    Collider[] colsSub = InGameUtils.DetectAround(newPos, 2, 1 << LayerID.Enemies);
                    if(colsSub.Length <= 0)
                        break;

                    target = colsSub[0].GetBaseObject();

                    // 타겟을 향해 방향전환 후 이동
                    Vector2 firstDir = target.Body.Center - transform.position;
                    transform.right = firstDir.normalized;
                    while(true)
                    {
                        Vector2 curDir = target.Body.Center - transform.position;
                        
                        if(Vector2.Dot(firstDir, curDir) < 0)
                            break;

                        if(curDir.magnitude > 0.5f)
                            transform.right = curDir.normalized;

                        yield return null;
                    }

                    // 타격을 입힘
                    EventHit?.Invoke(target);
                    // hitCount++;

                    // // 일정 관통횟수 이상 통과시 바로 파괴
                    // if(_PassCount <= hitCount)
                    // {
                    //     DestroyMissile();
                    //     yield break;
                    // }

                    // 1초 기달
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0.15f, 0.25f));

                    // if 타겟이 죽었다면 break
                    // if(target == null || target.Health.IsDead)
                    //     break;
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
