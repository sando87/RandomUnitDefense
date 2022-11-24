using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 바닥에 불길을 생성하여 적을 불태운다.
// 레벨업에 따른 화염 뎀지 크기 범위 증가

public class UnitFlamer : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _SplshRange = 0.2f;
    [SerializeField] float _SkillDuration = 3.0f;
    [SerializeField][Range(0, 1)] float _DotDamageRate = 0.1f;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    
    [SerializeField] float duration = 1; // ~ 5
    [SerializeField] float lifetime = 0.5f; // ~ 1
    [SerializeField] float angle = 10; // ~ 20
    [SerializeField] int count = 20; // ~ 100

    private MotionActionSingle mMotionAttack = null;

    [SerializeField] GameObject FireBlastPrefab = null;
    [SerializeField] GameObject FireDecalPrefab = null;
    [SerializeField] GameObject BuffEffect = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));
    }

    private void OnAttack(int idx)
    {
        StartCoroutine(CoAttack(idx));
    }

    IEnumerator CoAttack(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        Vector3 dest = mMotionAttack.Target.transform.position;
        Vector3 dir = dest - firePosition; dir.z = 0;

        GameObject fireBlastEffect = Instantiate(FireBlastPrefab, firePosition, Quaternion.identity);
        fireBlastEffect.transform.right = dir;
        ParticleSystem ps = fireBlastEffect.GetComponent<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.duration = duration;
        main.startLifetime = lifetime;
        var emission = ps.emission;
        emission.rateOverTime = count;
        var shape = ps.shape;
        shape.angle = angle;
        ps.Play();

        GameObject decal = Instantiate(FireDecalPrefab, dest, Quaternion.identity);
        StartCoroutine(RepeatBuff(decal));

        float time = 0;
        while(time < duration)
        {
            Collider[] cols = Physics.OverlapSphere(dest, SplshRange, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    hp.GetDamaged(mBaseObj.SpecProp.Damage * _DotDamageRate, mBaseObj);
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


}
