using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionAttack : MotionBase
{
    [SerializeField] private AnimationClip ReferenceAnim;

    private MobUnit Target = null;
    private float nextAttackTime = 0;

    public override UnitState State { get { return UnitState.Attack; } }
    public override bool IsReady()
    {
        if (nextAttackTime > Time.realtimeSinceStartup)
            return false;

        if(Target != null && Target.CurrentState != UnitState.Death)
        {
            if ((Target.transform.position - transform.position).magnitude < Unit.Spec.AttackRange)
                return true;
            else
                Target = null;
        }

        MobUnit[] mobs = Unit.DetectAround<MobUnit>(Unit.Spec.AttackRange);
        if (mobs == null)
            return false;

        Target = mobs[0];
        return true;
    }

    public override void OnEnter()
    {
        nextAttackTime = Time.realtimeSinceStartup + (1 / Unit.Spec.AttackSpeed);
        Unit.TurnHead(Target.transform.position);

        //대상의 위치에 따라 재생되는 attack 애니메이션을 다르게 해줘야 한다.
        int animClipCount = 3;
        float deg = Unit.CalcVerticalDegree(Target.transform.position);
        int stepDegree = 180 / animClipCount;
        int animIndex = (int)deg / stepDegree;
        Unit.Anim.SetTrigger("attack" + (animIndex + 1));

        float animPlayTime = ReferenceAnim.length;
        Invoke("OnFired", animPlayTime * 0.3f);
        Invoke("OnAnimationEnd", animPlayTime);
    }
    public override void OnLeave()
    {
        CancelInvoke("OnFired");
        CancelInvoke("OnAnimationEnd");
    }

    private void OnFired()
    {
        if (Target == null)
            return;

        Target.GetDamaged(Unit.Spec);
        //float ranOffX = UnityEngine.Random.Range(-0.1f, 0.1f);
        //float ranOffY = UnityEngine.Random.Range(-0.1f, 0.1f);
        //Skill skillObj = Instantiate<Skill>(BasicSkillPrefab);
        //skillObj.Owner = gameObject;
        //skillObj.Target = GetComponent<FSM>().Param.AttackTarget;
        //skillObj.StartPos = transform.position;
        //skillObj.EndPos = skillObj.Target.transform.position + new Vector3(ranOffX, 0.2f + ranOffY, 0);
        //skillObj.Damage = GetComponent<Stats>().AttackDamage;
    }
    private void OnAnimationEnd()
    {
        Unit.FSM.ChangeState(UnitState.Idle);
    }
}

