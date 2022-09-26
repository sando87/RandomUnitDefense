using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UpgradeType
{
    None, Melee, Gun, Magic, TypeD, Missile
}

public class InGameSystem : SingletonMono<InGameSystem>
{
    public const float MineralIntervalSec = 3.0f;
    public const float WaveIntervalSec = 3.0f;
    public const int MobCountPerWave = 30;
    public const float LineMobBurstIntervalSec = 1.5f;
    public const int KillPointForNewUnit = 5;
    public const int KillPointForMineralUp = 5;
    public const int LineMobLimit = 80;
    public const int StartKillPoint = 20;
    public const int MergeCountLevelup = 3;
    public const int KillPointForLevelup = 0;
    public const int MergeCountReunit = 2;

    [SerializeField] private GameObject StagePrefab = null;

    public int WaveNumber { get; private set; }
    public int Mineral { get; private set; }
    public int MineralStep { get; private set; }
    public int KillPoint { get; private set; }
    public int LineMobCount { get; private set; }
    public long WaveEndTick { get; private set; }
    public bool UserInputLocked { get; set; }
    public BaseObject SelectedUnit { get { return SelectedUnits.Count > 0 ? SelectedUnits[0] : null; } }

    private GameObject StageRoot = null;
    private Vector3[] WayPoints = new Vector3[4];
    private List<long> LineMobIDs = new List<long>();
    private List<BaseObject> SelectedUnits = new List<BaseObject>();
    private Dictionary<UpgradeType, int> UpgradePower = new Dictionary<UpgradeType, int>();

    protected override void Awake()
    {
        base.Awake();
        CleanUpGame();
    }
    void Start()
    {
        InGameInput.Instance.EventSelectUnits += OnSelectUnits;
        InGameInput.Instance.EventDeSelectUnits += OnDeselectUnits;
        InGameInput.Instance.EventMove += OnMoveUnits;
    }

    public void StartGame()
    {
        CleanUpGame();

        KillPoint = StartKillPoint;
        StageRoot = Instantiate(StagePrefab);
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
        WaveEndTick = 0;
        UserInputLocked = false;

        UpgradePower.Clear();
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            UpgradePower[type] = 0;

        if(StageRoot != null)
        {
            Destroy(StageRoot);
            StageRoot = null;
        }

        LineMobIDs.Clear();
        for (int i = 0; i < WayPoints.Length; ++i)
            WayPoints[i] = Vector3.zero;
    }

