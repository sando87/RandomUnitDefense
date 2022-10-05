using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨 업에 따른 느려지는 적 타게팅 증가, 최대 느려지는 속도 증가

public class UnitMarineBasic : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _BuffRange = 3.0f;
    [SerializeField] float _SkillDuration = 1.0f;
    [SerializeField] private ParticleSystem MuzzleParticle = null;
    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private BuffBase PassiveBuff = null;
    [SerializeField] private BuffBase AttackDeBuff = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float BuffRange { get { return _BuffRange * mBaseObj.BuffProp.SkillRange; } }
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    private MotionActionSingle mMotionAttack = null;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();
        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));
        StartCoroutine(RepeatBuff());

        InitMuzzleEffect();
    }

    private void OnAttack(int idx)
    {
        PlayMuzzleEffect();

        List<BaseObject> targets = new List<BaseObject>();

        BaseObject target = mMotionAttack.Target;
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
        target.BuffCtrl.ApplyBuff(AttackDeBuff, SkillDuration, true);
        targets.Add(target);
        
        GameObject obj = Instantiate(HitParticle, target.Body.Center, Quaternion.identity);
        Destroy(obj, 1.0f);

        Vector3 muzzlePos = mBaseObj.FirePosition.transform.position;        
        for(int i = 0; i < mBaseObj.SpecProp.Level; ++i)
        {
            Vector3 dir = GetMuzzleDir(i);
            if(Physics.Raycast(muzzlePos, dir, out RaycastHit hit, AttackRange, 1 << LayerID.Enemies))
            {
                target = hit.collider.GetBaseObject();
                if(!targets.Contains(target))
                {
                    target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
                    target.BuffCtrl.ApplyBuff(AttackDeBuff, SkillDuration, true);
                    targets.Add(target);
                    
                    obj = Instantiate(HitParticle, target.Body.Center, Quaternion.identity);
                    Destroy(obj, 1.0f);
                }
            }
        }
    }

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(BuffRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                col.GetBaseObject().BuffCtrl.ApplyBuff(PassiveBuff, 1);
        }
    }

    private void InitMuzzleEffect()
    {
        MuzzleParticle.transform.SetParent(mBaseObj.FirePosition.transform);
        MuzzleParticle.transform.localPosition = Vector3.zero;
        MuzzleParticle.transform.localRotation = Quaternion.identity;
        for(int i = 0; i < mBaseObj.SpecProp.Level; ++i)
        {
            MuzzleParticle.transform.GetChild(i).gameObject.SetActive(true);
        }

        float rotateAngle = (10 * (mBaseObj.SpecProp.Level - 1)) * 0.5f;
        MuzzleParticle.transform.localRotation = Quaternion.Euler(0, 0, rotateAngle);
    }
    private void PlayMuzzleEffect()
    {
        MuzzleParticle.Play();
    }
    
    private Vector3 GetMuzzleDir(int idx)
    {
        return MuzzleParticle.transform.GetChild(idx).right;
    }



}
