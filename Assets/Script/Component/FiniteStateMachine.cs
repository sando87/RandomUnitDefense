using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState { None, Appear, Idle, Move, Attack, Stun, Death, Disappear }

public abstract class MotionBase : MonoBehaviour
{
    public UnitBase Unit { get; set; }
    public abstract UnitState State { get; }
    public abstract bool IsReady();

    public virtual void OnInit() { }
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnLeave() { }
}

public class FiniteStateMachine
{
    public UnitState CurrentState { get { return CurrentMotion ? CurrentMotion.State : UnitState.None; } }
    private MotionBase CurrentMotion = null;
    private Dictionary<UnitState, MotionBase> Motions = new Dictionary<UnitState, MotionBase>();
    public void InitMotions(UnitBase owner)
    {
        Motions.Clear();
        MotionBase[] motions = owner.GetComponents<MotionBase>();
        foreach (MotionBase motion in motions)
        {
            motion.Unit = owner;
            motion.OnInit();
            Motions[motion.State] = motion;
        }
    }
    public void ChangeState(UnitState state)
    {
        if (!Motions.ContainsKey(state))
            return;

        if (CurrentMotion != null)
            CurrentMotion.OnLeave();

        CurrentMotion = Motions[state];
        CurrentMotion.OnEnter();
    }
    public void UpdateMotions()
    {
        if (CurrentMotion != null)
            CurrentMotion.OnUpdate();
    }
}
