using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 레이저로 누적데미지를 입힌다.

public class UnitChanneller : UnitPlayer
{
    [SerializeField] float _SkillRange = 2;

    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    private LaserAimming mLaserEffectObject = null;
    private MotionActionLoop mLaserMotion = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mLaserMotion = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mLaserMotion.EventStart = OnAttackBeamStart;
        mLaserMotion.EventEnd = OnAttackBeamEnd;
        StartCoroutine(CoMotionSwitcher(mLaserMotion, () => 0, () => SkillRange));
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
        while (!IsOutOfSkillRange(target))
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
        Collider[] nextCols = startTarget.DetectAround(1, 1 << LayerID.Enemies);
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
    private bool IsOutOfSkillRange(BaseObject target)
    {
        return (target.transform.position - mBaseObj.transform.position).magnitude > (SkillRange * 1.2f);
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
