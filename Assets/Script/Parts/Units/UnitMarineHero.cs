using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineHero : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField][Range(0, 1)] float _SkillCastRate = 0.2f;
    float SkillCastRate { get { return _SkillCastRate * mBaseObj.BuffProp.Percentage; } }
    [SerializeField] float _SlowDuration = 3.0f;
    float SlowDuration { get { return _SlowDuration * mBaseObj.BuffProp.SkillDuration; } }
    [SerializeField][Range(0, 100)] int _MoveSlowness = 20;

    [SerializeField] private GameObject BulletSparkPrefab = null;
    [SerializeField] private MagicGun MagicGunMissile = null;

    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
    }

    private void OnAttack(int idx)
    {
        BaseObject target = mMotionAttack.Target;
        // 20% 확률로 스킬샷 발동 부분
        float rate = Mathf.Clamp(SkillCastRate, 0, 1);
        bool isSkillShoot = UnityEngine.Random.Range(0, 1.0f) < rate;
        if (isSkillShoot && idx == 0)
            ShootMagicGun(target);
        else
            ShootSimpleGun(target);
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
        missile.EventHit = OnHitMissile;
        missile.Launch();
    }
    private void OnHitMissile(Vector3 dest)
    {
        Collider[] cols = InGameUtils.DetectAround(dest, 0.1f, 1 << LayerID.Enemies);
        foreach(Collider col in cols)
        {
            col.GetBaseObject().Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
        }
    }

}
