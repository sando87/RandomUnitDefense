using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitGunner : UnitUser
{
    [SerializeField] private Sprite[] IntroSprites = null;
    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;

    private LaserAimming mLaserEffectObject = null;

    public override void Init()
    {
        base.Init();
        FSM.ChangeState(UnitState.Appear);
        GetComponent<MotionSingleAttack>().EventFired = OnAttack;
        GetComponent<MotionKeepAttack>().EventStart = OnAttackBeamStart;
        GetComponent<MotionKeepAttack>().EventEnd = OnAttackBeamEnd;
    }
    public override void Release()
    {
        base.Release();
    }

    public override string SkillDescription
    {
        get
        {
            return "근접 거리의 적 유닛은 Beam공격으로 감속 효과";
        }
    }

    private void OnAttack(UnitBase target)
    {
        ShootProjectail(target);
    }

    private void OnAttackBeamStart(UnitBase target)
    {
        if(mLaserEffectObject != null)
        {
            Destroy(mLaserEffectObject.gameObject);
            mLaserEffectObject = null;
        }
        mLaserEffectObject = LaserAimming.Play(Center, target.gameObject);
        StartCoroutine(CoAttackBeam(target));
    }
    IEnumerator CoAttackBeam(UnitBase target)
    {
        while (true)
        {
            ApplySlowDeBuff(target);
            yield return new WaitForSeconds(Property.SkillDuration - 0.1f);
        }
    }
    private void OnAttackBeamEnd()
    {
        if (mLaserEffectObject != null)
        {
            Destroy(mLaserEffectObject.gameObject);
            mLaserEffectObject = null;
        }
        StopAllCoroutines();
    }
    private void ApplySlowDeBuff(UnitBase target)
    {
        DeBuffSlow buff = target.BuffCtrl.FindBuff<DeBuffSlow>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 결국 마린 여러마리가 공격해도 slow효과는 중복되지 않는 개념...
        else
            target.BuffCtrl.AddBuff(new DeBuffSlow(Property.SkillDuration));
    }
    private void ShootProjectail(UnitBase target)
    {
        Vector3 dir = target.Center - Center;
        dir.z = 0;
        SpritesAnimator.Play(Center, IntroSprites);

        SpritesAnimator proj = SpritesAnimator.Play(Center, ProjSprites, true);
        proj.transform.right = dir.normalized;
        proj.transform.DOMove(target.Center, 0.3f).OnComplete(() =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            UnitMob enemy = target as UnitMob;
            enemy.GetDamaged(Property);

            Destroy(proj.gameObject);
        });
    }

    class DeBuffSlow : BuffBase
    {
        //적 이동속도 20% 감소 디버프(duration시간만큼 지속)
        public DeBuffSlow(float duration) { Duration = duration; }
        public override void StartBuff(UnitBase target)
        {
            target.SR.color = Color.green;
            target.BuffValues.MoveSpeed -= 20;
        }
        public override void EndBuff(UnitBase target)
        {
            target.SR.color = Color.white;
            target.BuffValues.MoveSpeed += 20;
        }
    }
}
