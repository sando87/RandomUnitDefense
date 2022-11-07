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


        float hp = 0;
        if(WaveNumber >= 50)
            hp = 100000 * WaveNumber;
        else if (WaveNumber >= 35) hp = 25030 * WaveNumber;
        else if (WaveNumber >= 34) hp = 17020 * WaveNumber;
        else if (WaveNumber >= 33) hp = 11150 * WaveNumber;
        else if (WaveNumber >= 21) hp = 5030 * WaveNumber;
        else if (WaveNumber >= 20) hp = 3720 * WaveNumber;
        else if (WaveNumber >= 19) hp = 2250 * WaveNumber;
        else if(WaveNumber >= 12) hp = 1030 * WaveNumber;
        else if (WaveNumber >= 11) hp = 720 * WaveNumber;
        else if (WaveNumber >= 10) hp = 450 * WaveNumber;
        else if(WaveNumber >= 6) hp = 260 * WaveNumber;
        else if (WaveNumber >= 5) hp = 180 * WaveNumber;
        else if (WaveNumber >= 4) hp = 120 * WaveNumber;
        else hp = 55 * WaveNumber;
        
        mBaseObj.Health.InitHP(hp);
        StartCoroutine(MoveAround());
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
