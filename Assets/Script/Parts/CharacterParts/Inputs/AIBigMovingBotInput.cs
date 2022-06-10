using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴퓨터 인공지능 알고리즘에 기반하여 캐릭터(적몬스터)의 입력을 제어한다.
public class AIBigMovingBotInput : CharacterInput
{
    [SerializeField] float DetectRange = 5.0f;
    [SerializeField] float AttackRange = 3.0f;
    [SerializeField] float AttackDelay = 3.0f;
    [SerializeField] BoxCollider FootFrontArea = null;

    private float mHorizontal { set { SetInput(CharacterInputType.MoveHori, value); } }
    private bool mKeyDownJump { set { SetInput(CharacterInputType.Jump, value ? 1 : 0); } }
    private bool mKeyDownAttack1 { set { SetInput(CharacterInputType.AttackNormal, value ? 1 : 0); } }
    private bool mKeyDownAttack2 { set { SetInput(CharacterInputType.AttackMelee, value ? 1 : 0); } }
    private bool mKeyDownDetected { set { SetInput(CharacterInputType.Detected, value ? 1 : 0); } }
    private bool mKeyDownTransformed { set { SetInput(CharacterInputType.Transformed, value ? 1 : 0); } }

    private BaseObject mBaseObject = null;
    private bool mIsPatrolMode = false;
    private bool mIsNextAttack2 = false;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        mBaseObject.Health.EventDamaged += OnDamaged;

        StartCoroutine(StartPatrolMode());
    }

    IEnumerator StartPatrolMode()
    {
        mIsPatrolMode = true;
        mBaseObject.Health.IsArmorEnable = true;
        float horizontal = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        mHorizontal = 0;
        yield return new WaitForSeconds(1);
        mHorizontal = horizontal;
        float time = 0;
        while (true)
        {
            if (DetectPlayer(DetectRange, out Collider target))
            {
                mHorizontal = target.transform.position.x < mBaseObject.transform.position.x ? -1 : 1;
                yield return new WaitForSeconds(0.1f);
                mHorizontal = 0;
                mKeyDownDetected = true;
                yield return new WaitForSeconds(1);
                mKeyDownDetected = false;

                if (IsAttackable(target))
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
            else if (!IsAbleToGoForward() || time > 3)
            {
                time = 0;
                mHorizontal = 0;
                yield return new WaitForSeconds(2);
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
        mIsPatrolMode = false;
        mHorizontal = mBaseObject.transform.right.x;
        mBaseObject.Health.IsArmorEnable = true;
        while (true)
        {
            if (DetectPlayer(DetectRange, out Collider target))
            {
                mHorizontal = target.transform.position.x < mBaseObject.transform.position.x ? -1 : 1;
                yield return new WaitForSeconds(0.1f);
                if (IsAttackable(target))
                {
                    mHorizontal = 0;
                    StopAllCoroutines();
                    if(mIsNextAttack2)
                    {
                        StartCoroutine(StartAttackMode2());
                    }
                    else
                    {
                        StartCoroutine(StartAttackMode());
                    }
                    mIsNextAttack2 = !mIsNextAttack2;
                    yield break;
                }
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
        mIsPatrolMode = false;
        mHorizontal = 0;

        mKeyDownTransformed = true;
        mBaseObject.Health.IsArmorEnable = false;
        yield return new WaitForSeconds(0.5f);

        mKeyDownAttack2 = true;
        yield return new WaitForSeconds(0.1f);
        mKeyDownAttack2 = false;
        yield return new WaitForSeconds(AttackDelay);

        mKeyDownTransformed = false;
        mBaseObject.Health.IsArmorEnable = true;

        StopAllCoroutines();
        StartCoroutine(StartChaseMode());
    }

    IEnumerator StartAttackMode2()
    {
        mIsPatrolMode = false;
        mHorizontal = 0;

        mKeyDownDetected = true;
        yield return new WaitForSeconds(0.2f);
        mKeyDownDetected = false;
        yield return new WaitForSeconds(0.2f);
        mKeyDownDetected = true;
        yield return new WaitForSeconds(0.2f);
        mKeyDownDetected = false;
        yield return new WaitForSeconds(0.5f);

        mKeyDownAttack1 = true;
        yield return new WaitForSeconds(0.1f);
        mKeyDownAttack1 = false;
        yield return new WaitForSeconds(AttackDelay);

        mKeyDownAttack1 = false;
        mKeyDownDetected = false;
        StopAllCoroutines();
        StartCoroutine(StartChaseMode());
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
        if (Physics.Raycast(mBaseObject.Body.Center, mBaseObject.transform.right, out RaycastHit hit, dist, 1 << LayerID.Player))
        {
            target = hit.collider;
            return true;
        }
        else if (Physics.Raycast(mBaseObject.Body.Center, -mBaseObject.transform.right, out hit, dist, 1 << LayerID.Player))
        {
            target = hit.collider;
            return true;
        }

        target = null;
        return false;
    }

    private bool IsAttackable(Collider target)
    {
        int layerMask = 1 << LayerID.Platforms | 1 << LayerID.Player;
        if (Physics.Raycast(mBaseObject.Body.Center, mBaseObject.transform.right, out RaycastHit hit, AttackRange, layerMask))
        {
            if (hit.collider.GetBaseObject().gameObject.layer == LayerID.Player)
            {
                return target == hit.collider;
            }
        }
        return false;
    }

    private void OnDamaged(float validDamage, BaseObject attacker)
    {
        if (mIsPatrolMode)
        {
            mIsPatrolMode = false;
            StopAllCoroutines();
            StartCoroutine(StartDamagedMode());
        }
    }
    IEnumerator StartDamagedMode()
    {
        mIsPatrolMode = false;
        BaseObject target = mBaseObject.Health.LastAttacker;
        mHorizontal = target.transform.position.x < mBaseObject.transform.position.x ? -1 : 1;
        yield return new WaitForSeconds(0.1f);
        mHorizontal = 0;
        mKeyDownDetected = true;
        yield return new WaitForSeconds(1);
        mKeyDownDetected = false;

        StopAllCoroutines();
        StartCoroutine(StartChaseMode());
    }

}
