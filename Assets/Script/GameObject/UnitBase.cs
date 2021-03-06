﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : RGameObject
{
    [SerializeField] private SpecProperty Spec = new SpecProperty();

    public Animator Anim { get; set; }
    public SpriteRenderer SR { get; set; }
    public BoxCollider2D RectCollider { get; set; }

    public Vector3 Center { get { return transform.position + new Vector3(0, Height * 0.5f, 0); } }
    public float Height { get { return RectCollider.size.y; } }
    public ref readonly SpecProperty BasicSpec { get { return ref Spec; } }
    public UnitState CurrentState { get { return FSM.CurrentState; } }
    public int Level { get => LevelObject.GetLevel(); set => LevelObject.SetLevel(value); }
    private LevelDisplay LevelObject = null;

    public FiniteStateMachine FSM = new FiniteStateMachine();   //각 Motion(Idle, Move, Attack 등)을 스위칭 해주며 관리한다.
    public BuffController BuffCtrl = new BuffController();      //현재 유닛에 걸린 버프효과들 관리(지속시간이 지나면 소멸)
    public BuffProperty BuffValues = new BuffProperty();        //유닛의 버프로 제어가능한 속성
    public UnitProperty Property = new UnitProperty();          //유닛의 기본 속성

    public override void Init()
    {
        Anim = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
        RectCollider = GetComponent<BoxCollider2D>();

        InstanceLevelDisplay();
        Property.Init(this);
        BuffCtrl.Init(this);
        FSM.InitMotions(this);
    }
    public override void Release() { }

    protected virtual void Update()
    {
        BuffCtrl.UpdateBuffObjects();
        FSM.UpdateMotions();
        UpdateWorldPosZ();
    }

    public void UpdateWorldPosZ()
    {
        //2.5D 특성상 y값이 높을수록 카메라보다 더 멀리 배치되도록 하기 위한 z값 조절(그라운드 위 모든 유닛에게 적용 필요)
        Vector3 pos = transform.position;
        pos.z = pos.y * 0.1f;
        transform.position = pos;
    }
    public T[] DetectAround<T>(float range) where T : UnitBase
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        if (hitColliders.Length > 1)
        {
            List<T> list = new List<T>();
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject)
                    continue;

                Vector2 dir = hitCollider.transform.position - transform.position;
                if (dir.magnitude >= range)
                    continue;

                T mob = hitCollider.GetComponent<T>();
                if (mob != null && mob.CurrentState != UnitState.Death)
                    list.Add(mob);
            }

            if (list.Count > 0)
                return list.ToArray();
        }
        return null;
    }
    public void TurnHead(Vector3 target)
    {
        //대상이 나보다 왼쪽에 있으면 좌우 반전시킨다.
        SR.flipX = target.x < transform.position.x;
    }
    public float CalcVerticalDegree(Vector3 targetPos)
    {
        //y+축 기준으로 대상까지의 각도 계산(0~180도)
        Vector2 refDir = Vector2.up;
        Vector2 dir = targetPos - transform.position;
        dir.Normalize();
        float dot = Vector2.Dot(refDir, dir);
        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        return deg;
    }


    private void InstanceLevelDisplay()
    {
        if (LevelObject == null)
        {
            LevelObject = Resources.Load<LevelDisplay>("Prefabs/@LevelDisplay");
            LevelObject = Instantiate(LevelObject, transform);
            LevelObject.SetLevel(1);
        }
    }
}
