using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionsAttack : MotionBasic
{
    [SerializeField] InputType InputType = InputType.KeyA;
    [SerializeField] float HitPointRate = 0.15f;
    [SerializeField] int AttackDamage = 10;
    [SerializeField] float AttackDelay = 0.5f;
    [SerializeField] Collider AttackCollider = null;

    private Coroutine mCoAttack = null;
    private long mLastAttackTicks = 0;

    public override void OnInit()
    {
        base.OnInit();
    }

    void Update()
    {
        bool isAttack = mCharacterInput.GetInput(InputType) > 0;
        if (isAttack && mCoAttack == null && MyUtils.IsCooltimeOver(mLastAttackTicks, AttackDelay))
        {
            mAnim.SetLayerWeight(1, 1);
            mCoAttack = StartCoroutine(CoAttack());
        }
    }

    IEnumerator CoAttack()
    {
        yield return new WaitUntil(() => HitPointRate <= NormalizedTime);
        mLastAttackTicks = System.DateTime.Now.Ticks;
        DoSimpleHit();

        yield return new WaitUntil(() => 1.0f <= NormalizedTime);
    }

    void DoSimpleHit()
    {
        // 설정된 Attack 콜라이더와 겹쳐져있는 적을 감지하여 피격 가능한 객체가 있으면 데미지를 입힌다
        int layerMaskAttackable = mBaseObject.GetLayerMaskAttackable();
        Collider[] cols = Physics.OverlapBox(AttackCollider.bounds.center, AttackCollider.bounds.extents, Quaternion.identity, layerMaskAttackable);
        foreach(Collider col in cols)
        {
            Health targetHp = col.GetComponentInBaseObject<Health>();
            if (targetHp != null)
            {
                targetHp.LastHitPoint = mBaseObject.transform.position;
                targetHp.OnDamaged(AttackDamage, mBaseObject);
            }
        }

        // 전방 Range만큼의 범위를 조사하여(Raycast) 피격 가능한 객체가 있으면 데미지를 입힌다
        // RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, -0.5f, 0), transform.right, AttackRange, layerMaskAttackable);
        // foreach(RaycastHit hit in hits)
        // {
        //     IDamagable target = hit.collider.GetDamagableObject();
        //     if (target != null)
        //     {
        //         target.OnDamaged(AttackDamage, mBaseObject);
        //     }
        // }
    }

    public override void OnLeave()
    {
        base.OnLeave();

        mAnim.SetLayerWeight(1, 0);
        mCoAttack = null;
    }

}