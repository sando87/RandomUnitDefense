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

    [SerializeField] MotionActionSingle SkillAttack = null;
    [SerializeField] float _SkillSpeed = 0.3f;
    [SerializeField] float _SkillRange = 1;
    [SerializeField] int _RandomShootCount = 3;
    [SerializeField] GameObject HitFloorPrefab = null;
    [SerializeField][Range(0, 1)] float DotDamageRate = 0.1f;
    [SerializeField] private GameObject BulletSparkPrefab = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SkillSpeed { get { return _SkillSpeed * mBaseObj.BuffProp.SkillSpeed; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    int RandomShootCountTest { get { return mBaseObj.SpecProp.Level * 2; } }

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        if (curLevel <= 1)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 5;
            spec.damagesPerUp[0] = 1;
            _RandomShootCount = RandomShootCountTest;
        }
        else if (curLevel <= 2)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 15;
            spec.damagesPerUp[1] = 8;
            _RandomShootCount = RandomShootCountTest;
        }
        else if (curLevel <= 3)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 85;
            spec.damagesPerUp[2] = 68;
            _RandomShootCount = RandomShootCountTest;
        }
        else if (curLevel <= 4)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 240;
            spec.damagesPerUp[3] = 870;
            _RandomShootCount = RandomShootCountTest;
        }
        else if (curLevel <= 5)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 615;
            spec.damagesPerUp[4] = 1210;
            _RandomShootCount = RandomShootCountTest;
        }
        else if (curLevel <= 6)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 204800;
        }


        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        NormalAttack.EventFired = OnAttack;
        SkillAttack.EventFired = OnSkill;
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

    private void OnSkill(int idx)
    {
        StartCoroutine(CoRandomShoot());
    }
    IEnumerator CoRandomShoot()
    {
        GameObject hitFloorObject = Instantiate(HitFloorPrefab, mBaseObj.transform);
        
        while(true)
        {
            if(SkillAttack.NormalizedTime < 0.65f)
            {
                for(int i = 0; i < _RandomShootCount; ++i)
                {
                    DoRandomShootAround();
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

            yield return newWaitForSeconds.Cache(0.1f);

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
        if((dest - startPos).magnitude < SkillRange * 0.5f)
            dest = (dest.normalized * SkillRange * 0.5f);
        
        Vector3 dir = (dest - startPos).ZeroZ().normalized;
        LaserAimming laserObj = LaserAimming.Play(startPos, dest, "SniperLaser");
        this.ExDelayedCoroutine(1, () => Destroy(laserObj.gameObject));

        if(Physics.Raycast(startPos, dir, out RaycastHit hit, dir.magnitude, 1 << LayerID.Enemies))
        {
            float damage = mBaseObj.SpecProp.Damage * DotDamageRate;
            hit.collider.GetBaseObject().Health.GetDamaged(damage, mBaseObj);

            Vector3 pos = MyUtils.Random(hit.collider.GetBaseObject().Body.Center, 0.1f);
            GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
            Destroy(obj, 1.0f);
        }
    }
}
