using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 바닥에 불길을 생성하여 적을 불태운다.

public class UnitFlamer : UnitBase
{
    [SerializeField] float _AttackSpeed = 0.5f;
    float AttackSpeed { get { return _AttackSpeed * mBaseObj.BuffProp.AttackSpeed; } }
    [SerializeField] float _AttackRange = 0.5f;
    float AttackRange { get { return _AttackRange * mBaseObj.BuffProp.AttackRange; } }
    [SerializeField][Range(0, 1)] float _SplshRange = 0.2f;
    float SplshRange { get { return _SplshRange * mBaseObj.BuffProp.SplshRange; } }
    [SerializeField] float _SkillDuration = 3.0f;
    float SkillDuration { get { return _SkillDuration * mBaseObj.BuffProp.SkillDuration; } }

    private MotionActionSingle mMotionAttack = null;

    [SerializeField] GameObject FireBlastPrefab = null;
    [SerializeField] GameObject FireDecalPrefab = null;
    [SerializeField] GameObject BuffEffect = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        mMotionAttack = mBaseObj.MotionManager.FindMotion<MotionActionSingle>();
        mMotionAttack.EventFired = OnAttack;
        StartCoroutine(CoMotionSwitcher(mMotionAttack, 1 / AttackSpeed, AttackRange));
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

        GameObject decal = Instantiate(FireDecalPrefab, dest, Quaternion.identity);
        StartCoroutine(RepeatBuff(decal));

        while(true)
        {
            Collider[] cols = Physics.OverlapSphere(dest, SplshRange, 1 << LayerID.Enemies);
            foreach (Collider col in cols)
            {
                Health hp = col.GetComponentInBaseObject<Health>();
                if (hp != null)
                {
                    hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
                }
            }

            yield return null;

            if(!mBaseObj.MotionManager.IsCurrentMotion(mMotionAttack))
                break;
        }

        Destroy(fireBlastEffect);
        
    }
    
    private IEnumerator RepeatBuff(GameObject decal)
    {
        float time = 0;
        while(time < SkillDuration)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            time += 0.5f;
            Collider[] cols = decal.DetectAround(SplshRange, mBaseObj.GetLayerMaskAttackable());
            foreach (Collider col in cols)
                ApplyAttackSpeedUpBuff(col.GetBaseObject());
        }
        Destroy(decal);
    }

    private void ApplyAttackSpeedUpBuff(BaseObject target)
    {
        DeBuffFireDamaged buff = target.BuffCtrl.FindBuff<DeBuffFireDamaged>();
        if (buff != null)
            buff.RenewBuff(); //동일한 버프가 있을 경우에는 갱신만. => 중복 불가...
        else
        {
            GameObject buffEffectObj = Instantiate(BuffEffect, target.Renderer.transform);
            target.BuffCtrl.AddBuff(new DeBuffFireDamaged(buffEffectObj, target, mBaseObj));
        }
    }

    class DeBuffFireDamaged : BuffBase
    {
        private GameObject BuffEffect;
        private BaseObject mTarget = null;
        private BaseObject mAttacker = null;
        private Health mHP = null;
        public DeBuffFireDamaged(GameObject buffEffect, BaseObject target, BaseObject attacker) { Duration = 1; BuffEffect = buffEffect; mTarget = target; mAttacker = attacker; }
        public override void StartBuff(BaseObject target)
        {
            mHP = mTarget.Health;
        }
        public override void UpdateBuff(BaseObject target)
        {
            base.UpdateBuff(target);
            if(mHP.IsDead)
            {
                BuffEffect.SetActive(false);
            }
            else
            {
                BuffEffect.SetActive(true);
                mHP.GetDamaged(1, mAttacker);
            }
        }
        public override void EndBuff(BaseObject target)
        {
            Destroy(BuffEffect);
        }
    }
}
