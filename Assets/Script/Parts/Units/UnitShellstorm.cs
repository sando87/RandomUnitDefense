using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitShellstorm : UnitPlayer
{
    [SerializeField] MotionActionSingle _MotionMissileAttack = null;
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;

    [SerializeField] MotionActionSingle _SkillAttack = null;
    [SerializeField] float _SkillSpeed = 0.3f;
    [SerializeField] float _SkillRange = 1;
    [SerializeField] float _SplshRange = 1;
    [SerializeField] [Range(0, 10)] float _SkillDamageRate = 3.0f;

    [SerializeField] MissileTracing MissilePrefab = null;
    [SerializeField] ThrowingOver BarrelPrefab = null;
    [SerializeField] GameObject GasBarrelDecalPrefab = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillSpeed { get { return _SkillSpeed * mBaseObj.BuffProp.SkillSpeed; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        _MotionMissileAttack.EventFired = OnAttack;
        _SkillAttack.EventFired = OnSkill;
        StartCoroutine(CoMotionSwitcher(_MotionMissileAttack, () => AttackSpeed, () => AttackRange));
        StartCoroutine(CoMotionSwitcher(_SkillAttack, () => SkillSpeed, () => SkillRange));
    }

    private void OnAttack(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.parent.Find("@_" + idx).position;
        BaseObject target = _MotionMissileAttack.Target;
        MissileTracing missile = Instantiate(MissilePrefab, firePosition, Quaternion.identity);
        missile.transform.right = new Vector3(mBaseObj.transform.right.x, 0.5f, 0);
        missile.Target = target;
        missile.EventHit = OnHitMissile;
    }
    void OnHitMissile(BaseObject target)
    {
        if(target != null)
            target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
    }

    private void OnSkill(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        BaseObject target = _SkillAttack.Target;
        ThrowingOver barrel = Instantiate(BarrelPrefab, firePosition, Quaternion.identity);
        barrel.EventHit = OnHitGasBarrel;
        barrel.Launch(target.transform.position);
    }

    void OnHitGasBarrel(Vector3 dest)
    {
        // GameObject decal = Instantiate(GasBarrelDecalPrefab, dest, Quaternion.identity);
        // this.ExDelayedCoroutine(3, () => Destroy(decal));
        
        Collider[] cols = Physics.OverlapSphere(dest, SplshRange, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage * _SkillDamageRate, mBaseObj);
            }
        }
    }
}
