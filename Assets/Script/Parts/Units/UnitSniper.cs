using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// animing되는 연출 개선(조준 흔들림, 조준 정렬되는 연출필요)
// 레벨 업에 따른 기본 뎀지 증가, 크리티컬 확률 뎀지 증가, aiming 시간 증가

public class UnitSniper : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 3;
    [SerializeField][Range(0, 1)] float _CriticalPercent = 0.2f;
    [SerializeField][Range(1, 10)] float _CriticalDamageMultiplier = 3.0f;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] private Sprite[] AimingSprites = null;
    [SerializeField] LineRenderer AimmingLine = null;
    [SerializeField] BuffBase BuffEffectPrefab = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float CriticalPercent { get { return _CriticalPercent * mBaseObj.BuffProp.Percentage; } }
    float CriticalDamageMultiplier { get { return _CriticalDamageMultiplier; } }

    private SpritesAnimator mAimingEffect = null;
    private MotionActionLoop mMotionAiming = null;
    private MotionActionSingle mMotionShoot = null;

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        if (curLevel <= 1)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 7;
            spec.damagesPerUp[0] = 1;
        }
        else if (curLevel <= 2)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 35;
            spec.damagesPerUp[1] = 15;
        }
        else if (curLevel <= 3)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 140;
            spec.damagesPerUp[2] = 85;
        }
        else if (curLevel <= 4)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 350;
            spec.damagesPerUp[3] = 1150;
        }
        else if (curLevel <= 5)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 835;
            spec.damagesPerUp[4] = 1460;
        }
        else if (curLevel <= 6)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 204800;
        }

        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAiming = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mMotionAiming.EventStart = OnAttackBeamStart;
        mMotionAiming.EventUpdate = OnAttackBeamUpdate;
        mMotionAiming.EventEnd = OnAttackBeamEnd;

        mMotionShoot = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionShoot.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAiming, () => AttackSpeed, () => AttackRange));

        AimmingLine.positionCount = 2;
        AimmingLine.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        AimmingLine.gameObject.SetActive(false);

        StartCoroutine(KeepBuff(BuffEffectPrefab));
    }

    private void OnAttackBeamStart(BaseObject target)
    {
        if(mAimingEffect != null)
        {
            Destroy(mAimingEffect.gameObject);
            mAimingEffect = null;
        }
        
        AimmingLine.gameObject.SetActive(true);
        
        mAimingEffect = SpritesAnimator.Play(target.Body.Center, AimingSprites, false);
        mAimingEffect.transform.SetParent(target.transform);
        mAimingEffect.EventEnd = () =>
        {
            mMotionShoot.Target = target;
            mBaseObj.MotionManager.SwitchMotion(mMotionShoot);
        };
    }
    private void OnAttackBeamUpdate(BaseObject target)
    {
        Vector3 startPos = mBaseObj.FirePosition.transform.position;
        Vector3 endPos = target.Body.Center;
        AimmingLine.SetPosition(0, startPos);
        AimmingLine.SetPosition(1, endPos);

        if (IsOutOfRange(target))
        {
            mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
        }
    }
    private bool IsOutOfRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (AttackRange * 1.2f);
    }
    private void OnAttackBeamEnd()
    {
        AimmingLine.gameObject.SetActive(false);
        if (mAimingEffect != null)
        {
            Destroy(mAimingEffect.gameObject);
            mAimingEffect = null;
        }
    }

    private void OnAttack(int idx)
    {
        BaseObject target = mMotionAiming.Target;
        ShootProjectail(target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        LaserAimming laserObj = LaserAimming.Play(firePosition, target.Body.gameObject, "SniperLaser");
        this.ExDelayedCoroutine(1, () => Destroy(laserObj.gameObject));

        SpritesAnimator.Play(target.transform.position, OutroSprites);

        float damage = mBaseObj.SpecProp.Damage;
        int percent = (int)(CriticalPercent * 100.0f);
        if (UnityEngine.Random.Range(0, 100) < percent)
        {
            damage *= CriticalDamageMultiplier;
        }
        
        Collider[] cols = Physics.OverlapSphere(target.Body.Center, 0.25f, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(damage, mBaseObj);
            }
        }
    }
}
