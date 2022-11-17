using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEnemy : UnitBase
{
    [SerializeField] Sprite[] HitEffect = null;

    public int WaveNumber { get; set; } = 0;

    private Vector3[] mWayPoints = null;
    private int mWayPointIndex = 0;
    private MotionMove mMotionMove = null;

    void Start()
    {
        mWayPoints = InGameSystem.Instance.GetWayPoints();
        mWayPointIndex = 0;
        mMotionMove = mBaseObj.MotionManager.FindMotion<MotionMove>();
        if(HitEffect != null)
            mBaseObj.Health.EventDamaged += OnDamaged;

        float hp = GetBalanceTable();
        
        mBaseObj.Health.InitHP(hp);
        StartCoroutine(MoveAround());
    }

    float GetBalanceTable()
    {
        float hp = 0;
        if(WaveNumber >= 50) hp = 100000 * WaveNumber;
        else if (WaveNumber >= 40) hp = 70030 * WaveNumber;
        else if (WaveNumber >= 39) hp = 65030 * WaveNumber;
        else if (WaveNumber >= 38) hp = 60030 * WaveNumber;
        else if (WaveNumber >= 37) hp = 55030 * WaveNumber;
        else if (WaveNumber >= 36) hp = 50030 * WaveNumber;
        else if (WaveNumber >= 35) hp = 45030 * WaveNumber;
        else if (WaveNumber >= 34) hp = 40020 * WaveNumber;
        else if (WaveNumber >= 33) hp = 35150 * WaveNumber;
        else if (WaveNumber >= 32) hp = 30030 * WaveNumber;
        else if (WaveNumber >= 31) hp = 25720 * WaveNumber;
        else if (WaveNumber >= 30) hp = 20000 * WaveNumber;
        
        else if (WaveNumber >= 29) hp = 15000 * WaveNumber;
        else if (WaveNumber >= 28) hp = 15000 * WaveNumber;

        else if (WaveNumber >= 27) hp = 15000 * WaveNumber;
        else if (WaveNumber >= 26) hp = 14000 * WaveNumber;
        else if (WaveNumber >= 25) hp = 12000 * WaveNumber;
        else if (WaveNumber >= 24) hp = 10000 * WaveNumber;
        else if (WaveNumber >= 23) hp = 8600 * WaveNumber;
        else if (WaveNumber >= 22) hp = 7200 * WaveNumber;
        else if (WaveNumber >= 21) hp = 5800 * WaveNumber;
        else if (WaveNumber >= 20) hp = 5800 * WaveNumber;
        else if (WaveNumber >= 19) hp = 5800 * WaveNumber;
        else if (WaveNumber >= 18) hp = 5800 * WaveNumber;
        else if (WaveNumber >= 17) hp = 5800 * WaveNumber;
        else if (WaveNumber >= 16) hp = 4600 * WaveNumber;
        else if (WaveNumber >= 15) hp = 3400 * WaveNumber;
        else if (WaveNumber >= 14) hp = 2200 * WaveNumber;

        else if (WaveNumber >= 11) hp = 1750 * WaveNumber;
        else if (WaveNumber >= 10) hp = 1400 * WaveNumber;
        else if (WaveNumber >= 9) hp = 1050 * WaveNumber;
        else if (WaveNumber >= 8) hp = 700 * WaveNumber;
        else if (WaveNumber >= 7) hp = 350 * WaveNumber;

        else if (WaveNumber >= 4) hp = 220 * WaveNumber;
        else if (WaveNumber >= 3) hp = 130 * WaveNumber;
        else if (WaveNumber >= 2) hp = 55 * WaveNumber;
        else hp = 35 * WaveNumber;

        // 전반적으로 좀 잘 나온편...
        // 4w +4 20up
        // 11w +7 75up
        // 15w +13 110up
        // 26w +13 160up 100마리 2/2/2
        // 31w +18 195up 10마리 3/2/0
        // 36w +18 210up 150마리 끝... 3/3/0
        // ====== 레벨업 우선 =====
        // 4w +1 15up
        // 10w +3 50up 2/1/1
        // 21w +4 80up 2/2/2 끝...

        return hp;
    }

    private void OnDamaged(float valideDamage, BaseObject attacker)
    {
        // if(!mBaseObj.Health.IsDead && valideDamage > 0)
        // {
        //     Vector3 pos = MyUtils.Random(mBaseObj.Body.Center, 0.1f);
        //     SpritesAnimator effect = SpritesAnimator.Play(pos, HitEffect);
        //     effect.transform.SetParent(mBaseObj.transform);
        // }
    }

    IEnumerator MoveAround()
    {
        yield return null;

        while (true)
        {
            Vector3 dest = mWayPoints[mWayPointIndex];
            mMotionMove.Destination = dest;
            mBaseObj.MotionManager.SwitchMotion(mMotionMove);

            yield return new WaitUntil(() => mBaseObj.MotionManager.IsCurrentMotion<MotionIdle>());

            if((mBaseObj.transform.position - dest).sqrMagnitude < 0.1)
                mWayPointIndex = (mWayPointIndex + 1) % mWayPoints.Length;
                
            yield return null;
        }
    }


}
