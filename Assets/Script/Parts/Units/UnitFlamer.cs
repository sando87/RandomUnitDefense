using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 바닥에 불길을 생성하여 적을 불태운다.
// 레벨업에 따른 화염 뎀지 크기 범위 증가

public class UnitFlamer : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _SplshRange = 2.0f; // ~ 4.0f
    [SerializeField] float _SkillDuration = 1.0f; // ~ 5.0f
    [SerializeField][Range(0, 1)] float _DotDamageRate = 0.3f;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    float PS_lifetime { get { return SplshRange * 0.25f; } }
    float PS_angle { get { return SplshRange * 5; } }
    int PS_count { get { return (int)(SplshRange * 20); } }
    float SplshHeight { get { return 2 * SplshRange * Mathf.Tan(PS_angle * 0.5f * Mathf.Deg2Rad); } }

    private MotionActionLoop mMotionAttack = null;
    private ParticleSystem mFireBlastEffect = null;

    [SerializeField] GameObject FireBlastPrefab = null;
    [SerializeField] GameObject FireDecalPrefab = null;
    [SerializeField] GameObject BuffEffect = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        int curLevel = mBaseObj.SpecProp.Level;
        BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        spec.damage = InGameSystem.Instance.SaveTestInfo.units[6].damages[curLevel - 1];
        spec.damagesPerUp[curLevel - 1] = InGameSystem.Instance.SaveTestInfo.units[6].damagesPerUp[curLevel - 1];

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
        //     spec.damagesPerUp[1] = 12;
        // }
        // else if (curLevel <= 3)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 140;
        //     spec.damagesPerUp[2] = 70;
        // }
        // else if (curLevel <= 4)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 350;
        //     spec.damagesPerUp[3] = 450;
        // }
        // else if (curLevel <= 5)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 835;
        //     spec.damagesPerUp[4] = 1060;
        // }
        // else if (curLevel <= 6)
        // {
        //     BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
        //     spec.damage = 204800;
        // }

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionLoop>();
        mMotionAttack.Duration = SkillDuration;
        mMotionAttack.EventStart = OnAttack;
        mMotionAttack.EventEnd = () => StopFireBlasting();
        StartCoroutine(CoMotionSwitcherFlamer(mMotionAttack));
    }

    private void OnAttack(BaseObject target)
    {
        StartCoroutine("CoAttack");
    }

    private void StopFireBlasting()
    {
        StopCoroutine("CoAttack");
        if(mFireBlastEffect != null)
        {
            mFireBlastEffect.Stop();
            Destroy(mFireBlastEffect.gameObject, 0.5f);
            mFireBlastEffect = null;
        }
    }

    IEnumerator CoAttack()
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;

        GameObject fireBlastEffect = Instantiate(FireBlastPrefab, firePosition, Quaternion.identity, mBaseObj.transform);
        fireBlastEffect.transform.localRotation = Quaternion.identity;
        mFireBlastEffect = fireBlastEffect.GetComponent<ParticleSystem>();
        mFireBlastEffect.Stop();
        var main = mFireBlastEffect.main;
        main.duration = SkillDuration;
        main.startLifetime = PS_lifetime;
        var emission = mFireBlastEffect.emission;
        emission.rateOverTime = PS_count;
        var shape = mFireBlastEffect.shape;
        shape.angle = PS_angle;
        mFireBlastEffect.Play();

        // GameObject decal = Instantiate(FireDecalPrefab, dest, Quaternion.identity);
        // StartCoroutine(RepeatBuff(decal));

        float time = 0;
        while(time < SkillDuration)
        {
            BaseObject[] targets = GetEnmiesInFlameArea();
            if(targets != null && targets.Length > 0)
            {
                foreach (BaseObject target in targets)
                {
                    if (target.Health != null)
                        target.Health.GetDamaged(mBaseObj.SpecProp.Damage * _DotDamageRate, mBaseObj);
                }
            }

            yield return newWaitForSeconds.Cache(0.1f);
            time += 0.1f;
        }

        mBaseObj.MotionManager.SwitchMotion<MotionIdle>();
    }
    
    private IEnumerator RepeatBuff(GameObject decal)
    {
        float time = 0;
        while(time < SkillDuration)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            time += 0.5f;
            // Collider[] cols = decal.DetectAround(SplshRange, mBaseObj.GetLayerMaskAttackable());
            // foreach (Collider col in cols)
            //     ApplyAttackSpeedUpBuff(col.GetBaseObject());
        }
        Destroy(decal);
    }
    
    protected IEnumerator CoMotionSwitcherFlamer(MotionBase motion)
    {
        while (true)
        {
            float cooltime = AttackSpeed == 0 ? 0 : 1 / AttackSpeed;
            if (cooltime > 0)
                yield return new WaitForSeconds(cooltime);

            while (true)
            {
                yield return null;
                if (!mBaseObj.MotionManager.IsCurrentMotion<MotionIdle>())
                    continue;

                BaseObject target = DetectTargetInFlameArea();
                if (target == null)
                    continue;

                motion.Target = target;
                mBaseObj.MotionManager.SwitchMotion(motion);

                yield return new WaitUntil(() => !mBaseObj.MotionManager.IsCurrentMotion(motion));
                break;
            }
        }
    }

    private BaseObject DetectTargetInFlameArea()
    {
        Vector3 firePos = mBaseObj.FirePosition.transform.position;
        Vector3 fireDir = new Vector3(mBaseObj.transform.right.x, 0, 0);
        Vector3 cenPos = firePos + fireDir * SplshRange * 0.5f;
        Vector3 size = new Vector3(SplshRange, SplshHeight, 1);
        Collider[] cols = Physics.OverlapBox(cenPos, size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies);
        if(cols.Length > 0)
            return cols[0].GetBaseObject();

        Vector3 tmp = cenPos - mBaseObj.Body.Center;
        tmp.x *= -1;
        Vector3 oppositeCenPos = mBaseObj.Body.Center + tmp;
        cols = Physics.OverlapBox(oppositeCenPos, size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies);
        if(cols.Length > 0)
            return cols[0].GetBaseObject();

        return null;
    }

    // 화염방사되는 삼각형 지역 안에 들어오는 적들을 반환한다.(정확히는 해상 타겟의 Body.Center위치가 안에 들어오는 객체만)
    private BaseObject[] GetEnmiesInFlameArea()
    {
        Vector3 firePos = mBaseObj.FirePosition.transform.position;
        Vector3 fireDir = new Vector3(mBaseObj.transform.right.x, 0, 0);
        Vector3 cenPos = firePos + fireDir * SplshRange * 0.5f;
        Vector3 size = new Vector3(SplshRange, SplshHeight, 1);
        if(Physics.CheckBox(cenPos, size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies))
        {
            List<BaseObject> targets = new List<BaseObject>();
            Collider[] cols = Physics.OverlapBox(cenPos, size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies);
            foreach(Collider col in cols)
            {
                BaseObject target = col.GetBaseObject();
                if(target == null) continue;

                // Vector3 targetDir = target.Body.Center - firePos;
                // float degree = Vector3.Angle(fireDir, targetDir.normalized);
                // if(degree <= (PS_angle * 0.5f))
                {
                    targets.Add(target);
                }
            }

            return targets.ToArray();
        }

        return null;
    }


}
