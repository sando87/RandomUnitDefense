using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴퓨터 인공지능 알고리즘에 기반하여 캐릭터(적몬스터)의 입력을 제어한다.
public class AIFlyingBotInput : CharacterInput
{
    [SerializeField] float DetectRange = 5.0f;
    [SerializeField] float AttackRange = 3.0f;
    [SerializeField] float AttackDelay = 3.0f;

    private float mHorizontal { set { SetInput(CharacterInputType.MoveHori, value); } }
    private bool mKeyDownJump { set { SetInput(CharacterInputType.Jump, value ? 1 : 0); } }
    private bool mKeyDownAttack1 { set { SetInput(CharacterInputType.AttackNormal, value ? 1 : 0); } }
    private bool mKeyDownAttack2 { set { SetInput(CharacterInputType.AttackMelee, value ? 1 : 0); } }
    private bool mKeyDownDetected { set { SetInput(CharacterInputType.Detected, value ? 1 : 0); } }
    private bool mKeyDownMoveFlying { set { SetInput(CharacterInputType.MoveFlying, value ? 1 : 0); } }

    private BaseObject mBaseObject = null;
    private Vector3 mStartPosition = Vector3.zero;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        mStartPosition = mBaseObject.Body.Center;

        StartCoroutine(StartPatrolMode());
    }

    IEnumerator StartPatrolMode()
    {
        // 패트롤 모드가 시작되면 처음 시작 위치로 이동
        TargetDetected = null;
        mKeyDownMoveFlying = true;
        MoveDir = (mStartPosition - mBaseObject.Body.Center).normalized;
        yield return new WaitUntil(() => (mStartPosition - mBaseObject.Body.Center).sqrMagnitude < 0.1f);

        float time = 0;
        mKeyDownMoveFlying = false;
        MoveDir = Vector3.zero;
        while(true)
        {
            BaseObject target = DetectPlayer();
            if(target != null)
            {
                mKeyDownMoveFlying = false;
                mKeyDownDetected = true;
                TargetDetected = target;
                yield return new WaitForSeconds(1);
                mKeyDownDetected = false;

                if(IsAttackable(target))
                {
                    StopAllCoroutines();
                    StartCoroutine(StartAttackMode());
                    yield break;
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartChaseMode());
                    yield break;
                }
            }
            else
            {
                // 시간에 따라 일정주기로 0 -> 1 -> 0 -> -1 의 입력을 준다(즉 섰다가 오른쪽이동 다시 섰다가 왼쪽이동을 반복적으로 수행)
                int moveState = ((int)(time / 3.0f) % 4);
                int horiInput = (moveState % 2 == 0) ? 0 : (moveState - 2);
                mKeyDownMoveFlying = (horiInput != 0);
                MoveDir = new Vector3(horiInput, 0, 0);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator StartChaseMode()
    {
        mKeyDownMoveFlying = false;
        while (true)
        {
            BaseObject target = DetectPlayer();
            if (target != null)
            {
                TargetDetected = target;
                if (IsAttackable(target))
                {
                    MoveDir = Vector3.zero;
                    mKeyDownMoveFlying = false;
                    StopAllCoroutines();
                    StartCoroutine(StartAttackMode());
                    yield break;
                }
                else
                {
                    // 감지된 상대에게 바로 이동하는(대각선이동) 입력을 준다
                    mKeyDownMoveFlying = true;
                    MoveDir = (target.Body.Center - mBaseObject.Body.Center).normalized;
                }
            }
            else
            {
                TargetDetected = null;
                MoveDir = Vector3.zero;
                mKeyDownMoveFlying = false;
                StopAllCoroutines();
                StartCoroutine(StartPatrolMode());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator StartAttackMode()
    {
        mKeyDownMoveFlying = false;
        while (true)
        {
            BaseObject target = DetectPlayer();
            if(target != null)
            {
                TargetDetected = target;
                if(IsAttackable(target))
                {
                    mKeyDownMoveFlying = true;
                    MoveDir = (target.Body.Center - mBaseObject.Body.Center).normalized;
                    yield return new WaitForSeconds(0.1f);
                    mKeyDownMoveFlying = false;
                    mKeyDownAttack1 = true;
                    yield return null;
                    mKeyDownAttack1 = false;
                    yield return new WaitForSeconds(AttackDelay);
                }
                else
                {
                    mKeyDownMoveFlying = false;
                    mKeyDownAttack1 = false;
                    StopAllCoroutines();
                    StartCoroutine(StartChaseMode());
                    yield break;
                }
            }
            else
            {
                mKeyDownMoveFlying = false;
                TargetDetected = null;
                mKeyDownAttack1 = false;
                StopAllCoroutines();
                StartCoroutine(StartPatrolMode());
                yield break;
            }
        }
    }

    private BaseObject DetectPlayer()
    {
        Collider[] cols = Physics.OverlapSphere(mBaseObject.Body.Center, DetectRange, 1 << LayerID.Player);
        foreach(Collider col in cols)
        {
            int layerMask = 1 << LayerID.Platforms | 1 << LayerID.Player;
            Vector3 dir = (col.GetBaseObject().Body.Center - mBaseObject.Body.Center).normalized;
            if (Physics.Raycast(mBaseObject.Body.Center, dir, out RaycastHit hit, DetectRange, layerMask))
            {
                if (hit.collider.GetBaseObject().gameObject.layer == LayerID.Player)
                {
                    return hit.collider.GetBaseObject();
                }
            }
        }

        return null;
    }

    private bool IsAttackable(BaseObject target)
    {
        if((target.Body.Center - mBaseObject.Body.Center).magnitude > AttackRange)
            return false;

        int layerMask = 1 << LayerID.Platforms | 1 << LayerID.Player;
        Vector3 dir = (target.Body.Center - mBaseObject.Body.Center).normalized;
        if (Physics.Raycast(mBaseObject.Body.Center, dir, out RaycastHit hit, AttackRange, layerMask))
        {
            if (hit.collider.GetBaseObject() == target)
            {
                return true;
            }
        }

        return false;
    }

}
