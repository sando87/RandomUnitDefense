using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] int _MaxHealth = 100;
    [SerializeField] int _Armor = 0;
    [SerializeField] bool _Destroyable = false; // 죽으면 객체가 파괴되는지 여부
    [SerializeField][PrefabSelector(Consts.VFXPath)] string _DeadVFXPrefab = ""; //죽을때 보여줄 이펙트(파티클)
    [SerializeField][SFXSelector] string _DeadSFX = ""; //죽을때 사운드 효과

    public event Action<float, BaseObject> EventDamaged = null;

    public bool IsImmortal { get; set; }
    public bool IsArmorEnable { get; set; } = true;
    public bool IsDead { get { return CurrentHealth <= 0; } }
    public int CurrentHealth { get; private set; } = 0;
    public float CurrentHealthRate { get { return (float)CurrentHealth / _MaxHealth; } }
    public BaseObject LastAttacker { get; private set; } = null;
    public Vector3 LastHitPoint { get; set; } = Vector3.zero;

    private BaseObject mBaseObject = null;


    void Start()
    {
        mBaseObject = this.GetBaseObject();
        CurrentHealth = _MaxHealth;
    }

    public void GetDamaged(BaseObject attacker)
    {
        if(IsDead) return;

        int damage = (int)attacker.SpecProp.AttackDamage;
        if(IsImmortal)
            damage = 0;

        if(IsArmorEnable)
        {
            damage -= _Armor;
            if (damage < 0)
                damage = 0;
        }

        LastAttacker = attacker;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        EventDamaged?.Invoke(damage, attacker);

        if (IsDead && _Destroyable)
        {
            PlayDeadEffect(attacker);

            Destroy(mBaseObject.gameObject);
        }
    }

    public void PlayDeadEffect(BaseObject attacker = null)
    {
        GameObject ps = ObjectPooling.Instance.InstantiateVFX(_DeadVFXPrefab, mBaseObject.Body.Center, Quaternion.identity);
        if (ps != null)
        {
            if(attacker != null)
            {
                int dir = (attacker.transform.position.x < mBaseObject.transform.position.x) ? 1 : -1;
                ps.transform.localScale = new Vector3(dir, 1, 1);
            }

            ps.ReturnAfter(3);
        }

        SoundPlayManager.Instance.PlayInGameSFX(_DeadSFX);
    }

    public void ShowHideHealthBar(bool isShow)
    {
        
    }

}
