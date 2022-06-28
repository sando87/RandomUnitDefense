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

    protected IEnumerator CoMotionSwitcher(MotionBase motion, float motionCooltime, float detectRange)
    {
        while (true)
        {
            if (motionCooltime > 0)
                yield return new WaitForSeconds(motionCooltime);

            while (true)
            {
                yield return null;
                if (!mBaseObj.MotionManager.IsCurrentMotion<MotionIdle>())
                    continue;

                BaseObject target = null;
                if (detectRange > 0)
                {
                    Collider[] cols = mBaseObj.DetectAround(detectRange, 1 << LayerID.Enemies);
                    if (cols.Length <= 0)
                        continue;
                    else
                        target = cols[0].GetBaseObject();
                }

                motion.Target = target;
                mBaseObj.MotionManager.SwitchMotion(motion);

                yield return new WaitUntil(() => !mBaseObj.MotionManager.IsCurrentMotion(motion));
                break;
            }
        }
    }
}
