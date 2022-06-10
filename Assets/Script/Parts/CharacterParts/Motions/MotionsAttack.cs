using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionsAttack : MotionBasic
{
    [SerializeField] CharacterInputType InputType = CharacterInputType.AttackNormal;
    [SerializeField] float HitPointRate = 0.15f;
    [SerializeField] int AttackDamage = 10;
    [SerializeField] float AttackDelay = 0.5f;
    [SerializeField] Collider AttackCollider = null;

    private CharacterPhysics mCharPhysics = null;
    private Coroutine mCoAttack = null;
    private long mLastAttackTicks = 0;

    public override void OnInitMotion()
    {
        base.OnInitMotion();

        mCharPhysics = mBaseObject.CharacterPhy;
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
            IDamagable target = col.GetDamagableObject();
            if (target != null)
            {
                col.GetBaseObject().Health.LastHitPoint = mBaseObject.transform.position;
                target.OnDamaged(AttackDamage, mBaseObject);
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

    public override void OnExitMotion()
    {
        base.OnExitMotion();

        mAnim.SetLayerWeight(1, 0);
        mCoAttack = null;
    }

}