using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobUnit : RGameObject, IUserInputReciever
{
    public UnitState State = UnitState.None;
    public GameObjectProperty Property = new GameObjectProperty();
    public HealthBar HPBar;

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
    }

    public virtual void OnDragging(Vector3 worldPos)
    {
    }

    public void GetDamaged(GameObjectProperty attacker)
    {

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
        while (time < duration)
        {
            transform.position += (dir * moveSpeed * Time.deltaTime);
            time += Time.deltaTime;
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
