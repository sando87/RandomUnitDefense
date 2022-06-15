using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionManager : MonoBehaviour
{
    [SerializeField] MotionBase StartMotion = null;

    private Dictionary<int, MotionBase> mMotions = new Dictionary<int, MotionBase>();

    public int CurrentMotionID { get; private set; } = 0;
    public MotionBase CurrentMotion { get { return mMotions[CurrentMotionID]; } }
    public bool Lock { get; set; } = false;

    void Awake()
    {
        MotionBase[] motions = GetComponentsInChildren<MotionBase>();
        foreach (MotionBase motion in motions)
        {
            motion.OnInit();
            mMotions[motion.GetInstanceID()] = motion;
        }

        if(StartMotion != null)
            SwitchMotion(StartMotion);
    }

    public bool IsCurrentMotion<T>() where T : MotionBase
    {
        T motion = GetComponentInChildren<T>();
        if (motion != null)
            return motion == CurrentMotion;

        return false;
    }
    public void SwitchMotion<T>() where T : MotionBase
    {
        T motion = GetComponentInChildren<T>();
        if(motion != null)
            SwitchMotion(motion.ID);
    }
    public void SwitchMotion(MotionBase motion)
    {
        SwitchMotion(motion.ID);
    }
    public void SwitchMotion(int motionID)
    {
        if (!mMotions.ContainsKey(motionID)) return;
        if (Lock) return;

        if (CurrentMotionID != 0)
            mMotions[CurrentMotionID].OnLeave();

        CurrentMotionID = motionID;
        CurrentMotion.OnEnter();
    }
    public T FindMotion<T>() where T : MotionBase
    {
        return GetComponentInChildren<T>();
    }

    void Update() 
    {
        if (Lock) return;

        foreach (var each in mMotions)
        {
            MotionBase motion = each.Value;
            if(CurrentMotionID == motion.ID)
                continue;

            if(motion.OnReady())
            {
                SwitchMotion(motion);
                break;
            }
        }

        if(CurrentMotionID != 0)
            CurrentMotion.OnUpdate();
    }
}
