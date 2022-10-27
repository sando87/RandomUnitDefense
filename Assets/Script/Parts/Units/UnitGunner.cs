using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// 발사 피격시 랜덤하게 연쇄레이저

public class UnitGunner : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 2;
    [SerializeField] float _LaserPercent = 0.3f;

    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    float LaserPercent { get { return _LaserPercent * mBaseObj.BuffProp.Percentage; } }
    int LaserChainCount { get { return mBaseObj.SpecProp.Level; } }

    private MotionActionSingle mAttackMotion = null;

    void Start()
    {
        int curLevel = mBaseObj.SpecProp.Level;
        if (curLevel <= 1)
        {
        }
        else if (curLevel <= 2)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 50;
        }
        else if (curLevel <= 3)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 300;
        }
        else if (curLevel <= 4)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 1800;
        }
        else if (curLevel <= 5)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 10800;
        }
        else if (curLevel <= 6)
        {
            BasicSpec spec = mBaseObj.SpecProp.GetPrivateFieldValue<BasicSpec>("_Spec");
            spec.damage = 64800;
        }

        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mAttackMotion = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mAttackMotion.EventFired = OnAttack;

        StartCoroutine(CoMotionSwitcher(mAttackMotion, () => AttackSpeed, () => AttackRange));
    }

    private void OnAttack(int idx)
    {
        ShootProjectail(mAttackMotion.Target);
    }
    private void ShootProjectail(BaseObject target)
    {
        Vector3 firePosition = mBaseObj.FirePosition.transform.position;
        Vector3 dir = target.Body.Center - firePosition;
        dir.z = 0;
        SpritesAnimator.Play(firePosition, IntroSprites);

        SpritesAnimator proj = SpritesAnimator.Play(firePosition, ProjSprites, true);
        proj.transform.right = dir.normalized;
        
        float damage = mBaseObj.SpecProp.Damage;
        bool isHit = MyUtils.IsHitPercent(LaserPercent);
        int laserChainCount = LaserChainCount;

        proj.transform.CoMoveTo(target.Body.transform, 0.5f, () =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            target.Health.GetDamaged(damage, mBaseObj);

            if(isHit)
            {
                AttackLaserBeam(target, mBaseObj.FirePosition.transform, damage * 3, laserChainCount);
            }

            Destroy(proj.gameObject);
        });
    }

    private void AttackLaserBeam(BaseObject target, Transform startPos, float damage, int remainChainCount)
    {
        if(remainChainCount <= 0)
            return;

        LaserAimming laserEffectObject = LaserAimming.Play(startPos.position, target.Body.gameObject, "GunnerLaser");
        laserEffectObject.transform.SetParent(startPos);
        laserEffectObject.transform.DOLocalMoveY(0, 0.2f).OnComplete(() => Destroy(laserEffectObject.gameObject));

        if(target.Health != null)
        {
            target.Health.GetDamaged(damage, mBaseObj);

            // StartCoroutine(CoDamageLaser(target, damage, 0.5f));

            Collider[] cols = target.DetectAround(2, 1 << LayerID.Enemies);
            if(cols.Length > 1)
            {
                var sortCols = cols.OrderBy((col) => (target.transform.position - col.transform.position).sqrMagnitude);
                AttackLaserBeam(sortCols.ElementAt(1).GetBaseObject(), target.Body.transform, damage, remainChainCount - 1);
            }
        }
    }

    IEnumerator CoDamageLaser(BaseObject target, float damage, float duration)
    {
        float time = 0;
        float dotDamge = damage * 0.5f;
        while(time < duration)
        {
            yield return new WaitForSeconds(0.05f);
            if(target == null || target.Health == null || target.Health.IsDead)
                break;

            target.Health.GetDamaged(dotDamge, mBaseObj);
            dotDamge *= 0.5f;
            time += 0.05f;
        }
    }
}
