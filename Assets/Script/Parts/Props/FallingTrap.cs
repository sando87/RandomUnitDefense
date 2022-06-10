using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 처음 시작시 지면위에 없으면 밑에 플레이어를 감지하여 플레이어 지나갈때 떨어지도록 동작
// 처음 시작시 지면위에 있으면 지면이 사라지면 떨어지도록 동작

public class FallingTrap : MonoBehaviour
{
    private bool mIsOnGround = false;
    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        mBaseObject.CharacterPhy.GravityLock = true;

        if(IsOnTheGround())
        {
            StartCoroutine(CoStartDetectingGroundMode());
        }
        else
        {
            StartCoroutine(CoStartDetectingPlayerMode());
        }
    }

    IEnumerator CoStartDetectingGroundMode()
    {
        yield return new WaitUntil(() => !IsOnTheGround());

        StartToDrop();
    }
    IEnumerator CoStartDetectingPlayerMode()
    {
        yield return new WaitUntil(() => IsPlayerDetected());

        StartToDrop();
    }

    private void OnDamaged(float damage, BaseObject attacker)
    {
        StartToDrop();
    }

    private void DoAttackToCrushedObjects()
    {
        Bounds bounds = mBaseObject.Body.Collider.GetWorldBounds2D();
        int layerMask = 1 << LayerID.Player | 1 << LayerID.Enemies;
        Collider[] cols = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, layerMask);
        foreach (Collider col in cols)
        {
            IDamagable health = col.GetDamagableObject();
            if (health != null)
            {
                health.OnDamaged(1000, this.GetBaseObject());
            }
        }
    }

    private bool IsOnTheGround()
    {
        return Physics.Raycast(mBaseObject.Body.Center, Vector3.down, Consts.BlockSize, 1 << LayerID.Platforms);
    }
    private bool IsPlayerDetected()
    {
        Bounds bounds = mBaseObject.Body.Collider.GetWorldBounds2D();
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(bounds.LeftBottom(), Vector3.down, out hit, Consts.BlockSize * 5, 1 << LayerID.Platforms | 1 << LayerID.Player))
        {
            if(hit.collider.GetBaseObject().gameObject.layer == LayerID.Player)
            {
                return true;
            }
        }
        else if (Physics.Raycast(bounds.RightBottom(), Vector3.down, out hit, Consts.BlockSize * 5, 1 << LayerID.Platforms | 1 << LayerID.Player))
        {
            if (hit.collider.GetBaseObject().gameObject.layer == LayerID.Player)
            {
                return true;
            }
        }
        return false;
    }

    // 떨어지기 전에 흔들리고 약간의 딜레이 후 떨어지는 연출 구현
    private void StartToDrop()
    {
        mBaseObject.Renderer.ShakeObject(0.1f, 0.5f);
        this.ExDelayedCoroutine(0.5f, () =>
        {
            mBaseObject.CharacterPhy.GravityLock = false;
        });
    }

}
