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

    void Start()
    {
        mGameSystem = InGameSystem.Instance;
        mBuff = this.GetBaseObject().BuffProp;
    }

    public float Damage
    {
        get
        {
            float damagePerUp = _Spec.damagesPerUp[Level - 1];
            float damage = _Spec.damage + mGameSystem.GetUpgradeCount(_Spec.attackType) * damagePerUp;
            return damage * mBuff.AttackDamage;
        }
    }
    public float TotalHP { get { return _Spec.totalHP * mBuff.TotalHP; } }
    public float Armor { get { return _Spec.armor * mBuff.Armor; } }
    public float MoveSpeed { get { return _Spec.moveSpeed * mBuff.MoveSpeed; } }
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
}