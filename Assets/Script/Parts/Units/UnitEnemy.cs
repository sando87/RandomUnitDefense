using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEnemy : UnitBase
{
    [SerializeField] Sprite[] HitEffect = null;

    public int WaveIndex { get; set; } = 0;

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

        mBaseObj.Health.InitHP(15 + (WaveIndex * WaveIndex * 30));
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
