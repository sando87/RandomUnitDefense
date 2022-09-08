﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineBasic : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField] float _BuffRange = 3.0f;
    float BuffRange { get { return _BuffRange * mBaseObj.BuffProp.SkillRange; } }

    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private BuffBase BuffEffect = null;

    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
        StartCoroutine(RepeatBuff());
    }

    private void OnAttack(int idx)
    {   
        BaseObject target = mMotionAttack.Target;
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);

        Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
        GameObject obj = Instantiate(HitParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(BuffRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                col.GetBaseObject().BuffCtrl.ApplyBuff(BuffEffect);
        }
    }



}
