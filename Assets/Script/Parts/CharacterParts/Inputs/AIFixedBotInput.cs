using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴퓨터 인공지능 알고리즘에 기반하여 캐릭터(적몬스터)의 입력을 제어한다.
public class AIFixedBotInput : CharacterInput
{
    [SerializeField] float AttackRange = 3.0f;
    [SerializeField] float AttackDelay = 3.0f;
    [SerializeField] Transform AimingWeapon = null;

    private float mHorizontal { set { SetInput(CharacterInputType.MoveHori, value); } }
    private bool mKeyDownJump { set { SetInput(CharacterInputType.Jump, value ? 1 : 0); } }
    private bool mKeyDownAttack1 { set { SetInput(CharacterInputType.AttackNormal, value ? 1 : 0); } }
    private bool mKeyDownAttack2 { set { SetInput(CharacterInputType.AttackMelee, value ? 1 : 0); } }

    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        
        StartCoroutine(StartAttackMode());
    }

    IEnumerator StartAttackMode()
    {
        while (true)
        {
            BaseObject target = DetectPlayer();
            if (target != null)
            {
                if(AimingWeapon != null)
                    AimingWeapon.ExLookAtPosition(target.Body.Center);

                if(IsAttackable(target))
                {
                    mKeyDownAttack1 = true;
                    yield return new WaitForSeconds(0.1f);
                    mKeyDownAttack1 = false;
                    yield return new WaitForSeconds(AttackDelay);
                }
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    private BaseObject DetectPlayer()
    {
        Collider[] cols = Physics.OverlapSphere(mBaseObject.Body.Center, AttackDelay, 1 << LayerID.Player);
        foreach (Collider col in cols)
        {
            if (col.GetBaseObject().gameObject.layer != LayerID.Player)
                continue;

            return col.GetBaseObject();
        }

        return null;
    }

    private bool IsAttackable(BaseObject target)
    {
        return (target.Body.Center - mBaseObject.Body.Center).magnitude < AttackRange;
    }
}
