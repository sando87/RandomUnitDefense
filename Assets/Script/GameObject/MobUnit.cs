using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobUnit : RGameObject, IUserInputReciever
{
    [SerializeField] private Animator Anim = null;
    [SerializeField] private HealthBar HPBar = null;

    public GameStats Stats = new GameStats();
    public UnitState CurrentState { get; private set; }
    private float CurrentHP = 0;

    public override void Init()
    {
        CurrentHP = Stats.TotalHP;
        CurrentState = UnitState.Idle;
        HPBar.Init(Stats.CharacterHeight, HealthBarSize.Medium);

        Vector3[] waypoints = new Vector3[] {
            new Vector3(-5.5f, -2.5f, 0),
            new Vector3(5.5f, -2.5f, 0),
            new Vector3(5.5f, 2.5f, 0),
            new Vector3(-5.5f, 2.5f, 0)
        };
        StartCoroutine(MoveAround(waypoints));

    }

    public override void Release()
    {
    }

    public virtual void OnClick()
    {
    }

    public virtual void OnDragAndDrop(Vector3 worldPos)
    {
    }

    public virtual void OnDragging(Vector3 worldPos)
    {
    }



    public void GetDamaged(GameStats attacker)
    {
        CurrentHP -= attacker.AttackDamage;
        HPBar.UpdateHealthBar(CurrentHP / Stats.TotalHP);
        if(CurrentHP <= 0)
        {
            CurrentHP = 0;
            StopAllCoroutines();
            Anim.SetTrigger("death");
            CurrentState = UnitState.Death;
            Destroy(gameObject, 1.0f);
        }
    }

    private IEnumerator MoveAround(Vector3[] waypoints)
    {
        int idx = 0;
        Anim.SetTrigger("move");
        while (true)
        {
            //dest지점으로 유닛 Smoothly 이동
            Vector3 dest = waypoints[idx];
            LookAt(dest);
            float moveSpeed = Stats.MoveSpeed;
            Vector3 dir = dest - transform.position;
            dir.z = 0;
            float distance = dir.magnitude;
            dir.Normalize();
            float duration = distance / moveSpeed;
            float time = 0;
            while (time < duration)
            {
                transform.position += (dir * moveSpeed * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = dest;
            idx = (idx + 1) % waypoints.Length;
            yield return null;
        }
    }


    private void LookAt(Vector3 pos)
    {
        if (pos.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

}
