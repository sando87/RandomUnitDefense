using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEnemy : UnitBase
{
    public UnitEnemy SubMobPrefab = null;

    public int WaveNumber { get; set; } = 0;
    public bool IsEnforced { get; set; } = false;
    public bool IsBoss { get { return SubMobPrefab != null; } }
    public int WayPointIndex { get; set; } = 0;

    void Start()
    {
        mBaseObj.Health.EventDamaged += OnDamaged;

        float hp = GetBalanceTable();

        if(IsEnforced)
        {
            hp *= 4; //체력 증가
            mBaseObj.BuffProp.MoveSpeed = 200; // (이속버프+% : 이속 증가)
        }
        
        if(IsBoss)
            hp *= 300;
            
        mBaseObj.Health.InitHP(hp);

        StartCoroutine(MoveAround());
    }

    float GetBalanceTable()
    {
        float hp = 0;
        if(WaveNumber >= 50) hp = 100000 * WaveNumber;
        else if (WaveNumber >= 40) hp = 70030 * WaveNumber;
        else if (WaveNumber >= 39) hp = 62020 * WaveNumber;
        else if (WaveNumber >= 38) hp = 54020 * WaveNumber;
        else if (WaveNumber >= 37) hp = 46150 * WaveNumber;
        else if (WaveNumber >= 36) hp = 33030 * WaveNumber;
        else if (WaveNumber >= 35) hp = 25720 * WaveNumber;
        
        else if (WaveNumber >= 27) hp = 21500 * WaveNumber;
        else if (WaveNumber >= 26) hp = 18000 * WaveNumber;
        else if (WaveNumber >= 25) hp = 15500 * WaveNumber;
        else if (WaveNumber >= 24) hp = 12550 * WaveNumber;
        else if (WaveNumber >= 23) hp = 8500 * WaveNumber;

        else if (WaveNumber >= 18) hp = 6050 * WaveNumber;
        else if (WaveNumber >= 17) hp = 5000 * WaveNumber;
        else if (WaveNumber >= 16) hp = 3950 * WaveNumber;
        else if (WaveNumber >= 15) hp = 2900 * WaveNumber;
        else if (WaveNumber >= 14) hp = 1900 * WaveNumber;

        else if (WaveNumber >= 11) hp = 1650 * WaveNumber;
        else if (WaveNumber >= 10) hp = 1300 * WaveNumber;
        else if (WaveNumber >= 9) hp = 1050 * WaveNumber;
        else if (WaveNumber >= 8) hp = 700 * WaveNumber;
        else if (WaveNumber >= 7) hp = 350 * WaveNumber;

        else if (WaveNumber >= 5) hp = 285 * WaveNumber;
        else if (WaveNumber >= 4) hp = 180 * WaveNumber;
        else if (WaveNumber >= 3) hp = 105 * WaveNumber;
        else if (WaveNumber >= 2) hp = 45 * WaveNumber;
        else hp = 25 * WaveNumber;

        // 전반적으로 좀 잘 나온편...
        // 4w +4 20up
        // 11w +7 75up
        // 17w +12 110up
        // 26w +12 160up 100마리 2/2/2
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
        if(!mBaseObj.Health.IsDead && valideDamage > 0)
        {
            mBaseObj.Renderer.StartTwinkle();
        }
    }

    IEnumerator MoveAround()
    {
        yield return null;

        Vector3[] wayPoints = InGameSystem.Instance.GetWayPoints();
        MotionMove motionMove = mBaseObj.MotionManager.FindMotion<MotionMove>();

        while (true)
        {
            Vector3 dest = wayPoints[WayPointIndex];
            motionMove.Destination = dest;
            motionMove.EventArrived = () => 
            {
                WayPointIndex = (WayPointIndex + 1) % wayPoints.Length;
            };
            mBaseObj.MotionManager.SwitchMotion(motionMove);

            yield return new WaitUntil(() => mBaseObj.MotionManager.IsCurrentMotion<MotionIdle>());
        }
    }


}
