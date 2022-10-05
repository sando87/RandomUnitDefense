using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineHero : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField][Range(0, 1)] float _SkillCastRate = 0.2f;
    [SerializeField][Range(0, 10)] float _SkillDamageRate = 2.0f;
    [SerializeField] RuntimeAnimatorController _ACForFast = null;

    [SerializeField] float _FireSpeed = 1.0f;

    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private SimpleMissile SimpleMissilePrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillCastRate { get { return _SkillCastRate * mBaseObj.BuffProp.Percentage; } }

    private MotionActionLoop mMotionAttack = null;
    private Coroutine mCoAttack = null;
    private float mFireCount = 2; // 3, 4, 5, 6
    private float mFireInterval = 0.125f; // 0.125f, 0.125f, 0.0833f, 0.0833f

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        mFireCount = curLevel + 1;
        if(curLevel >= 4)
        {
            mFireInterval = 0.0833f;
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
        }

        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mMotionAttack.EventStart = OnAttackStart;
        mMotionAttack.EventEnd = OnAttackEnd;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));
    }

    private void OnAttackStart(BaseObject target)
    {
        mMotionAttack.SetActionLoopSpeed(_FireSpeed);
        if(mCoAttack != null) StopCoroutine(mCoAttack);
        mCoAttack = StartCoroutine(CoAttack(target));
    }

    IEnumerator CoAttack(BaseObject _target)
    {
        BaseObject target = _target;

        int count = 0;
        while (!IsOutOfSkillRange(target) && count < mFireCount)
        {
            Vector3 firePosition = mBaseObj.FirePosition.transform.position;
            SimpleMissile missile = Instantiate(SimpleMissilePrefab, firePosition, Quaternion.identity);
            missile.Launch(target);
            float damage = mBaseObj.SpecProp.Damage;
            missile.EventHit = (t) => 
            {
                if(t != null)
                {
                    Vector3 pos = MyUtils.Random(t.Body.Center, 0.2f);
                    GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
                    Destroy(obj, 1.0f);

                    t.Health.GetDamaged(damage, mBaseObj);
                }
            };

            yield return newWaitForSeconds.Cache(mFireInterval);
            count++;
        }

        mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
        mCoAttack = null;
    }
    private bool IsOutOfSkillRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (AttackRange * 1.2f);
    }

    private void OnAttackEnd()
    {
        if(mCoAttack != null)
        {
            StopCoroutine(mCoAttack);
            mCoAttack = null;
        }
    }

    private void ShootSimpleGun(BaseObject target)
    {
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
        Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
        GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity, target.transform);
        Destroy(obj, 1.0f);
    }
    private void ShootMagicGun(BaseObject target)
    {
        MagicGun missile = Instantiate(MagicGunMissile, mBaseObj.FirePosition.transform.position, Quaternion.identity);
        missile.Target = target.transform;
        missile.Launch();
        float damage = mBaseObj.SpecProp.Damage * _SkillDamageRate;
        missile.EventHit = (dest) =>
        {
            Collider[] cols = InGameUtils.DetectAround(dest, 0.1f, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                col.GetBaseObject().Health.GetDamaged(damage, mBaseObj);
            }
        };
    }
}