    public bool TryCreateRandomUnit()
    {
        if (KillPoint < KillPointForNewUnit)
            return false;

        KillPoint -= KillPointForNewUnit;
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
        if (KillPoint < KillPointForMineralUp)
            return false;

        KillPoint -= KillPointForMineralUp;
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
        WaveEndTick = 0;
        yield return newWaitForSeconds.Cache(WaveIntervalSec);
        WaveNumber = 1;
        while(true)
        {
            //웨이브시작 라인몹 출몰하는 루프 진입
            int mobBurstCount = 0;
            WaveEndTick = System.DateTime.Now.Ticks + System.TimeSpan.FromSeconds(MobCountPerWave * LineMobBurstIntervalSec).Ticks;
            while (mobBurstCount < MobCountPerWave)
            {
                CreateLineMob();
                mobBurstCount++;
                yield return newWaitForSeconds.Cache(LineMobBurstIntervalSec);

                if (LineMobCount >= LineMobLimit)
                    FinishGame(false);
            }

            //한 웨이브 끝나고 대기시간 후 다음 웨이브 시작
            WaveEndTick = 0;
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
        yield return null;
        RUIFormInGame inGameUI = FindObjectOfType<RUIFormInGame>();
        while(true)
        {
            yield return newWaitForSeconds.Cache(MineralIntervalSec);
            Mineral += MineralStep;
            inGameUI.ShowMineralRasingEffect(MineralStep);
        }
    }
    private bool CreateLineMob()
    {
        LineMobCount++;
        long id = LineMobIDs[WaveNumber - 1];
        EnemyCharactor mobData = EnemyCharactors.Inst.GetDataOfId(id);
        GameObject enemy = Instantiate(mobData.prefab, WayPoints[3], Quaternion.identity, StageRoot.transform);
        enemy.GetComponentInChildren<UnitEnemy>().WaveIndex = WaveNumber - 1;
        return true;
    }

    public Vector3[] GetWayPoints() 
    {
        return WayPoints;
    }

    public void OnMergeForLevelup()
    {
        List<BaseObject[]> mergeUnitSet = DetectMergeableUnits(MergeCountLevelup);
        if (mergeUnitSet != null && mergeUnitSet.Count > 0)
        {
            foreach (BaseObject[] objs in mergeUnitSet)
            {
                if(KillPoint >= KillPointForLevelup)
                {
                    KillPoint -= KillPointForLevelup;
                    LOG.warn(objs.Length != MergeCountLevelup);
                    MergeForLevelup(objs[0], objs[1], objs[2]);
                }
            }
        }

        DeselectAll();
    }
    private void MergeForLevelup(BaseObject unitA, BaseObject unitB, BaseObject unitC)
    {
        unitA.MotionManager.SwitchMotion<MotionDisappear>();
        unitB.MotionManager.SwitchMotion<MotionDisappear>();
        unitC.MotionManager.SwitchMotion<MotionDisappear>();
        BaseObject newUnit = CreateUnit(unitA.Unit.ResourceID);
        newUnit.SpecProp.Level = SelectedUnit.SpecProp.Level + 1;
    }
    public void OnMergeForReunit()
    {
        List<BaseObject[]> mergeUnitSet = DetectMergeableUnits(MergeCountReunit);
        if (mergeUnitSet != null && mergeUnitSet.Count > 0)
        {
            foreach (BaseObject[] objs in mergeUnitSet)
            {
                LOG.warn(objs.Length != MergeCountReunit);
                MergeForReunit(objs[0], objs[1]);
            }
        }

        DeselectAll();
    }
    private void MergeForReunit(BaseObject unitA, BaseObject unitB)
    {
        unitA.MotionManager.SwitchMotion<MotionDisappear>();
        unitB.MotionManager.SwitchMotion<MotionDisappear>();
        BaseObject newUnit = CreateRandomUnit();
        newUnit.SpecProp.Level = SelectedUnit.SpecProp.Level;
    }
    public void RefundUnit()
    {
        foreach(BaseObject unit in SelectedUnits)
        {
            unit.MotionManager.SwitchMotion<MotionDisappear>();
            AddMinerals(100 * unit.SpecProp.Level * unit.SpecProp.Level);
        }
        DeselectAll();
    }
    private void OnSelectUnits(BaseObject[] units)
    {
        SelectedUnits.Clear();
        SelectedUnits.AddRange(units);
        foreach(BaseObject unit in units)
        {
            unit.GetComponentInChildren<UserInput>().OnSelect();
        }
    }
    private void OnDeselectUnits()
    {
        DeselectAll();
    }
    private void DeselectAll()
    {
        foreach (BaseObject unit in SelectedUnits)
        {
            unit.GetComponentInChildren<UserInput>().OnDeSelect();
        }
        SelectedUnits.Clear();
    }
    private void OnMoveUnits(BaseObject obj, Vector3 dest)
    {
        bool isSelectedUnit = false;
        foreach(BaseObject unit in SelectedUnits)
        {
            if(obj == unit)
            {
                isSelectedUnit = true;
                break;
            }
        }

        if(isSelectedUnit)
        {
            foreach (BaseObject unit in SelectedUnits)
            {
                Vector3 destination = MyUtils.Random(dest, 0.5f);
                unit.GetComponentInChildren<UserInput>().OnMove(destination);
            }
        }
        else
        {
            obj.GetComponentInChildren<UserInput>().OnMove(dest);
        }
    }

    // 같은 종류의 같은 레벨의 맵의 모든 유닛
    public List<BaseObject> DetectSameUnit(BaseObject targetUnit)
    {
        List<BaseObject> rets = new List<BaseObject>();
        foreach (Transform child in StageRoot.transform)
        {
            if(child.gameObject.layer == targetUnit.gameObject.layer)
            {
                BaseObject obj = child.gameObject.GetBaseObject();
                if (obj.IsMergable(targetUnit))
                    rets.Add(obj);
            }
        }
        return rets;
    }
    // 현재 선택된 유닛기준으로 Merge가능한 유닛별로 리스팅해서 반환
    public List<BaseObject[]> DetectMergeableUnits(int unitMergeCount)
    {
        // 선택된 유닛이 한마리일 경우 전체 유닛을 모두 대상
        if(SelectedUnits.Count == 1)
        {
            List<BaseObject> sameAllUnits = DetectSameUnit(SelectedUnits[0]);
            if (sameAllUnits.Count < unitMergeCount)
                return null;

            SortByDistance(SelectedUnits[0].Body.Center, sameAllUnits);
            List<BaseObject> mergeSet = sameAllUnits.GetRange(0, unitMergeCount);
            return new List<BaseObject[]>() { mergeSet.ToArray() };
        }
        // 선택된 유닛이 다수일 경우 선택된 유닛들만 대상
        else if(SelectedUnits.Count > 1)
        {
            List<BaseObject[]> rets = new List<BaseObject[]>();

            BaseObject[] units = SelectedUnits.ToArray();
            for (int i = 0; i < units.Length; ++i)
            {
                if (units[i] == null) continue;

                List<int> sameUnitIndex = new List<int>();
                sameUnitIndex.Add(i);
                int j = i + 1;
                for (; j < units.Length; ++j)
                {
                    if (units[j] == null) continue;

                    BaseObject bo = units[j];
                    if (bo.IsMergable(units[i]))
                    {
                        sameUnitIndex.Add(j);
                        if(sameUnitIndex.Count >= unitMergeCount)
                        {
                            List<BaseObject> subset = new List<BaseObject>();
                            foreach (int subIdx in sameUnitIndex)
                            {
                                subset.Add(units[subIdx]);
                                units[subIdx] = null;
                            }
                            rets.Add(subset.ToArray());
                            break;
                        }
                    }
                }
            }

            if(rets.Count > 0)
                return rets;
        }
        return null;
    }

    private void SortByDistance(Vector3 center, List<BaseObject> rets)
    {
        if(rets.Count <= 0) return;

        rets.Sort((unitA, unitB) =>
        {
            float distA = (center - unitA.transform.position).sqrMagnitude;
            float distB = (center - unitB.transform.position).sqrMagnitude;
            return distA > distB ? 1 : -1;
        });
    }
}
