using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨 업에 따른 미사일 개수 증가

public class UnitPowerGirl : UnitPlayer
{
    [SerializeField] MotionActionSingle NormalAttack = null;
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 2;
    [SerializeField] SimpleMissile SimpleMissile = null;

    [SerializeField] MotionActionLoop SkillAttack = null;
    [SerializeField] float _SkillSpeed = 0.3f;
    [SerializeField] float _SkillRange = 1;
    [SerializeField] float _RandomShootInterval = 0.5f;
    [SerializeField] GameObject HitFloorPrefab = null;
    [SerializeField][Range(0, 1)] float DotDamageRate = 0.1f;
    [SerializeField] private GameObject BulletSparkPrefab = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillSpeed { get { return _SkillSpeed * mBaseObj.BuffProp.SkillSpeed; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    float[] intervals = { 0.3f, 0.2f, 0.1f, 0.07f, 0.04f, 0.02f };
    float[] speeds = { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 2.0f };
    float RandomShootIntervalTest { get { return intervals[mBaseObj.SpecProp.Level - 1]; } }
    float LoopMotionSpeed { get { return speeds[mBaseObj.SpecProp.Level - 1]; } }

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        if (curLevel <= 1)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 5;
            spec.damagesPerUp[0] = 1;
            _RandomShootInterval = RandomShootIntervalTest;
            SkillAttack.SetActionLoopSpeed(LoopMotionSpeed);
        }
        else if (curLevel <= 2)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 15;
            spec.damagesPerUp[1] = 8;
            _RandomShootInterval = RandomShootIntervalTest;
            SkillAttack.SetActionLoopSpeed(LoopMotionSpeed);
        }
        else if (curLevel <= 3)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 85;
            spec.damagesPerUp[2] = 68;
            _RandomShootInterval = RandomShootIntervalTest;
            SkillAttack.SetActionLoopSpeed(LoopMotionSpeed);
        }
        else if (curLevel <= 4)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 240;
            spec.damagesPerUp[3] = 870;
            _RandomShootInterval = RandomShootIntervalTest;
            SkillAttack.SetActionLoopSpeed(LoopMotionSpeed);
        }
        else if (curLevel <= 5)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 615;
            spec.damagesPerUp[4] = 1210;
            _RandomShootInterval = RandomShootIntervalTest;
            SkillAttack.SetActionLoopSpeed(LoopMotionSpeed);
        }
        else if (curLevel <= 6)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 204800;
            SkillAttack.SetActionLoopSpeed(LoopMotionSpeed);
        }


        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        NormalAttack.EventFired = OnAttack;
        SkillAttack.EventStart = OnSkillStart;
        StartCoroutine(CoMotionSwitcher(NormalAttack, () => AttackSpeed, () => AttackRange));
        StartCoroutine(CoMotionSwitcher(SkillAttack, () => SkillSpeed, () => SkillRange));
    }

    void OnAttack(int idx)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.parent.Find("@_" + idx).position;
        BaseObject target = NormalAttack.Target;
        SimpleMissile missile = Instantiate(SimpleMissile, firePosition, Quaternion.identity);
        missile.Launch(target);
        float damage = mBaseObj.SpecProp.Damage;
        missile.EventHit = (t) => 
        {
            Collider[] cols = Physics.OverlapSphere(missile.transform.position, 0.25f, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    hp.GetDamaged(damage, mBaseObj);
                }
            }
        };
    }

    private void OnSkillStart(BaseObject target)
    {
        StartCoroutine(CoRandomShoot());
    }
    IEnumerator CoRandomShoot()
    {
        GameObject hitFloorObject = Instantiate(HitFloorPrefab, mBaseObj.transform);
        
        while(true)
        {
            DoRandomShootAround();

            yield return newWaitForSeconds.Cache(_RandomShootInterval);

            if(mBaseObj.MotionManager.CurrentMotion != SkillAttack)
                break;
        }

        if (hitFloorObject != null)
        {
            Destroy(hitFloorObject);
            hitFloorObject = null;
        }
    }

    void DoRandomShootAround()
    {
        Vector3 startPos = mBaseObj.Body.Center;
        Vector3 dest = MyUtils.Random(mBaseObj.Body.Center, SkillRange);
        Vector3 dir = dest - startPos;
        dir.z = 0;
        if(dir.magnitude < SkillRange * 0.75f)
            dest = startPos + (dir.normalized * SkillRange * 0.75f);
        
        dir = (dest - startPos).ZeroZ();
        LaserAimming laserObj = LaserAimming.Play(startPos, dest, "SniperLaser");
        this.ExDelayedCoroutine(1, () => Destroy(laserObj.gameObject));
        
        // 우선 발사지점에 겹쳐져있는 적 감지하여 공격
        Collider[] cols = Physics.OverlapSphere(startPos, 0.1f, 1 << LayerID.Enemies);
        if(cols.Length > 0)
        {
            BaseObject target = cols[UnityEngine.Random.Range(0, cols.Length)].GetBaseObject();
            float damage = mBaseObj.SpecProp.Damage * DotDamageRate;
            target.Health.GetDamaged(damage, mBaseObj);

            Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
            GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
            Destroy(obj, 1.0f);
            return;
        }

        // 겹쳐있는 적이 없으면 직선상 걸리는 적에게 타격
        if(Physics.Raycast(startPos, dir.normalized, out RaycastHit hit, dir.magnitude, 1 << LayerID.Enemies))
        {
            float damage = mBaseObj.SpecProp.Damage * DotDamageRate;
            hit.collider.GetBaseObject().Health.GetDamaged(damage, mBaseObj);

            Vector3 pos = MyUtils.Random(hit.collider.GetBaseObject().Body.Center, 0.1f);
            GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
            Destroy(obj, 1.0f);
        }
    }
}
