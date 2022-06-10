using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴퓨터 인공지능 알고리즘에 기반하여 캐릭터(적몬스터)의 입력을 제어한다.
public class AIillBugInput : CharacterInput
{
    [SerializeField] float DetectRange = 5.0f;
    [SerializeField] float AttackRange = 3.0f;
    [SerializeField] float AttackDelay = 3.0f;
    [SerializeField] BoxCollider FootFrontArea = null;
    [SerializeField] BoxCollider FrontGroundArea = null;

    private float mHorizontal { get { return GetInput(CharacterInputType.MoveHori); } set { SetInput(CharacterInputType.MoveHori, value); } }
    private bool mKeyDownJump { set { SetInput(CharacterInputType.Jump, value ? 1 : 0); } }
    private bool mKeyDownAttack1 { set { SetInput(CharacterInputType.AttackNormal, value ? 1 : 0); } }
    private bool mKeyDownAttack2 { set { SetInput(CharacterInputType.AttackMelee, value ? 1 : 0); } }
    private bool mKeyDownDetected { set { SetInput(CharacterInputType.Detected, value ? 1 : 0); } }

    private BaseObject mBaseObject = null;
    private Collider mTarget = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        
        StartCoroutine(StartPatrolMode());
    }

    IEnumerator StartPatrolMode()
    {
        mTarget = null;

        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = (int)UnityEngine.Random.Range(0, 3) - 1;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));
        mHorizontal = 0;

        while(true)
        {
            if(DetectPlayer(DetectRange, out Collider target))
            {
                mTarget = target;

                StopAllCoroutines();
                StartCoroutine(StartChaseMode());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator StartChaseMode()
    {
        mHorizontal = mBaseObject.transform.right.x;
        float time = 0;
        float interval = 2.0f;
        while (true)
        {
            if (mTarget != null)
            {
                if (IsAttackable())
                {
                    mHorizontal = 0;
                    StopAllCoroutines();
                    StartCoroutine(StartAttackMode());
                    yield break;
                }
                else
                {
                    // if (IsEndOfGround() && IsJumpable())
                    // {
                    //     time = 0;
                    //     interval = UnityEngine.Random.Range(2.0f, 10.0f);
                    //     mKeyDownJump = true;
                    //     yield return null;
                    //     mKeyDownJump = false;
                    // }
                    // else if(IsDroppable())
                    // {
                    //     time = 0;
                    //     interval = UnityEngine.Random.Range(2.0f, 10.0f);
                    //     float hori = mHorizontal;
                    //     mKeyDownJump = true;
                    //     mHorizontal = 0;
                    //     yield return null;
                    //     mKeyDownJump = false;
                    //     yield return new WaitForSeconds(0.1f);
                    //     mHorizontal = -hori;
                    // }
                    // else 
                    if (time > interval)
                    {
                        time = 0;
                        interval = UnityEngine.Random.Range(2.0f, 4.0f);
                        mHorizontal = mTarget.GetBaseObject().transform.position.x < mBaseObject.transform.position.x ? -1 : 1;
                    }
                    else
                    {
                        time += Time.deltaTime;
                        // if (UnityEngine.Random.Range(0, 100) == 0)
                        // {
                        //     mKeyDownJump = true;
                        //     yield return null;
                        //     mKeyDownJump = false;
                        // }
                    }
                }
            }
            else
            {
                mHorizontal = 0;
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
        if(isAttackDelay > 5)
            yield return new WaitForSeconds(isAttackDelay * 0.1f);

        while (true)
        {
            if (mTarget != null)
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

    private bool DetectPlayer(float dist, out Collider target)
    {
        int layerMask = 1 << LayerID.Player;
        Collider[] cols = Physics.OverlapSphere(mBaseObject.Body.Center, dist, layerMask);
        foreach(Collider col in cols)
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

    private bool IsEndOfGround()
    {
        if (FootFrontArea == null)
            return false;

        if (mBaseObject.transform.up.y < 0.5f)
            return false;

        Collider[] cols = Physics.OverlapBox(FootFrontArea.bounds.center, FootFrontArea.bounds.extents, Quaternion.identity, 1 << LayerID.Platforms);
        return cols.Length == 0;
    }

    private bool IsJumpable()
    {
        if (FrontGroundArea == null)
            return false;

        Collider[] cols = Physics.OverlapBox(FrontGroundArea.bounds.center, FrontGroundArea.bounds.extents, Quaternion.identity, 1 << LayerID.Platforms);
        return cols.Length > 0;
    }

    private bool IsDroppable()
    {
        if(mTarget == null)
            return false;

        if (mBaseObject.transform.up.y > -0.5f)
            return false;

        Vector3 dir = mTarget.GetBaseObject().Body.Center - mBaseObject.Body.Center;
        dir.Normalize();
        return Physics.Raycast(mBaseObject.Body.Center, dir, 5, 1 << LayerID.Player);
    }

}
