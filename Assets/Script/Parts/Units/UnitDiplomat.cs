using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 피격시 범위 피해
// 레벨업에 따른 개수 증가

public class UnitDiplomat : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _SplshRange = 0.1f;
    [SerializeField] float _SkillRange = 3.0f;
    
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] BuffBase BuffEffect = null;
    
    [SerializeField] GuidedMissile _Projectaile = null;
    [SerializeField][PrefabSelector(Consts.VFXPath)] string _HitVFX = "";

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    private MotionActionSingle mMotionAttack = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, () => AttackSpeed, () => AttackRange));

        StartCoroutine(KeepBuff(BuffEffect));
    }

    void OnAttack(int idx)
    {
        ShootProjectail(mMotionAttack.Target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        Vector3 dir = target.Body.Center - firePosition;
        dir.z = 0;

        GuidedMissile projectile = Instantiate(_Projectaile, firePosition, Quaternion.identity);
        projectile.transform.right = dir.normalized;
        
        float damage = mBaseObj.SpecProp.Damage;

        projectile.EventHit += (target) => 
        {
            ObjectPooling.Instance.InstantiateVFX(_HitVFX, projectile.transform.position, Quaternion.identity).ReturnAfter(1);
            target.Health.GetDamaged(damage, mBaseObj);
        };
    }
}
