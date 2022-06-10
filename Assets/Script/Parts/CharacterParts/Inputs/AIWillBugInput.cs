using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴퓨터 인공지능 알고리즘에 기반하여 캐릭터(적몬스터)의 입력을 제어한다.
public class AIWillBugInput : CharacterInput
{
    [SerializeField] float DetectRange = 5.0f;
    [SerializeField] float AttackRange = 3.0f;
    [SerializeField] float AttackDelay = 3.0f;
    [SerializeField] BoxCollider FootFrontArea = null;

    private float mHorizontal { set { SetInput(CharacterInputType.MoveHori, value); } }
    private bool mKeyDownJump { set { SetInput(CharacterInputType.Jump, value ? 1 : 0); } }
    private bool mKeyDownAttack1 { set { SetInput(CharacterInputType.AttackNormal, value ? 1 : 0); } }
    private bool mKeyDownDetected { set { SetInput(CharacterInputType.Detected, value ? 1 : 0); } }

    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();

        StartCoroutine(StartPatrolMode());
    }

    IEnumerator StartPatrolMode()
    {
        float horizontal = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = horizontal;

        float time = 0;
        float interval = 3;
        while (true)
        {
            if (DetectPlayer(DetectRange, out Collider target))
            {
                mHorizontal = 0;
                mKeyDownDetected = true;
                yield return new WaitForSeconds(0.1f);
                mKeyDownDetected = false;

                if (IsAttackable())
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
            else if (!IsAbleToGoForward() || time > interval)
            {
                time = 0;
                mHorizontal = 0;
                yield return new WaitForSeconds(0.2f);
                horizontal *= -1;
                mHorizontal = horizontal;
            }
            else
            {
                time += Time.deltaTime;
                mHorizontal = horizontal;
            }

            yield return null;
        }
    }

    IEnumerator StartChaseMode()
    {
        mHorizontal = mBaseObject.transform.right.x;
        while (true)
        {
            if (DetectPlayer(DetectRange, out Collider target))
            {
                if (IsAttackable())
                {
                    mHorizontal = 0;
                    StopAllCoroutines();
                    StartCoroutine(StartAttackMode());
                    yield break;
                }
                // else if (!IsAbleToGoForward())
                // {
                //     mKeyDownJump = true;
                //     yield return new WaitForSeconds(0.2f);
                //     mKeyDownJump = false;
                // }
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(StartPatrolMode());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator StartAttackMode()
    {
        mHorizontal = 0;
        int isAttackDelay = UnityEngine.Random.Range(0, 15);
        if (isAttackDelay > 5)
            yield return new WaitForSeconds(isAttackDelay * 0.1f);

        while (true)
        {
            if (DetectPlayer(DetectRange, out Collider target))
            {
                if (IsAttackable())
                {
                    mKeyDownAttack1 = true;
                    yield return new WaitForSeconds(0.1f);
                    mKeyDownAttack1 = false;
                    yield return new WaitForSeconds(AttackDelay);
                }
                else
                {
                    mKeyDownAttack1 = false;
                    StopAllCoroutines();
                    StartCoroutine(StartChaseMode());
                    yield break;
                }
            }
            else
            {
                mKeyDownAttack1 = false;
                StopAllCoroutines();
                StartCoroutine(StartPatrolMode());
                yield break;
            }
        }
    }

    private bool IsAbleToGoForward()
    {
        if (mBaseObject.Body.IsObstacled)
            return false;

        if (FootFrontArea == null)
            return true;

        Collider[] cols = Physics.OverlapBox(FootFrontArea.bounds.center, FootFrontArea.bounds.extents, Quaternion.identity, 1 << LayerID.Platforms);
        return cols.Length > 0;
    }

    private bool DetectPlayer(float dist, out Collider target)
    {
        int layerMask = 1 << LayerID.Player;
        Collider[] cols = Physics.OverlapSphere(mBaseObject.Body.Center, dist, layerMask);
        foreach (Collider col in cols)
        {
            target = col;
            return true;
        }

        target = null;
        return false;
    }

    private bool IsAttackable()
    {
        int layerMask = 1 << LayerID.Player;
        if (Physics.Raycast(mBaseObject.Body.Center, mBaseObject.transform.right, AttackRange, layerMask))
        {
            return true;
        }
        return false;
    }

}
