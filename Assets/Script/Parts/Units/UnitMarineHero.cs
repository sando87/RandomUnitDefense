using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineHero : UnitPlayer
{
    [SerializeField] float _AttackSPD = 1.0f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField][Range(0, 1)] float _SkillCastRate = 0.2f;
    [SerializeField][Range(0, 10)] float _SkillDamageRate = 2.0f;
    [SerializeField] RuntimeAnimatorController _ACForFast = null;

    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private SimpleMissile SimpleMissilePrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;
    [SerializeField][SFXSelector] string _SFXName = "";
    [SerializeField][PrefabSelector(Consts.VFXPath)] string _VFXName = "";
    
    [SerializeField] TextHitEffect _TextHitVFX = null;

    float AttackSpeed { get { return _AttackSPD * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillCastRate { get { return _SkillCastRate * mBaseObj.BuffProp.Percentage; } }

    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        if (curLevel <= 1)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 8;
            spec.damagesPerUp[0] = 1;
            _AttackSPD = 2.0f;
        }
        else if (curLevel <= 2)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 25;
            spec.damagesPerUp[1] = 14;
            _AttackSPD = 4.0f;
        }
        else if (curLevel <= 3)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 120;
            spec.damagesPerUp[2] = 85;
            _AttackSPD = 8.0f;
        }
        else if (curLevel <= 4)
        {
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 250;
            spec.damagesPerUp[3] = 555;
            _AttackSPD = 10.0f;
        }
        else if (curLevel <= 5)
        {
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 600;
            spec.damagesPerUp[4] = 1220;
            _AttackSPD = 20.0f;
        }
        else if (curLevel <= 6)
        {
            mBaseObj.Animator.runtimeAnimatorController = _ACForFast;
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 30000;
            _AttackSPD = 100.0f;
        }
        
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttackFire;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));
    }

    private void OnAttackFire(int fireIndex)
    {
        FireOneShot();
        SoundPlayManager.Instance.PlaySFX(_SFXName);
        ObjectPooling.Instance.InstantiateVFX(_VFXName, mBaseObj.transform.position, Quaternion.identity).ReturnAfter(1);
    }

    private void FireOneShot()
    {
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

                TextHitEffect textHitVfx = Instantiate(_TextHitVFX, pos, Quaternion.identity);
                textHitVfx.SetText((int)damage);
                Destroy(textHitVfx.gameObject, 1.0f);

                t.Health.GetDamaged(damage, mBaseObj);
            }
        };
    }
    private bool IsOutOfSkillRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (AttackRange * 1.2f);
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
