using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnDoor : MonoBehaviour, IMapEditorObject
{
    [SerializeField] GameObject[] EnemyPrefabList = null; //생성시킬 몬스터 리스트
    [SerializeField] float DetectRadius = 5.0f; //주변의 적 수를 감지할 영역 크기
    [SerializeField] float MaxEnemyCount = 20; //최대 생성 가능한 적 수
    [SerializeField] float AroundEnemyCount = 5; //주변에 유지시킬 적 수

    private bool mIsDestroyed = false;
    private int mRepawnedEnemyCount = 0;
    private BoxCollider mGroundDetector = null;
    private SpriteRenderer mImageClose = null;
    private SpriteRenderer mImageHalf = null;
    private SpriteRenderer mImageOpen = null;

    void Start()
    {
        mGroundDetector = GetComponentInChildren<BoxCollider>();

        Transform imageObj = transform.Find("@Image");
        mImageOpen = imageObj.GetChild(0).GetComponent<SpriteRenderer>();
        mImageHalf = imageObj.GetChild(1).GetComponent<SpriteRenderer>();
        mImageClose = imageObj.GetChild(2).GetComponent<SpriteRenderer>();

        // 리스폰 조건 확인하면서 리스폰 처리하는 루틴 시작
        StartCoroutine(CoRespawnEnemy());
    }

    void Update()
    {
        // 터지는 연출 수행중이면 리턴
        if(mIsDestroyed) return;

        // 바닥이 없거나 최대 몬스터 수이상으로 리스폰 했으면 파괴 처리
        if(IsBrokenGround() || MaxEnemyCount <= mRepawnedEnemyCount)
        {
            StartCoroutine(StartDestroy());
        }
    }

    IEnumerator CoRespawnEnemy()
    {
        while(mRepawnedEnemyCount < MaxEnemyCount)
        {
            // 리스폰 조건 확인(주변에 적이 일정 수 이하일 경우)
            yield return new WaitUntil(() => GetAroundEnemy() < AroundEnemyCount);

            yield return StartCoroutine(CoOpenDoor());

            // 실제 리스폰 처리
            mRepawnedEnemyCount++;
            int idx = UnityEngine.Random.Range(0, EnemyPrefabList.Length);
            Instantiate(EnemyPrefabList[idx], transform.position, Quaternion.identity);

            yield return StartCoroutine(CoCloseDoor());

            // 몬스터 한마리 리스폰되고 다른 리스폰까지의 기달
            yield return new WaitForSeconds(3);
        }
    }

    private int GetAroundEnemy()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, DetectRadius, 1 << LayerID.Enemies);
        return cols.Length;
    }
    private bool IsBrokenGround()
    {
        Collider[] cols = Physics.OverlapBox(mGroundDetector.bounds.center, mGroundDetector.bounds.extents, Quaternion.identity, 1 << LayerID.Platforms);
        return cols.Length <= 0;
    }

    IEnumerator StartDestroy()
    {
        mIsDestroyed = true;
        // 터지는 연출 시작
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    IEnumerator CoOpenDoor()
    {
        mImageClose.gameObject.SetActive(false);
        mImageHalf.gameObject.SetActive(true);
        //mImageOpen.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        mImageClose.gameObject.SetActive(false);
        mImageHalf.gameObject.SetActive(false);
        //mImageOpen.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator CoCloseDoor()
    {
        mImageClose.gameObject.SetActive(false);
        mImageHalf.gameObject.SetActive(true);
        //mImageOpen.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        mImageClose.gameObject.SetActive(true);
        mImageHalf.gameObject.SetActive(false);
        //mImageOpen.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
    }

    public void OnInitMapEditor()
    {
        enabled = false;
    }
}
