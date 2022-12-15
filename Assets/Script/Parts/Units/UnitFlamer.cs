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
    [SerializeField][Range(0, 1)] float _DotDamageRate = 0.1f;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    float PS_lifetime { get { return SplshRange * 0.25f; } }
    float PS_angle { get { return SplshRange * 5; } }
    int PS_count { get { return (int)(SplshRange * 20); } }
    float SplshHeight { get { return 2 * SplshRange * Mathf.Tan(PS_angle * 0.5f * Mathf.Deg2Rad); } }

    private MotionActionSingle mMotionAttack = null;

    [SerializeField] GameObject FireBlastPrefab = null;
    [SerializeField] GameObject FireDecalPrefab = null;
    [SerializeField] GameObject BuffEffect = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcherFlamer(mMotionAttack));
    }

    private void OnAttack(int idx)
    {
        StartCoroutine(CoAttack(idx));
    }

    IEnumerator CoAttack(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;

        GameObject fireBlastEffect = Instantiate(FireBlastPrefab, firePosition, Quaternion.identity, mBaseObj.transform);
        //fireBlastEffect.transform.right = dir;
        ParticleSystem ps = fireBlastEffect.GetComponent<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.duration = SkillDuration;
        main.startLifetime = PS_lifetime;
        var emission = ps.emission;
        emission.rateOverTime = PS_count;
        var shape = ps.shape;
        shape.angle = PS_angle;
        ps.Play();

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

            // if(!mBaseObj.MotionManager.IsCurrentMotion(mMotionAttack))
            //     break;
        }

        yield return newWaitForSeconds.Cache(1);

        Destroy(fireBlastEffect);
        
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

                if (!IsEnmiesInFlameArea())
                    continue;

                motion.Target = null;
                mBaseObj.MotionManager.SwitchMotion(motion);

                yield return new WaitUntil(() => !mBaseObj.MotionManager.IsCurrentMotion(motion));
                break;
            }
        }
    }

    private bool IsEnmiesInFlameArea()
    {
        Vector3 firePos = mBaseObj.FirePosition.transform.position;
        Vector3 fireDir = new Vector3(mBaseObj.transform.right.x, 0, 0);
        Vector3 cenPos = firePos + fireDir * SplshRange * 0.5f;
        Vector3 size = new Vector3(SplshRange, SplshHeight, 1);
        if(Physics.CheckBox(cenPos, size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies))
            return true;

        Vector3 tmp = cenPos - mBaseObj.Body.Center;
        tmp.x *= -1;
        Vector3 oppositeCenPos = mBaseObj.Body.Center + tmp;
        if(Physics.CheckBox(oppositeCenPos, size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies))
            return true;

        return false;
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

                Vector3 targetDir = target.Body.Center - firePos;
                float degree = Vector3.Angle(fireDir, targetDir.normalized);
                if(degree <= (PS_angle * 0.5f))
                {
                    targets.Add(target);
                }
            }

            return targets.ToArray();
        }

        return null;
    }


}
