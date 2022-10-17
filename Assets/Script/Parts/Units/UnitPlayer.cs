using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 때리면 폭탄
// 멀티샷, 멀티 미사일
// 연쇄 스킬(번개, 사슬 등등)
// 시비르 부메랑(멀리 떨어질수록 뎀지 증가)
// 타게팅 집중용 캐릭

public class UnitPlayer : UnitBase
{
    private LaserAimming mBuffEffectLine = null;

    protected IEnumerator KeepBuff(BuffBase buffPrefab)
    {
        mBaseObj.GetComponentInChildren<UserInput>().EventSelected += () => ShowPassiveBuffEffect();
        mBaseObj.GetComponentInChildren<UserInput>().EventDeSelected += () => HidePassiveBuffEffect();

        while(true)
        {
            yield return newWaitForSeconds.Cache(1);
            Collider col = mBaseObj.DetectMostCloseAround(1, 1 << mBaseObj.gameObject.layer);
            if(col != null)
            {
                col.GetBaseObject().BuffCtrl.ApplyBuff(buffPrefab, 1.1f);
            }

            if(mBuffEffectLine != null)
                mBuffEffectLine.Target = col == null ? null : col.GetBaseObject().gameObject;
        }
    }

    void ShowPassiveBuffEffect()
    {
        Vector3 startPosition = mBaseObj.Body.Center;
        mBuffEffectLine = LaserAimming.Play(startPosition, null, "ChannellerLaser");
        mBuffEffectLine.transform.SetParent(mBaseObj.Body.transform);

    }
    void HidePassiveBuffEffect()
    {
        if(mBuffEffectLine != null)
        {
            Destroy(mBuffEffectLine.gameObject);
            mBuffEffectLine = null;
        }
    }
}
