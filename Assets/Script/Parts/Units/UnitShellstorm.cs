using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitShellstorm : UnitBase
{
    [SerializeField] MotionActionSingle _MotionMissileAttack = null;
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }

    [SerializeField] MotionActionSingle _SkillAttack = null;
    [SerializeField] float _Cooltime = 3;
    float Cooltime { get { return _Cooltime * mBaseObj.BuffProp.Cooltime; } }
    [SerializeField] float _SkillRange = 1;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] float _SplshRange = 1;
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }

    [SerializeField] SimpleMissile SimpleMissilePrefab = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        _MotionMissileAttack.EventFired = OnAttack;
        _SkillAttack.EventFired = OnSkill;
        StartCoroutine(CoMotionSwitcher(_MotionMissileAttack, 1 / AttackSpeed, AttackRange));
        StartCoroutine(CoMotionSwitcher(_SkillAttack, Cooltime, SkillRange));
    }

    private void OnAttack(int idx)
    {
        BaseObject target = _MotionMissileAttack.Target;
        SimpleMissile missile = Instantiate(SimpleMissilePrefab, mBaseObj.Body.Center, Quaternion.identity);
        missile.EventHit = OnHitMissile;
        missile.Launch(target);
    }
    void OnHitMissile(BaseObject target)
    {
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
    }

    private void OnSkill(int idx)
    {
        BaseObject target = _SkillAttack.Target;
        SimpleMissile barrel = Instantiate(SimpleMissilePrefab, mBaseObj.Body.Center, Quaternion.identity);
        barrel.EventHit = OnHitGasBarrel;
        barrel.Launch(target);
    }

    void OnHitGasBarrel(BaseObject target)
    {
        Collider[] cols = Physics.OverlapSphere(target.transform.position, SplshRange, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            }
        }
    }
}
