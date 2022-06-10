using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MultiBeam : SkillBase
{
    [SerializeField] float FireDuration = 1.0f; // 난사되는 총 시간
    [SerializeField] float FireInterval = 0.1f; // 난사되는 총알 간격
    [SerializeField] int Damage = 1; // 피격시 입힐 데미지
    [SerializeField] float ProjectileMaxDistance = 10; // 투사체 날악가는 거리
    [SerializeField] float ProjectileDuration = 0.4f; // 투사체 날악가는 시간
    [SerializeField] GameObject ProjectilePrefab = null; // 생성할 투사체 프리팹
    [SerializeField][PrefabSelector(Consts.VFXPath)] string IntroVFX = ""; // 투사체 생성 이펙트
    [SerializeField][PrefabSelector(Consts.VFXPath)] string OutroVFX = ""; // 투사체 터질때 이펙트
    [SerializeField][PrefabSelector(Consts.VFXPath)] string HitVFX = ""; // 투사체 타격시 이펙트
    [SerializeField][SFXSelector] string FireSFX = ""; // 투사체 발사시 사운드

    private BaseObject mOwner = null;

    public override void Fire(BaseObject owner)
    {
        base.Fire(owner);

        mOwner = owner;
        StartCoroutine(CoShot());
    }

    IEnumerator CoShot()
    {
        List<GameObject> objs = new List<GameObject>();
        float time = 0;
        while (time < FireDuration)
        {
            SoundPlayManager.Instance.PlayInGameSFX(FireSFX);

            GameObject obj = DoRandomShot();
            objs.Add(obj);

            yield return new WaitForSeconds(FireInterval);
            time += FireInterval;
        }

        foreach (GameObject obj in objs)
        {
            yield return new WaitUntil(() => obj == null);
        }

        Destroy(gameObject);
    }


    // 투사체 발사 및 충돌시 처리까지 모두 이 함수에서 처리(발사 및 피격시 이펙트 처리까지..)
    GameObject DoRandomShot()
    {
        Vector3 firePosition = transform.position;
        // 투사체 객체 생성
        GameObject projectileObj = Instantiate(ProjectilePrefab, firePosition, Quaternion.identity);

        // Shoot 시작 효과 생성 후 몇초뒤 삭제
        if (IntroVFX.Length > 0)
        {
            ObjectPooling.Instance.InstantiateVFX(IntroVFX, firePosition, Quaternion.identity).ReturnAfter(3);
        }

        // 전방 방향으로 발사(이동) 후 종료지점에서 투사체 삭제
        float randomOffX = UnityEngine.Random.Range(-0.2f, 0.2f);
        float randomOffY = UnityEngine.Random.Range(-0.2f, 0.2f);
        Vector3 dest = firePosition + (Direction * ProjectileMaxDistance) + new Vector3(randomOffX, randomOffY, 0);
        Vector3 dir = dest - firePosition;
        projectileObj.transform.right = dir.normalized;
        projectileObj.transform.DOMove(dest, ProjectileDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            EndProjectaile(projectileObj);
        });

        // 지나가는 도중에 특정 타겟과 충돌시 해당 객체에게 타격 처리
        ColliderTriggerProxy trigger = projectileObj.GetComponent<ColliderTriggerProxy>();

        // 투사체의 충돌 대상은 상대편 Layer와 지형Layer로 설정
        trigger.SetLayerMask(mOwner.GetLayerMaskAttackable());

        // 투사체의 충돌시 처리할 함수 등록
        trigger.EventTriggerEnter += (other) =>
        {
            // 피격 객체가 데미지를 입힐 수 있는 개체이면 데미지를 입히고 투사체 삭제
            IDamagable target = other.GetDamagableObject();
            if (target != null)
            {
                other.GetBaseObject().Health.LastHitPoint = projectileObj.transform.position;
                target.OnDamaged(Damage, mOwner);
            }

            EndProjectaile(projectileObj);

            if (HitVFX.Length > 0)
            {
                ObjectPooling.Instance.InstantiateVFX(HitVFX, projectileObj.transform.position, Quaternion.identity).ReturnAfter(3);
            }

        };

        return projectileObj;
    }

    private void EndProjectaile(GameObject projectileObj)
    {
        Vector3 pos = projectileObj.transform.position;
        // 종료 이펙트 연출 후 사라짐
        if (OutroVFX.Length > 0)
        {
            ObjectPooling.Instance.InstantiateVFX(OutroVFX, pos, Quaternion.identity).ReturnAfter(3);
        }

        projectileObj.transform.DOKill();
        Destroy(projectileObj);
    }
}
