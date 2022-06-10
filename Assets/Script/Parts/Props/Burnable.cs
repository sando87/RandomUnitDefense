using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    [SerializeField][PrefabSelector(Consts.VFXPath)] string BurningVFX = "";

    BaseObject mBaseObject = null;
    List<GameObject> mBurnVFXs = new List<GameObject>();

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        Health hp = mBaseObject.Health;
        if(hp != null)
        {
            hp.EventDamaged += OnDamaged;
        }
    }

    void OnDamaged(float validDamage, BaseObject attacker)
    {
        if(mBaseObject.Health.IsDead)
        {
            foreach(GameObject ps in mBurnVFXs)
                ps.ReturnAfter();

        }
        else if(validDamage > 0)
        {
            StartCoroutine(CoDamaged());
        }
    }

    IEnumerator CoDamaged()
    {
        Vector3 randomPos = MyUtils.Random(mBaseObject.Body.Collider.GetWorldBounds2D(), 0.5f);
        randomPos.z = mBaseObject.Body.Center.z;
        GameObject ps = ObjectPooling.Instance.InstantiateVFX(BurningVFX, randomPos, Quaternion.identity);
        mBurnVFXs.Add(ps);

        yield return new WaitForSeconds(0.3f);

        IDamagable health = mBaseObject.Health.GetComponentInChildren<IDamagable>();
        if (health != null)
        {
            health.OnDamaged(1, mBaseObject);
        }
    }

}
