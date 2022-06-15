using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPowerGirl : UnitBase
{
    [SerializeField] MotionActionSingle NormalAttack = null;
    [SerializeField] SimpleMissile SimpleMissile = null;

    [SerializeField] MotionActionSingle SkillAttack = null;
    [SerializeField] GameObject HitFloorPrefab = null;
    [SerializeField][Range(0, 1)] float Accuracy = 0.25f;

    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        NormalAttack.EventFired = OnAttack;
        SkillAttack.EventFired = OnSkill;
    }

    // public override string SkillDescription
    // {
    //     get
    //     {
    //         return "일정 시간마다 총기 난사";
    //     }
    // }

    void OnAttack(Collider[] targets)
    {
        BaseObject target = targets[0].GetBaseObject();
        SimpleMissile missile = Instantiate(SimpleMissile, mBaseObj.Body.Center, Quaternion.identity);
        missile.EventHit = OnHitMissile;
        missile.Launch(target);
    }
    void OnHitMissile(BaseObject target)
    {
        target.Health.GetDamaged(mBaseObj);
    }

    private void OnSkill(Collider[] targets)
    {
        StartCoroutine(CoRandomShoot(targets));
    }
    IEnumerator CoRandomShoot(Collider[] targets)
    {
        GameObject hitFloorObject = Instantiate(HitFloorPrefab, mBaseObj.transform);
        while(true)
        {
            foreach (Collider col in targets)
            {
                int percent = (int)(Accuracy * 100.0f);
                if (UnityEngine.Random.Range(0, 100) < percent)
                {
                    col.GetBaseObject().Health.GetDamaged(mBaseObj);
                }
            }

            yield return null;

            if(mBaseObj.MotionManager.CurrentMotion != SkillAttack)
                break;
        }

        Destroy(hitFloorObject);
    }
}
