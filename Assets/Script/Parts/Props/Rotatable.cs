using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rotatable : MonoBehaviour
{
    [SerializeField] ObjectAxis ObjectDireaction = ObjectAxis.Square;

    private BaseObject mBaseObject = null;
    private Coroutine mCoroutine = null;
    private Transform mPreviousParent = null;
    private GameObject mCenterObj = null;
    private GameObject mHitObj = null;

    void Start() 
    {
        mBaseObject = this.GetBaseObject();
    }

    // 캐릭터 객체에서 이 함수를 호출해줌
    public void OnPushed(Vector3 forceDir)
    {
        if(mCoroutine == null)
        {
            if(IsRotatable(forceDir) && ObjectDireaction != ObjectAxis.Horizon)
            {
                Bounds bounds = mBaseObject.Body.Collider.GetWorldBounds2D();

                // 물체 회전의 중심이 되는 임시 객체 생성
                mCenterObj = new GameObject();
                mCenterObj.transform.position = (forceDir.x > 0) ? bounds.RightBottom() : bounds.LeftBottom();
                mPreviousParent = mBaseObject.transform.parent;
                mBaseObject.transform.SetParent(mCenterObj.transform);

                // 물체 회전시 물체에 닿는 모든 객체에 타격을 위히기 위한 중심 위치 객체 임시 생성
                mHitObj = new GameObject();
                mHitObj.transform.position = (forceDir.x > 0) ? bounds.RightTop() : bounds.LeftTop();
                mHitObj.transform.SetParent(mBaseObject.transform);

                if(ObjectDireaction == ObjectAxis.Vertical)
                    ObjectDireaction = ObjectAxis.Horizon;

                mCoroutine = StartCoroutine(CoStartRotate(forceDir));
            }
        }
    }

    IEnumerator CoStartRotate(Vector3 forceDir)
    {
        mBaseObject.CharacterPhy.GravityLock = true;
        Vector3 targetRot = (forceDir.x > 0) ? new Vector3(0, 0, -90) : new Vector3(0, 0, 90);
        mCenterObj.transform.DOLocalRotate(targetRot, 0.3f, RotateMode.FastBeyond360).SetEase(Ease.InQuad);

        float time = 0;
        while(time < 0.3f)
        {
            Attack();
            time += Time.deltaTime;
            yield return null;
        }

        mBaseObject.transform.SetParent(mPreviousParent);
        mPreviousParent = null;
        mCoroutine = null;
        Destroy(mCenterObj);
        mCenterObj = null;
        Destroy(mHitObj);
        mHitObj = null;
        mBaseObject.CharacterPhy.GravityLock = false;
    }

    void OnDisable() 
    {
        if(mCenterObj != null)
        {
            Destroy(mCenterObj);
            mCenterObj = null;
        }

        if (mHitObj != null)
        {
            Destroy(mHitObj);
            mHitObj = null;
        }
    }

    // 미는 반대쪽에 방해물이 없어야 물체가 돌아갈수 있기 때문에 반대편에 다른 장애물이 없는지 확인
    private bool IsRotatable(Vector3 forceDir)
    {
        return !Physics.Raycast(mBaseObject.Body.Center, forceDir, Consts.BlockSize, 1 << LayerID.Platforms);
    }

    private void Attack()
    {
        Collider[] cols = Physics.OverlapSphere(mCenterObj.transform.position, Consts.BlockSize * 0.5f, 1 << LayerID.Enemies);
        foreach(Collider col in cols)
        {
            IDamagable target = col.GetDamagableObject();
            if (target != null)
            {
                col.GetBaseObject().Health.LastHitPoint = mHitObj.transform.position;
                target.OnDamaged(100, mBaseObject);
            }
        }
    }
}
