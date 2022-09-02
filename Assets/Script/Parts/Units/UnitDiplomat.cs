using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 스킬 사거리 20%증가

public class UnitDiplomat : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }

    [SerializeField] float _SkillRange = 3.0f;
    float SkillRange { get { return _SkillRange * mBaseObj.BuffProp.SkillRange; } }

    [SerializeField] private Sprite[] ProjSprites = null;
    [SerializeField] private Sprite[] OutroSprites = null;
    [SerializeField] GameObject BuffEffect = null;

    private MotionActionSingle mMotionAttack = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
        StartCoroutine(RepeatBuff());
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

        SpritesAnimator proj = SpritesAnimator.Play(firePosition, ProjSprites, true);
        proj.transform.right = dir.normalized;
        proj.transform.CoMoveTo(target.Body.transform, 0.3f, () =>
        {
            SpritesAnimator.Play(proj.transform.position, OutroSprites);

            target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);

            Destroy(proj.gameObject);
        });
    }

    

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(SkillRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                ApplyBuff(col.GetBaseObject());
        }
    }

    private void ApplyBuff(BaseObject target)
    {
        BuffSkillRangeUp buff = target.BuffCtrl.FindBuff<BuffSkillRangeUp>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 중복 불가...
        else
        {
            GameObject buffEffectObj = Instantiate(BuffEffect, target.Renderer.transform);
            target.BuffCtrl.AddBuff(new BuffSkillRangeUp(buffEffectObj));
        }
    }

    class BuffSkillRangeUp : BuffBase
    {
        //1초간 아군 공속 20% 증소
        private GameObject BuffEffect;
        public BuffSkillRangeUp(GameObject buffEffect) { Duration = 1; BuffEffect = buffEffect; }
        public override void StartBuff(BaseObject target)
        {
            BuffEffect.SetActive(true);
            target.BuffProp.SkillRange += 20;
        }
        public override void EndBuff(BaseObject target)
        {
            target.BuffProp.SkillRange -= 20;
            Destroy(BuffEffect);
        }
    }

}
