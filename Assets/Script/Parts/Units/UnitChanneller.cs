using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레이저로 누적데미지를 입힌다.

public class UnitChanneller : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 1;
    [SerializeField] float _AttackRange = 2;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }

    private LaserAimming mLaserEffectObject = null;
    private MotionActionLoop mLaserMotion = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        int curLevel = mBaseObj.SpecProp.Level;
        BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        spec.damage = InGameSystem.Instance.SaveTestInfo.units[4].damages[curLevel - 1];
        spec.damagesPerUp[curLevel - 1] = InGameSystem.Instance.SaveTestInfo.units[4].damagesPerUp[curLevel - 1];

        // if (curLevel <= 1)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 7;
        //     spec.damagesPerUp[0] = 1;
        // }
        // else if (curLevel <= 2)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 35;
        //     spec.damagesPerUp[1] = 20;
        // }
        // else if (curLevel <= 3)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 140;
        //     spec.damagesPerUp[2] = 125;
        // }
        // else if (curLevel <= 4)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 350;
        //     spec.damagesPerUp[3] = 920;
        // }
        // else if (curLevel <= 5)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 835;
        //     spec.damagesPerUp[4] = 2360;
        // }
        // else if (curLevel <= 6)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 204800;
        // }

        mLaserMotion = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mLaserMotion.EventStart = OnAttackBeamStart;
        mLaserMotion.EventEnd = OnAttackBeamEnd;
        StartCoroutine(CoMotionSwitcher(mLaserMotion, () => AttackSpeed, () => AttackRange));
    }

    private void OnAttackBeamStart(BaseObject target)
    {
        if(mLaserEffectObject != null)
        {
            Destroy(mLaserEffectObject.gameObject);
            mLaserEffectObject = null;
        }
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        mLaserEffectObject = LaserAimming.Play(firePosition, target.Body.gameObject, "ChannellerLaser");
        mLaserEffectObject.transform.SetParent(mBaseObj.FirePosition.transform);
        StartCoroutine("CoAttackBeam");
    }
    IEnumerator CoAttackBeam()
    {
        BaseObject target = mLaserMotion.Target;
        StartCoroutine(CoAttackBeamSub(target, mLaserEffectObject, 3));
        
        float timeToMaxDamage = 3;
        float hitInterval = 0.1f;
        float currentDamageRate = 0;
        float currentTime = 0;
        float currentDamagePerSec = 0;
        while (!IsOutOfAttackRange(target))
        {
            Collider[] cols = Physics.OverlapSphere(target.transform.position, 0.2f, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    float dotDamage = currentDamagePerSec * hitInterval;
                    hp.GetDamaged(dotDamage, mBaseObj);
                }
            }

            yield return newWaitForSeconds.Cache(hitInterval);
            currentTime += hitInterval;
            currentDamageRate = Mathf.Min(1, currentTime / timeToMaxDamage);
            currentDamagePerSec = mBaseObj.SpecProp.Damage * currentDamageRate;

            if(target == null || target.Health.IsDead)
                break;
        }
        mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
    }
    IEnumerator CoAttackBeamSub(BaseObject startTarget, LaserAimming parentLaser, int count)
    {
        Collider[] nextCols = startTarget.DetectAround(1.5f, 1 << LayerID.Enemies);
        if(nextCols.Length <= 1 || count <= 0)
            yield break;

        BaseObject target = nextCols[nextCols.Length - 1].GetBaseObject();
        
        Vector3 firePosition = startTarget.Body.Center;
        LaserAimming laserEffectObject = LaserAimming.Play(firePosition, target.Body.gameObject, "ChannellerLaser");
        laserEffectObject.transform.SetParent(startTarget.Body.transform);

        count--;
        StartCoroutine(CoAttackBeamSub(target, laserEffectObject, count));

        float timeToMaxDamage = 3;
        float hitInterval = 0.1f;
        float currentDamageRate = 0;
        float currentTime = 0;
        float currentDamagePerSec = 0;
        while (target != null && !target.Health.IsDead && parentLaser != null)
        {
            Collider[] cols = Physics.OverlapSphere(target.transform.position, 0.2f, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    float dotDamage = currentDamagePerSec * hitInterval;
                    hp.GetDamaged(dotDamage, mBaseObj);
                }
            }

            yield return newWaitForSeconds.Cache(hitInterval);
            currentTime += hitInterval;
            currentDamageRate = Mathf.Min(1, currentTime / timeToMaxDamage);
            currentDamagePerSec = mBaseObj.SpecProp.Damage * currentDamageRate;
        }

        Destroy(laserEffectObject.gameObject);
    }
    private bool IsOutOfAttackRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (AttackRange * 1.2f);
    }
    private void OnAttackBeamEnd()
    {
        if (mLaserEffectObject != null)
        {
            Destroy(mLaserEffectObject.gameObject);
            mLaserEffectObject = null;
        }
        StopCoroutine("CoAttackBeam");
    }
}
