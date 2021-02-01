using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState { None, Create, Idle, Move, Attack, Stun, Death, Disappear, CastMainSkill, CastSubSkill}

public class UserUnit : RGameObject, IUserInputReciever
{
    public UnitState State = UnitState.None;
    public GameObjectProperty Property = new GameObjectProperty();
    public Animator Anim;

    public override void Init()
    {
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
        switch (State)
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
        MobUnit[] mobs = DetectAroundMob(Property.AttackRange);
        if(mobs.Length > 0)
            ChangeToAttack(mobs[0]);
    }

    public virtual void ChangeToCreate() { State = UnitState.Create; }
    public virtual void ChangeToDisappear() { State = UnitState.Disappear; }
    public virtual void ChangeToIdle() { State = UnitState.Idle; }
    public virtual void ChangeToMove(Vector3 worldPos)
    {
        State = UnitState.Move;
        StartCoroutine("MoveTo", worldPos);
    }
    public virtual void ChangeToAttack(MobUnit target)
    {
        State = UnitState.Attack;
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
                    continue;
                list.Add(mob);
            }
            MobUnit[] rets = list.ToArray();
            return rets;
        }
        return default;
    }

    private IEnumerator MoveTo(Vector3 dest)
    {
        //dest지점으로 유닛 Smoothly 이동
        float moveSpeed = Property.MoveSpeed;
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
        transform.position = dest;
        ChangeToIdle();
    }

    private IEnumerator Attack(MobUnit enemy)
    {
        //공격 속도에 따라 반복적으로 공격을 수행하는 동작
        float waitSecForNextAttack = 1 / Property.AttackSpeed;

        while (true)
        {
            if (enemy == null || enemy.State == UnitState.Death)
                break;

            Vector2 dist = enemy.transform.position - transform.position;
            if (dist.magnitude > Property.AttackRange)
            {
                MobUnit[] enemies = DetectAroundMob(Property.AttackRange);
                if (enemies.Length <= 0)
                    break;

                enemy = enemies[0];
            }
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
            GetComponent<Animator>().Play("attack_up", -1, 0);
        else if (deg < 120)
            GetComponent<Animator>().Play("attack_mid", -1, 0);
        else
            GetComponent<Animator>().Play("attack_down", -1, 0);
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
