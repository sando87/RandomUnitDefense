using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecProperty : MonoBehaviour
{
    [SerializeField] BasicSpec _Spec = new BasicSpec();

    private BuffProperty mBuff = null;
    private InGameSystem mGameSystem = null;

    public int Level { get; set; } = 1;
    public BuffProperty Buff { get { if (mBuff == null) mBuff = this.GetBaseObject().BuffProp; return mBuff; } }

    void Start()
    {
        mGameSystem = InGameSystem.Instance;
    }

    public float Damage
    {
        get
        {
            float damagePerUp = _Spec.damagesPerUp[Level - 1];
            float damage = _Spec.damage + mGameSystem.GetUpgradeCount(_Spec.attackType) * damagePerUp;
            return damage * Buff.AttackDamage;
        }
    }
    public float TotalHP { get { return _Spec.totalHP * Buff.TotalHP; } }
    public float Armor { get { return _Spec.armor * Buff.Armor; } }
    public float MoveSpeed { get { return _Spec.moveSpeed * Buff.MoveSpeed; } }
    public string DamageInfo { get { return _Spec.DamageInfo; } }
    public UpgradeType DamageType { get { return _Spec.attackType; } }
}


[Serializable]
public class BasicSpec
{
    public float totalHP = 100;
    public float armor = 0;
    public float moveSpeed = 3;
    public UpgradeType attackType = UpgradeType.TypeA;
    public float damage = 10;
    public float[] damagesPerUp = new float[5] { 1, 2, 3, 4, 5 };
    public string DamageInfo { get { return damage + "+" + String.Join("/", damagesPerUp); } }
}