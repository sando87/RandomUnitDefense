using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] int _MaxHealth = 100;
    [SerializeField] int _Armor = 0;
    [SerializeField] bool _Destroyable = false; // 죽으면 객체가 파괴되는지 여부
    [SerializeField][PrefabSelector(Consts.VFXPath)] string _DeadVFXPrefab = ""; //죽을때 보여줄 이펙트(파티클)
    [SerializeField][SFXSelector] string _DeadSFX = ""; //죽을때 사운드 효과
    [SerializeField] float _DeadVelocity = 0; //땅에 떨어져 충돌시 해당 속도 이상이면 데미지를 입게되어 죽는다.

    public event Action<float, BaseObject> EventDamaged = null;

    public bool IsImmortal { get; set; }
    public bool IsArmorEnable { get; set; } = true;
    public bool IsDead { get { return CurrentHealth <= 0; } }
    public int CurrentHealth { get; private set; } = 0;
    public float CurrentHealthRate { get { return (float)CurrentHealth / _MaxHealth; } }
    public BaseObject LastAttacker { get; private set; } = null;
    public Vector3 LastHitPoint { get; set; } = Vector3.zero;

    public PlayerBase myPlayerState;

    private BaseObject mBaseObject = null;

    private Tween shineTween = null;
    private Tween flickerTween = null;

    // !! UPC H2 로봇은 구조가 달라서 다르게 적용해야함
    private Renderer myRrenderer = null;
    private Renderer gunRrenderer = null;

    void Start()
    {
        myRrenderer = this.GetBaseObject().GetComponentInChildren<Renderer>();

        if(myRrenderer.gameObject.transform.childCount > 0)
        {
            gunRrenderer = myRrenderer.gameObject.GetComponentInChildren<Renderer>();
        }

        mBaseObject = this.GetBaseObject();
        myPlayerState = mBaseObject.PlayerState;
        CurrentHealth = _MaxHealth;

        if (_DeadVelocity > 0)
            mBaseObject.CharacterPhy.EventCrushed += OnPhysicsCrushed;
    }

    void IDamagable.OnDamaged(int damage, BaseObject attacker)
    {
        if(IsDead) return;

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

    // 떨이지면서 지면과 충돌시 호출됨(CharacterPhysics.cs 파일에서 처리)
    private void OnPhysicsCrushed(Vector3 crushedVelocity)
    {
        // 특정 속도 이상으로 강하게 떨어지면 100의 데미지를 입는다.
        if(crushedVelocity.y < _DeadVelocity * -1)
        {
            ((IDamagable)this).OnDamaged(100, mBaseObject);
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

}
