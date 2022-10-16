using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    public const float LineMobBurstIntervalSec = 1.7f;
    public const int KillPointForNewUnit = 5;
    public const int KillPointForMineralUp = 5;
    public const int LineMobLimit = 80;
    public const int StartKillPoint = 10;
    public const int MergeCountLevelup = 3;
    public const int KillPointForLevelup = 0;
    public const int MergeCountReunit = 2;

    [SerializeField] private GameObject StagePrefab = null;
    [SerializeField] private GameObject DeathPointPrefab = null;

    public int WaveNumber { get; private set; }
    public int Mineral { get; private set; }
    public int MineralStep { get; private set; }
    public int KillPoint { get; private set; }
    public int LineMobCount { get; private set; }
    public long WaveEndTick { get; private set; }
    public bool UserInputLocked { get; set; }
    public BaseObject SelectedUnit { get { return SelectedUnits.Count > 0 ? SelectedUnits.First().Key : null; } }
    public event System.Action EventSelectUnit = null;

    private GameObject StageRoot = null;
    private Vector3[] WayPoints = new Vector3[4];
    private List<long> LineMobIDs = new List<long>();
    private Dictionary<BaseObject, bool> SelectedUnits = new Dictionary<BaseObject, bool>();
    private Dictionary<UpgradeType, int> UpgradePower = new Dictionary<UpgradeType, int>();
    private List<BaseObject> mUnitsForLevelup = new List<BaseObject>();

    private RUIFormInGame mInGameUI = null;

    protected override void Awake()
    {
        base.Awake();
        CleanUpGame();
    }
    void Start()
    {
        InGameInput.Instance.EventClick += OnClickUnit;
        InGameInput.Instance.EventLongClick += OnLongClickUnit;
        InGameInput.Instance.EventDragStart += OnDragStart;
        InGameInput.Instance.EventDragging += OnDragging;
        InGameInput.Instance.EventDragEnd += OnDragEnd;
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
    public void DeathLineMob(BaseObject deathMob)
    {
        LineMobCount--;

        Vector3 pos = deathMob.transform.position;
        GameObject deathPoint = Instantiate(DeathPointPrefab, pos, Quaternion.identity, transform);
        float ranPosX = UnityEngine.Random.Range(-0.5f, 0.5f);
        deathPoint.transform.DOMoveY(pos.y + 1, 0.15f).SetEase(Ease.OutQuad);
        deathPoint.transform.DOMoveY(pos.y, 0.15f).SetEase(Ease.InQuad).SetDelay(0.15f);
        deathPoint.transform.DOMoveX(pos.x + ranPosX, 0.3f).SetEase(Ease.Linear);

        Transform kps = mInGameUI.KillPointSet;
        deathPoint.transform.DOMove(kps.transform.position, 1).SetEase(Ease.InQuad).SetDelay(1).OnComplete(() =>
        {
            KillPoint++;
            Destroy(deathPoint);
            mInGameUI.KillPointSet.DOKill();
            mInGameUI.KillPointSet.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f).From(1).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
        });
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
        
        mInGameUI = FindObjectOfType<RUIFormInGame>();
        
        while(true)
        {
            yield return newWaitForSeconds.Cache(MineralIntervalSec);
            Mineral += MineralStep;
            mInGameUI.ShowMineralRasingEffect(MineralStep);
        }
    }
    private bool CreateLineMob()
    {
        LineMobCount++;
        long id = LineMobIDs[WaveNumber - 1];
        EnemyCharactor mobData = EnemyCharactors.Inst.GetDataOfId(id);
        GameObject enemy = Instantiate(mobData.prefab, WayPoints[3], Quaternion.identity, StageRoot.transform);
        enemy.GetComponentInChildren<UnitEnemy>().ResourceID = id;
        enemy.GetComponentInChildren<UnitEnemy>().WaveIndex = WaveNumber - 1;
        return true;
    }

    public Vector3[] GetWayPoints() 
    {
        return WayPoints;
    }

    public void OnMergeForLevelup()
    {
        SortUnitsForLevelUp();
        if(IsMergeableForLevelUp())
            MergeForLevelup(mUnitsForLevelup[0], mUnitsForLevelup[1], mUnitsForLevelup[2]);

        DeSelectAll();
    }
    private void MergeForLevelup(BaseObject unitA, BaseObject unitB, BaseObject unitC)
    {
        unitA.MotionManager.SwitchMotion<MotionDisappear>();
        unitB.MotionManager.SwitchMotion<MotionDisappear>();
        unitC.MotionManager.SwitchMotion<MotionDisappear>();
        BaseObject newUnit = CreateUnit(unitA.Unit.ResourceID);
        newUnit.SpecProp.Level = SelectedUnit.SpecProp.Level + 1;
        newUnit.SynSpec.MergeSynergySpecs(unitA.SynSpec, unitB.SynSpec, unitC.SynSpec);
    }
    public void OnMergeForReunit()
    {
        if(IsMergeableForReUnit())
        {
            BaseObject[] units = SelectedUnits.Keys.ToArray();
            MergeForReunit(units[0], units[1]);
        }

        DeSelectAll();
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
        foreach(var item in SelectedUnits)
        {
            BaseObject unit = item.Key;
            unit.MotionManager.SwitchMotion<MotionDisappear>();
            AddMinerals(100 * unit.SpecProp.Level * unit.SpecProp.Level);
        }
        DeSelectAll();
    }





    private void OnClickUnit(BaseObject unit)
    {
        if(unit == null)
        {
            DeSelectAll();
        }
        else if(unit.gameObject.layer == LayerID.Enemies)
        {
            ShowEnemyHealthBar(unit);
        }
        else if (unit.gameObject.layer == LayerID.Player)
        {
            if(SelectedUnits.ContainsKey(unit))
            {
                DeSelectUnit(unit);
            }
            else
            {
                SelectUnit(unit);
                EventSelectUnit?.Invoke();
            }
        }
    }
    private void OnLongClickUnit(Vector3 worldPos)
    {
        DeSelectAll();
    }
    private void OnDragStart(Vector3 worldPos)
    {

    }
    private void OnDragging(Vector3 worldPos)
    {
        if(InGameInput.Instance.DownObject != null)
        {
            BaseObject target = InGameInput.Instance.DownObject;
            UserInput ui = target.GetComponentInChildren<UserInput>();
            if (ui != null)
                ui.OnDrawMoveIndicator(worldPos);
        }
        else
        {
            Rect worldArea = ToRect(InGameInput.Instance.DownWorldPos, worldPos);
            mInGameUI.ShowSelectArea(worldArea);
        }
    }
    private void OnDragEnd(Vector3 worldEndPos)
    {
        if (InGameInput.Instance.DownObject != null)
        {
            BaseObject target = InGameInput.Instance.DownObject;
            UserInput ui = target.GetComponentInChildren<UserInput>();
            if (ui != null)
                ui.OnMove(worldEndPos);
        }
        else
        {
            mInGameUI.HideSelectArea();
            Rect worldArea = ToRect(InGameInput.Instance.DownWorldPos, worldEndPos);
            SelectUnitsInArea(worldArea);
            EventSelectUnit?.Invoke();
        }
    }

    private Rect ToRect(Vector3 worldStartPos, Vector3 worldEndPos)
    {
        Vector2 center = (worldEndPos + worldStartPos) * 0.5f;
        Vector2 size = worldEndPos - worldStartPos;
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        Rect worldArea = new Rect();
        worldArea.size = size;
        worldArea.center = center;
        return worldArea;
    }





    private void SelectUnit(BaseObject unit)
    {
        SelectedUnits[unit] = true;
        UserInput ui = unit.GetComponentInChildren<UserInput>();
        if (ui != null)
            ui.OnSelect();
    }
    private void SelectUnitsInArea(Rect area)
    {
        Collider[] cols = Physics.OverlapBox(area.center, area.size * 0.5f, Quaternion.identity, 1 << LayerID.Player);
        if(cols.Length > 0)
        {
            DeSelectAll();
            foreach (Collider col in cols)
            {
                SelectUnit(col.GetBaseObject());
            }
            return;
        }

        cols = Physics.OverlapBox(area.center, area.size * 0.5f, Quaternion.identity, 1 << LayerID.Enemies);
        foreach (Collider col in cols)
        {
            ShowEnemyHealthBar(col.GetBaseObject());
        }
    }
    private void DeSelectUnit(BaseObject unit)
    {
        SelectedUnits.Remove(unit);
        UserInput ui = unit.GetComponentInChildren<UserInput>();
        if (ui != null)
            ui.OnDeSelect();
    }
    private void DeSelectAll()
    {
        BaseObject[] units = SelectedUnits.Keys.ToArray();
        foreach (BaseObject unit in units)
        {
            DeSelectUnit(unit);
        }
    }
    private void ShowEnemyHealthBar(BaseObject enemy)
    {
        if (enemy.Health != null)
        {
            enemy.Health.ShowHealthBar();
        }
    }


    // 같은 종류의 같은 레벨끼리 먼저 나오도록 정렬
    public void SortUnitsForLevelUp()
    {
        mUnitsForLevelup.Clear();
        if(SelectedUnits.Count != 3)
            return;

        BaseObject[] objs = SelectedUnits.Keys.ToArray();
        if(objs[0].IsMergable(objs[1]))
        {
            mUnitsForLevelup.Add(objs[0]);
            mUnitsForLevelup.Add(objs[1]);
            mUnitsForLevelup.Add(objs[2]);
        }
        else if (objs[0].IsMergable(objs[2]))
        {
            mUnitsForLevelup.Add(objs[0]);
            mUnitsForLevelup.Add(objs[2]);
            mUnitsForLevelup.Add(objs[1]);
        }
        else if (objs[1].IsMergable(objs[2]))
        {
            mUnitsForLevelup.Add(objs[1]);
            mUnitsForLevelup.Add(objs[2]);
            mUnitsForLevelup.Add(objs[0]);
        }
    }
    public bool IsMergeableForLevelUp()
    {
        SortUnitsForLevelUp();
        return mUnitsForLevelup.Count == 3;
    }
    public bool IsMergeableForReUnit()
    {
        if (SelectedUnits.Count == 2)
        {
            BaseObject[] units = SelectedUnits.Keys.ToArray();
            return units[0].IsMergable(units[1]);
        }
        return false;
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
