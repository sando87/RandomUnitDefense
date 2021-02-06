using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionAttack2 : MotionBase
{
    [SerializeField] private AnimationClip ReferenceAnim = null;
    [SerializeField] private GameObject FiredParticle = null;
    [SerializeField] private int AnimCount = 1;

    private UnitMob Target = null;
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

        UnitMob[] mobs = Unit.DetectAround<UnitMob>(Unit.Spec.AttackRange);
        if (mobs == null)
            return false;

        Target = mobs[0];
        return true;
    }

    public override void OnEnter()
    {
        Unit.TurnHead(Target.transform.position);

        //대상의 위치에 따라 재생되는 attack 애니메이션을 다르게 해줘야 한다.
        float deg = Unit.CalcVerticalDegree(Target.transform.position);
        int stepDegree = 180 / AnimCount;
        int animIndex = (int)deg / stepDegree;
        Unit.Anim.SetTrigger("attack" + (animIndex + 1));

        //float timePerAttack = 1 / Unit.Spec.AttackSpeed;
        //if (timePerAttack < ReferenceAnim.length)
        //    Unit.Anim.SetFloat("attackSpeed", ReferenceAnim.length / timePerAttack);
        //else
        //    Unit.Anim.SetFloat("attackSpeed", 1);

        float animSpeed = Unit.Anim.GetFloat("attackSpeed");
        float animPlayTime = ReferenceAnim.length / animSpeed;
        Invoke("OnFiredFirst", animPlayTime * 0.1f);
        Invoke("OnFiredSecond", animPlayTime * 0.6f);
        Invoke("OnAnimationEnd", animPlayTime);
    }
    public override void OnLeave()
    {
        CancelInvoke("OnFiredFirst");
        CancelInvoke("OnFiredSecond");
        CancelInvoke("OnAnimationEnd");
    }

    private void OnFiredFirst()
    {
        if (Target == null)
            return;

        nextAttackTime = Time.realtimeSinceStartup + (1 / Unit.Spec.AttackSpeed);
        Target.GetDamaged(Unit.Spec);

        Vector3 pos = Target.Center;
        pos.x += Random.Range(-0.1f, 0.1f);
        pos.y += Random.Range(-0.1f, 0.1f);
        GameObject obj = Instantiate(FiredParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }
    private void OnFiredSecond()
    {
        if (Target == null)
            return;

        Vector3 pos = Target.Center;
        pos.x += Random.Range(-0.1f, 0.1f);
        pos.y += Random.Range(-0.1f, 0.1f);
        GameObject obj = Instantiate(FiredParticle, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }
    private void OnAnimationEnd()
    {
        Unit.FSM.ChangeState(UnitState.Idle);
    }
}

