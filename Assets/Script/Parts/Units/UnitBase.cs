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
