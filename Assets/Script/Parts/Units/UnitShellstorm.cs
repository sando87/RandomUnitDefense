using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레벨 업에 따른 미사일 및 가스통 발사 개수 증가

public class UnitShellstorm : UnitPlayer
{
    [SerializeField] MotionActionSingle _MotionMissileAttack = null;
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;

    [SerializeField] MotionActionSingle _SkillAttack = null;
    [SerializeField] float _SkillSpeed = 0.3f;
    [SerializeField] float _SkillRange = 1;
    [SerializeField] float _SplshRange = 1;
    [SerializeField] float _SkillDuration = 3;
    [SerializeField] [Range(0, 10)] float _SkillDamageRate = 3.0f;

    [SerializeField] MissileTracing MissilePrefab = null;
    [SerializeField] ThrowingOver BarrelPrefab = null;
    [SerializeField] GameObject GasBarrelDecalPrefab = null;
    [SerializeField] BuffDamagable PoisonDeBuff = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillSpeed { get { return _SkillSpeed * mBaseObj.BuffProp.SkillSpeed; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        _MotionMissileAttack.EventFired = OnAttack;
        _SkillAttack.EventFired = OnSkill;
        StartCoroutine(CoMotionSwitcher(_MotionMissileAttack, () => AttackSpeed, () => AttackRange));
        StartCoroutine(CoMotionSwitcher(_SkillAttack, () => SkillSpeed, () => SkillRange));

        LOG.trace(PoisonDeBuff.BuffID);
    }

    private void OnAttack(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.parent.Find("@_" + idx).position;
        BaseObject target = _MotionMissileAttack.Target;
        MissileTracing missile = null;

        List<MissileTracing> missiles = new List<MissileTracing>();
        float damage = mBaseObj.SpecProp.Damage;
        for(int i = 0; i < 4; ++i)
        {
            this.ExDelayedCoroutine(0.05f * i, () => 
            {
                firePosition = MyUtils.Random(firePosition, 0.2f);
                missile = Instantiate(MissilePrefab, firePosition, Quaternion.identity);
                missiles.Add(missile);
                missile.transform.right = new Vector3(mBaseObj.transform.right.x, UnityEngine.Random.Range(0.5f, 1.2f), 0);
                missile.Target = target;
                missile.EventHit = (pos) => 
                {
                    Collider[] cols = InGameUtils.DetectAround(pos, 0.1f, 1 << LayerID.Enemies);
                    foreach(Collider col in cols)
                        col.GetBaseObject().Health.GetDamaged(damage, mBaseObj);
                };

            });
        }

        // this.ExDelayedCoroutine(0.7f, () => 
        // {
        //     foreach(MissileTracing mis in missiles)
        //         mis.IsAttackable = true;
        // });
    }


    private void OnSkill(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        BaseObject target = _SkillAttack.Target;
        ThrowingOver barrel = Instantiate(BarrelPrefab, firePosition, Quaternion.identity);
        barrel.Launch(target.transform.position);
        float damage = mBaseObj.SpecProp.Damage * _SkillDamageRate;
        float skillDuration = SkillDuration;
        float range = SplshRange;
        barrel.EventHit = (dest) =>
        {
            Coroutine posioning = this.ExRepeatCoroutine(0.1f, () => DoPoisonAround(dest, range));
            
            GameObject decal = Instantiate(GasBarrelDecalPrefab, dest, Quaternion.identity);
            this.ExDelayedCoroutine(skillDuration, () => { Destroy(decal); StopCoroutine(posioning); });

            Collider[] cols = Physics.OverlapSphere(dest, range, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    hp.GetDamaged(damage, mBaseObj);
                }
            }
        };
    }

    private void DoPoisonAround(Vector3 pos, float range)
    {
        Collider[] cols = Physics.OverlapSphere(pos, range, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            BaseObject target = col.GetBaseObject();
            if(target.BuffCtrl != null)
            {
                target.BuffCtrl.ApplyBuff(PoisonDeBuff, SkillDuration);
            }
        }

    }
}
