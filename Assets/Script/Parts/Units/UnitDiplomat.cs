using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 피격시 범위 피해
// 레벨업에 따른 개수 증가

public class UnitDiplomat : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] int _PassPercent = 10;
    [SerializeField] int _ProjectileCount = 1;
    
    [SerializeField] BuffBase BuffEffect = null;
    
    [SerializeField] GuidedMissile _Projectaile = null;
    [SerializeField][PrefabSelector(Consts.VFXPath)] string _HitVFX = "";

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    int PassPercent { get { return _PassPercent + mBaseObj.BuffProp.Percentage; } }

    private MotionActionSingle mMotionAttack = null;

    int PassPercentTest { get { return 10 + mBaseObj.SpecProp.Level * 10; } }
    
    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        if (curLevel <= 1)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 8;
            spec.damagesPerUp[0] = 1;
            _PassPercent = PassPercentTest;
            _ProjectileCount = curLevel;
        }
        else if (curLevel <= 2)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 25;
            spec.damagesPerUp[1] = 14;
            _PassPercent = PassPercentTest;
            _ProjectileCount = curLevel;
        }
        else if (curLevel <= 3)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 120;
            spec.damagesPerUp[2] = 85;
            _PassPercent = PassPercentTest;
            _ProjectileCount = curLevel;
        }
        else if (curLevel <= 4)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 250;
            spec.damagesPerUp[3] = 555;
            _PassPercent = PassPercentTest;
            _ProjectileCount = curLevel;
        }
        else if (curLevel <= 5)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 600;
            spec.damagesPerUp[4] = 1220;
            _PassPercent = PassPercentTest;
            _ProjectileCount = curLevel;
        }
        else if (curLevel <= 6)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 30000;
            _PassPercent = PassPercentTest;
            _ProjectileCount = curLevel;
        }

        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));

        // StartCoroutine(KeepBuff(BuffEffect));
    }

    void OnAttack(int idx)
    {
        for(int i = 0; i < _ProjectileCount; ++i)
            ShootProjectail(mMotionAttack.Target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        Vector3 dir = target.Body.Center - firePosition;
        dir.z = 0;

        GuidedMissile projectile = Instantiate(_Projectaile, firePosition, Quaternion.identity);
        projectile.transform.right = dir.normalized;
        projectile.PassPercent = PassPercent;

        float damage = mBaseObj.SpecProp.Damage;

        projectile.EventHit += (target) => 
        {
            // ObjectPooling.Instance.InstantiateVFX(_HitVFX, projectile.transform.position, Quaternion.identity).ReturnAfter(1);
            
            Vector3 force = projectile.transform.right * 7;
            target.Health.GetForced(force, mBaseObj);

            target.Health.GetDamaged(damage, mBaseObj);
        };
    }
}
