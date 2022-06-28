using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitShellstorm : UnitBase
{
    [SerializeField] GameObject MissilePrefab = null;
    [SerializeField] Transform MissileStartPosition = null;
    [SerializeField] GameObject BarrelPrefab = null;
    [SerializeField] Transform BarrelStartPosition = null;
    
    void Start()
    {
        mBaseObj.MotionManager.SwitchMotion<MotionAppear>();

        GetComponent<MotionActionSingle>().EventFired = OnAttack;
    }

    private void OnAttack(int idx)
    {
        GameObject missile = Instantiate(MissilePrefab, MissileStartPosition.position, Quaternion.identity);
        //missile.EventHit = OnExplosionMssile;
    }

    void OnExplosionMssile(GameObject missile)
    {
        Collider[] cols = Physics.OverlapSphere(missile.transform.position, 0.5f, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            }
        }
        Destroy(missile);
    }

    void OnExplosionGasBarrel(GameObject barrel)
    {
        Collider[] cols = Physics.OverlapSphere(barrel.transform.position, 1.0f, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            Health hp = col.GetComponentInBaseObject<Health>();
            if (hp != null)
            {
                hp.GetDamaged(mBaseObj.SpecProp.Damage, mBaseObj);
            }
        }
        Destroy(barrel);

    }
}
