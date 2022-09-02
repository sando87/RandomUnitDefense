using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPowerGirl : UnitPlayer
{
    [SerializeField] MotionActionSingle NormalAttack = null;
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 2;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField] SimpleMissile SimpleMissile = null;

    [SerializeField] MotionActionSingle SkillAttack = null;
    [SerializeField] float _Cooltime = 3;
    float Cooltime { get { return _Cooltime * mBaseObj.BuffProp.Cooltime; } }
    [SerializeField] float _SkillRange = 1;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }
    [SerializeField] GameObject HitFloorPrefab = null;
    [SerializeField][Range(0, 1)] float Accuracy = 0.25f;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        NormalAttack.EventFired = OnAttack;
        SkillAttack.EventFired = OnSkill;
        StartCoroutine(CoMotionSwitcher(NormalAttack, 1 / AttackSpeed, AttackRange));
        StartCoroutine(CoMotionSwitcher(SkillAttack, Cooltime, SkillRange));
    }

    void OnAttack(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.parent.Find("@_" + idx).position;
        BaseObject target = NormalAttack.Target;
        SimpleMissile missile = Instantiate(SimpleMissile, firePosition, Quaternion.identity);
        missile.EventHit = OnHitMissile;
        missile.Launch(target);
    }
    void OnHitMissile(BaseObject target)
    {
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
    }

    private void OnSkill(int idx)
    {
        StartCoroutine(CoRandomShoot());
    }
    IEnumerator CoRandomShoot()
    {
        GameObject hitFloorObject = Instantiate(HitFloorPrefab, mBaseObj.transform);
        
        Collider[] targets = mBaseObj.DetectAround(SkillRange, 1 << LayerID.Enemies);
        while(true)
        {
            if(SkillAttack.NormalizedTime < 0.8f)
            {
                foreach (Collider col in targets)
                {
                    int percent = (int)(Accuracy * 100.0f);
                    if (UnityEngine.Random.Range(0, 100) < percent)
                    {
                        col.GetBaseObject().Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
                    }
                }
            }
            else
            {
                if(hitFloorObject != null)
                {
                    Destroy(hitFloorObject);
                    hitFloorObject = null;
                }
            }

            yield return null;

            if(mBaseObj.MotionManager.CurrentMotion != SkillAttack)
                break;
        }

        if (hitFloorObject != null)
        {
            Destroy(hitFloorObject);
            hitFloorObject = null;
        }
    }
}
