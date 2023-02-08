using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨 업에 따른 느려지는 적 타게팅 증가, 최대 느려지는 속도 증가

public class UnitMarineBasic : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 1;
    [SerializeField] float _BuffRange = 3.0f;
    [SerializeField] float _SkillDuration = 3.0f;
    [SerializeField] float _ForcePower = 10.0f;
    [SerializeField] BuffBase BuffEffectPrefab = null;
    [SerializeField] BuffBase AttackDeBuff = null;

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
    
        StartCoroutine(KeepBuff(BuffEffectPrefab));
    }

    private void OnAttack(int idx)
    {
        Vector3[] fireDirs = DoMuzzleShotgunEffect();

        List<BaseObject> targets = new List<BaseObject>();

        Vector3 muzzlePos = mBaseObj.FirePosition.transform.position;
        foreach(Vector3 fireDir in fireDirs)
        {
            RaycastHit[] hits = Physics.RaycastAll(muzzlePos, fireDir, AttackRange, 1 << LayerID.Enemies);
            foreach(RaycastHit hit in hits)
            {
                BaseObject target = hit.collider.GetBaseObject();
                if (!targets.Contains(target))
                {
                    targets.Add(target);
                    break;
                }
            }
        }

        foreach (BaseObject target in targets)
        {
            Vector3 force = (target.transform.position - mBaseObj.transform.position).normalized * _ForcePower;
            target.Health.GetForced(force, mBaseObj);

            target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            target.BuffCtrl.ApplyBuff(AttackDeBuff, SkillDuration, true);

            ObjectPooling.Instance.InstantiateVFX("HitSimple", target.Body.Center, Quaternion.identity).ReturnAfter(1);
        }
    }

    private Vector3[] DoMuzzleShotgunEffect()
    {
        Vector3 firePos = mBaseObj.FirePosition.transform.position;
        Vector3 fireDir = mBaseObj.FirePosition.transform.position - mBaseObj.Body.Center;

        List<Vector3> ShootDirections = new List<Vector3>();
        GameObject vfx = ObjectPooling.Instance.InstantiateVFX("MuzzleShotgun", firePos, Quaternion.identity);
        vfx.transform.localScale = new Vector3(AttackRange, AttackRange, 1);
        vfx.transform.right = fireDir;

        float rotateAngle = 7.5f * (mBaseObj.SpecProp.Level - 1);
        vfx.transform.Rotate(new Vector3(0, 0, rotateAngle));

        for(int i = 0; i < vfx.transform.childCount; ++i)
        {
            GameObject subVFX = vfx.transform.GetChild(i).gameObject;
            subVFX.SetActive(i < mBaseObj.SpecProp.Level);
            if(subVFX.activeSelf)
                ShootDirections.Add(subVFX.transform.right);
        }

        vfx.ReturnAfter(1);
        return ShootDirections.ToArray();
    }
    


}
