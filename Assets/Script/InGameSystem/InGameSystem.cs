using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UpgradeType
{
    None, TypeA, TypeB, TypeC, TypeD, TypeE
}

public class InGameSystem : SingletonMono<InGameSystem>
{
    public const float MineralIntervalSec = 1.0f;
    public const float WaveIntervalSec = 10.0f;
    public const int MobCountPerWave = 5;
    public const float LineMobBurstIntervalSec = 1.5f;
    public const int KillPointCost = 5;
    public const int LineMobLimit = 80;
    public const int StartKillPoint = 200;

    [SerializeField] private GameObject StagePrefab = null;
    [SerializeField] private HUDFunctions HUDPrefab = null;

    public int WaveNumber { get; private set; }
    public int Mineral { get; private set; }
    public int MineralStep { get; private set; }
    public int KillPoint { get; private set; }
    public int LineMobCount { get; private set; }
    public float RemainSecond { get; private set; }
    public bool UserInputLocked { get; set; }
    public HUDFunctions HUDObject { get; set; }

    private GameObject StageRoot = null;
    private Vector3[] WayPoints = new Vector3[4];
    private List<long> LineMobIDs = new List<long>();
    private Dictionary<UpgradeType, int> UpgradePower = new Dictionary<UpgradeType, int>();

    protected override void Awake()
    {
        base.Awake();
        CleanUpGame();
    }

    public void StartGame()
    {
        CleanUpGame();

        KillPoint = StartKillPoint;
        StageRoot = Instantiate(StagePrefab);
        HUDObject = Instantiate(HUDPrefab, transform);
        WayPoints[0] = StageRoot.transform.Find("WayPoint_LB").position;
        WayPoints[1] = StageRoot.transform.Find("WayPoint_RB").position;
        WayPoints[2] = StageRoot.transform.Find("WayPoint_RT").position;
        WayPoints[3] = StageRoot.transform.Find("WayPoint_LT").position;

        foreach(EnemyCharactor mob in EnemyCharactors.Inst.Enums())
            LineMobIDs.Add(mob.ID);

        StartCoroutine(LineMobGenerator());
        StartCoroutine(MineralMining());
    }
    public void CleanUpGame()
    {
        StopAllCoroutines();

        WaveNumber = 1;
        Mineral = 0;
        MineralStep = 1;
        KillPoint = 0;
        LineMobCount = 0;
        RemainSecond = 0;
        UserInputLocked = false;

        UpgradePower.Clear();
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            UpgradePower[type] = 0;

        if(StageRoot != null)
        {
            Destroy(StageRoot);
            StageRoot = null;
        }

        if (HUDObject != null)
        {
            Destroy(HUDObject);
            HUDObject = null;
        }

        LineMobIDs.Clear();
        for (int i = 0; i < WayPoints.Length; ++i)
            WayPoints[i] = Vector3.zero;
    }

    public bool TryCreateRandomUnit()
    {
        if (KillPoint < KillPointCost)
            return false;

        KillPoint -= KillPointCost;
        CreateRandomUnit();
        return true;
    }
    public BaseObject CreateRandomUnit()
    {
        int randomIndex = UnityEngine.Random.Range(0, UserCharactors.Inst.Count);
        UserCharactor data = UserCharactors.Inst.GetDataOfIndex(randomIndex);
        return CreateUnit(data.ID);
    }
    public BaseObject CreateUnit(long unitResourceID)
    {
        Vector3 pos = StageRoot.transform.position;
        pos.x += UnityEngine.Random.Range(-1.0f, 1.0f);
        pos.y += UnityEngine.Random.Range(-1.0f, 1.0f);
        UserCharactor data = UserCharactors.Inst.GetDataOfId(unitResourceID);
        GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity, StageRoot.transform);
        BaseObject baseObj = obj.GetBaseObject();
        baseObj.Unit.ResourceID = unitResourceID;
        return baseObj;
    }
    public bool RaiseMineralStep()
    {
        if (KillPoint < KillPointCost)
            return false;

        KillPoint -= KillPointCost;
        MineralStep++;
        return true;
    }
    public bool UpgradeWeapon(UpgradeType type)
    {
        int cost = UpgradePower[type];
        if (Mineral < cost)
            return false;

        Mineral -= cost;
        UpgradePower[type]++;
        return true;
    }
    public void DeathLineMob()
    {
        KillPoint++;
        LineMobCount--;
    }
    public void AddMinerals(int mineral)
    {
        Mineral += mineral;
    }
    public void FinishGame(bool success)
    {
        if (success)
        {
            RUiMessageBox.PopUp("Success!!", (isOK) => {
                CleanUpGame();
                RUIManager.Instance.SwitchToForm<RUIFormLobby>(default);
            });
        }
        else
        {
            RUiMessageBox.PopUp("Game Over..", (isOK) => {
                CleanUpGame();
                RUIManager.Instance.SwitchToForm<RUIFormLobby>(default);
            });
        }
    }
    public int GetUpgradeCount(UpgradeType type) { return UpgradePower[type]; }

    private IEnumerator LineMobGenerator()
    {
        RemainSecond = WaveIntervalSec;
        yield return newWaitForSeconds.Cache(WaveIntervalSec);
        WaveNumber = 1;
        while(true)
        {
            //웨이브시작 라인몹 출몰하는 루프 진입
            int mobBurstCount = 0;
            RemainSecond = MobCountPerWave * LineMobBurstIntervalSec;
            while (mobBurstCount < MobCountPerWave)
            {
                CreateLineMob();
                mobBurstCount++;
                yield return newWaitForSeconds.Cache(LineMobBurstIntervalSec);

                if (LineMobCount > LineMobLimit)
                    FinishGame(false);
            }

            //한 웨이브 끝나고 대기시간 후 다음 웨이브 시작
            RemainSecond = WaveIntervalSec;
            yield return newWaitForSeconds.Cache(WaveIntervalSec);
            WaveNumber++;

            if (WaveNumber > LineMobIDs.Count)
            {
                FinishGame(true);
                break;
            }
        }
    }
    private IEnumerator MineralMining()
    {
        while(true)
        {
            yield return newWaitForSeconds.Cache(MineralIntervalSec);
            Mineral += MineralStep;
            RemainSecond -= MineralIntervalSec;
            RemainSecond = Mathf.Max(0, RemainSecond);
        }
    }
    private bool CreateLineMob()
    {
        LineMobCount++;
        long id = LineMobIDs[WaveNumber - 1];
        EnemyCharactor mobData = EnemyCharactors.Inst.GetDataOfId(id);
        Instantiate(mobData.prefab, WayPoints[3], Quaternion.identity, StageRoot.transform);
        return true;
    }

    public Vector3[] GetWayPoints() { return WayPoints; }

}
