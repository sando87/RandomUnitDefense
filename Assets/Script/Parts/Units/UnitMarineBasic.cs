using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarineBasic : UnitPlayer
{
    [SerializeField] float _AttackSpeed = 0.5f;
    [SerializeField] float _AttackRange = 0.5f;
    [SerializeField] float _BuffRange = 3.0f;
    [SerializeField] float _SkillDuration = 1.0f;
    [SerializeField] private GameObject HitParticle = null;
    [SerializeField] private BuffBase PassiveBuff = null;
    [SerializeField] private BuffBase AttackDeBuff = null;

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
        StartCoroutine(RepeatBuff());
    }

    private void OnAttack(int idx)
    {   
        BaseObject target = mMotionAttack.Target;
        target.Health.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
        target.BuffCtrl.ApplyBuff(AttackDeBuff, SkillDuration, true);

        Vector3 pos = MyUtils.Random(target.Body.Center, 0.1f);
        GameObject obj = Instantiate(HitParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }

    private IEnumerator RepeatBuff()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(0.5f);
            Collider[] cols = mBaseObj.DetectAround(BuffRange, 1 << mBaseObj.gameObject.layer);
            foreach (Collider col in cols)
                col.GetBaseObject().BuffCtrl.ApplyBuff(PassiveBuff, 1);
        }
    }



}
