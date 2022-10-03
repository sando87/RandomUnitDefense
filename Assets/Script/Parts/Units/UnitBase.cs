using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    protected BaseObject mBaseObj = null;

    public long ResourceID { get; set; } = 0;

    protected virtual void Awake()
    {
        mBaseObj = this.GetBaseObject();
    }

    protected IEnumerator CoMotionSwitcher(MotionBase motion, System.Func<float> speed, System.Func<float> range)
    {
        BaseObject prevTarget = null;
        while (true)
        {
            float cooltime = speed() == 0 ? 0 : 1 / speed();
            if (cooltime > 0)
                yield return new WaitForSeconds(cooltime);

            while (true)
            {
                yield return null;
                if (!mBaseObj.MotionManager.IsCurrentMotion<MotionIdle>())
                    continue;

                float detectRange = range();
                BaseObject target = null;
                if (detectRange > 0)
                {
                    if(prevTarget != null && !prevTarget.Health.IsDead)
                    {
                        if(mBaseObj.IsInDetectRange(prevTarget, detectRange))
                            target = prevTarget;
                        else
                            prevTarget = null;
                    }

                    if(target == null)
                    {
                        Collider[] cols = mBaseObj.DetectAround(detectRange, 1 << LayerID.Enemies);
                        if (cols.Length <= 0)
                            continue;
                        else
                            target = cols[UnityEngine.Random.Range(0, cols.Length)].GetBaseObject();
                    }
                }

                prevTarget = target;
                motion.Target = target;
                mBaseObj.MotionManager.SwitchMotion(motion);

                yield return new WaitUntil(() => !mBaseObj.MotionManager.IsCurrentMotion(motion));
                break;
            }
        }
    }
}
