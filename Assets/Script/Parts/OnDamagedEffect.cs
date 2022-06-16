using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OnDamagedEffect : MonoBehaviour
{
    [SerializeField][PrefabSelector(Consts.VFXPath)] string _DeadVFXPrefab = ""; //죽을때 보여줄 이펙트(파티클)
    [SerializeField][SFXSelector] string _DeadSFX = ""; //죽을때 사운드 효과

    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();
        mBaseObject.Health.EventDamaged += OnDamaged;
    }

    private void OnDamaged(float validDamage, BaseObject attacker)
    {
        if (validDamage > 0)
        {
            PlayDeadEffect(attacker);
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
