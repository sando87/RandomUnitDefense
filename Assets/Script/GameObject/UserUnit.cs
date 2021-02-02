using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState { None, Create, Idle, Move, Attack, Stun, Death, Disappear, CastMainSkill, CastSubSkill}

public class UserUnit : RGameObject, IUserInputReciever
{
    [SerializeField] private Animator Anim = null;
    [SerializeField] private UserUnitSpec Spec = null;

    public UnitState CurrentState { get; private set; }

    public override void Init()
    {
        CurrentState = UnitState.Idle;
    }

    public override void Release()
    {
    }

    public virtual void OnClick()
    {
    }

    public virtual void OnDragAndDrop(Vector3 worldPos)
    {
        ChangeToMove(worldPos);
    }

    public virtual void OnDragging(Vector3 worldPos)
    {
    }


    void Update()
    {
        switch (CurrentState)
        {
            case UnitState.Create: UpdateCreate(); break;
            case UnitState.Idle: UpdateIdle(); break;
            case UnitState.Move: UpdateMove(); break;
            case UnitState.Attack: UpdateAttack(); break;
            case UnitState.Disappear: UpdateDisappear(); break;
        }
    }

    protected virtual void UpdateCreate() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateDisappear() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateIdle()
    {
        MobUnit[] mobs = DetectAroundMob(Spec.AttackRange);
        if(mobs != null)
            ChangeToAttack(mobs[0]);
    }

    public virtual void ChangeToCreate() { CurrentState = UnitState.Create; }
    public virtual void ChangeToDisappear() { CurrentState = UnitState.Disappear; }
    public virtual void ChangeToIdle()
    {
        CurrentState = UnitState.Idle;
        Anim.SetTrigger("idle");
    }
    public virtual void ChangeToMove(Vector3 worldPos)
    {
        CurrentState = UnitState.Move;
        Anim.SetTrigger("move");
        StartCoroutine("MoveTo", worldPos);
    }
    public virtual void ChangeToAttack(MobUnit target)
    {
        CurrentState = UnitState.Attack;
        StartCoroutine("Attack", target);
    }



    private MobUnit[] DetectAroundMob(float range)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        if (hitColliders.Length > 1)
        {
            List<MobUnit> list = new List<MobUnit>();
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject)
                    continue;

                Vector2 dir = hitCollider.transform.position - transform.position;
                if (dir.magnitude >= range)
                    continue;

                MobUnit mob = hitCollider.GetComponent<MobUnit>();
                if (mob != null)
                    list.Add(mob);
            }

            if (list.Count > 0)
                return list.ToArray();
        }
        return null;
    }

    private IEnumerator MoveTo(Vector3 dest)
    {
        //dest지점으로 유닛 Smoothly 이동
        float moveSpeed = Spec.MoveSpeed;
        Vector3 dir = dest - transform.position;
        dir.z = 0;
        float distance = dir.magnitude;
        dir.Normalize();
        float duration = distance / moveSpeed;
        float time = 0;
        LookAt(dest);
        while (time < duration)
        {
            transform.position += (dir * moveSpeed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        dest.z = 0;
        transform.position = dest;
        ChangeToIdle();
    }

    private IEnumerator Attack(MobUnit enemy)
    {
        //공격 속도에 따라 반복적으로 공격을 수행하는 동작
        float waitSecForNextAttack = 1 / Spec.AttackSpeed;

        while (true)
        {
            if (enemy == null || enemy.CurrentState == UnitState.Death)
                break;

            Vector2 dist = enemy.transform.position - transform.position;
            if (dist.magnitude > Spec.AttackRange)
            {
                MobUnit[] enemies = DetectAroundMob(Spec.AttackRange);
                if (enemies == null)
                    break;

                enemy = enemies[0];
            }
            enemy.GetDamaged(Spec);
            LookAt(enemy.transform.position);
            PlayAttackAnimAimmingTarget(enemy.transform.position);
            yield return Yielders.GetWaitForSeconds(waitSecForNextAttack);
        }
        ChangeToIdle();
    }

    private void LookAt(Vector3 pos)
    {
        if (pos.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
    private void PlayAttackAnimAimmingTarget(Vector3 pos)
    {
        //y축+ 기준으로 3등분하여 60도 이하면 up방향 애님, 120도 이하면 mid방향 애님, 그 외 down방향 애님 재생
        Vector2 refDir = new Vector2(0, 1);
        Vector2 dir = pos - transform.position;
        dir.Normalize();
        float dot = Vector2.Dot(refDir, dir);
        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if (deg < 60)
            Anim.SetTrigger("attack1");
        else if (deg < 120)
            Anim.SetTrigger("attack2");
        else
            Anim.SetTrigger("attack3");
    }


    //공격모션 애니메이션 중 실제 유효한 모션에 호출되는 함수
    //public void OnAnimFired(int sequence)
    //{
    //    float ranOffX = UnityEngine.Random.Range(-0.1f, 0.1f);
    //    float ranOffY = UnityEngine.Random.Range(-0.1f, 0.1f);
    //    Skill skillObj = Instantiate<Skill>(BasicSkillPrefab);
    //    skillObj.Owner = gameObject;
    //    skillObj.Target = GetComponent<FSM>().Param.AttackTarget;
    //    skillObj.StartPos = transform.position;
    //    skillObj.EndPos = skillObj.Target.transform.position + new Vector3(ranOffX, 0.2f + ranOffY, 0);
    //    skillObj.Damage = GetComponent<Stats>().AttackDamage;
    //}
}
