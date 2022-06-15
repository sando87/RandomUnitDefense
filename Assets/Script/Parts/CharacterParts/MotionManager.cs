using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionManager : MonoBehaviour
{
    [SerializeField] MotionBasic StartMotion = null;

    private Dictionary<int, MotionBasic> mMotions = new Dictionary<int, MotionBasic>();

    public int CurrentMotionID { get; private set; } = 0;
    public MotionBasic CurrentMotion { get { return mMotions[CurrentMotionID]; } }
    public bool Lock { get; set; } = false;

    void Awake()
    {
        MotionBasic[] motions = GetComponentsInChildren<MotionBasic>();
        foreach (MotionBasic motion in motions)
        {
            motion.OnInit();
            mMotions[motion.GetInstanceID()] = motion;
        }

        if(StartMotion != null)
            SwitchMotion(StartMotion);
    }

    public bool IsCurrentMotion<T>() where T : MotionBasic
    {
        T motion = GetComponentInChildren<T>();
        if (motion != null)
            return motion == CurrentMotion;

        return false;
    }
    public void SwitchMotion<T>() where T : MotionBasic
    {
        T motion = GetComponentInChildren<T>();
        if(motion != null)
            SwitchMotion(motion.ID);
    }
    public void SwitchMotion(MotionBasic motion)
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
    public T FindMotion<T>() where T : MotionBasic
    {
        return GetComponentInChildren<T>();
    }

    void Update() 
    {
        if (Lock) return;

        foreach (var each in mMotions)
        {
            MotionBasic motion = each.Value;
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
