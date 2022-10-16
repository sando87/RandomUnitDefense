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

    [SerializeField] float FireMotionSpeed = 0.7f;

    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private SimpleMissile SimpleMissilePrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillCastRate { get { return _SkillCastRate * mBaseObj.BuffProp.Percentage; } }

    private MotionActionLoop mMotionAttack = null;
    private Coroutine mCoAttack = null;
    private float mFireCount = 2; // 3, 4, 5, 6
    private int mCurFireCount = 0;

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        mFireCount = curLevel + 1;
        if (curLevel <= 1)
        {
            mFireCount = 2;
        }
        else if (curLevel <= 2)
        {
            mFireCount = 3;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 50;
        }
        else if (curLevel <= 3)
        {
            mFireCount = 4;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 250;
        }
        else if (curLevel <= 4)
        {
            mFireCount = 6;
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
            _AttackSpeed *= 0.7f;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 1250;
        }
        else if (curLevel <= 5)
        {
            mFireCount = 8;
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
            FireMotionSpeed = 1;
            _AttackSpeed *= 0.5f;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 6000;
        }
        else if (curLevel <= 6)
        {
            mFireCount = 10;
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
            FireMotionSpeed = 2;
            _AttackSpeed *= 0.2f;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 30000;
        }
        

        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mMotionAttack.EventStart = OnAttackStart;
        mMotionAttack.EventEnd = OnAttackEnd;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));
    }

    private void OnAttackStart(BaseObject target)
    {
        mCurFireCount = 0;
        mMotionAttack.SetActionLoopSpeed(FireMotionSpeed);
        mMotionAttack.AddAnimEvent(0.1f, () => FireOneShot());
        
        if(mCoAttack != null)
            StopCoroutine(mCoAttack);

        mCoAttack = StartCoroutine(CoAttack(target));
    }

    IEnumerator CoAttack(BaseObject _target)
    {
        BaseObject target = _target;

        while (mCurFireCount < mFireCount)
        {
            yield return null;
        }

        float normalTime = (int)mMotionAttack.NormalizedTime + 0.9f;
        yield return new WaitUntil(() => mMotionAttack.NormalizedTime >= normalTime);

        mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
        mCoAttack = null;
    }
    private void FireOneShot()
    {
        mCurFireCount++;
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        SimpleMissile missile = Instantiate(SimpleMissilePrefab, firePosition, Quaternion.identity);
        missile.Launch(mMotionAttack.Target);
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
