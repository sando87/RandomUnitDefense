﻿using System;
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
    public const float KillPointIntervalSec = 1.5f;
    public const float WaveIntervalSec = 5.0f;
    public const int MobCountPerWave = 60;
    public const float LineMobBurstIntervalSec = 1.0f;
    public const int KillPointForNewUnit = 10;
    public const int KillPointForMineralUp = 30;
    public const int LineMobLimit = 150;
    public const int StartKillPoint = 20;
    public const int MergeCountLevelup = 3;
    public const int KillPointForLevelup = 0;
    public const int MergeCountReunit = 2;

    [SerializeField] private GameObject StagePrefab = null;
    [SerializeField] private GameObject DeathPointPrefab = null;

    public int WaveNumber { get; private set; }
    public int NextWaveNumberForTest { get; set; }
    public int Mineral { get; set; }
    public int MineralStep { get; private set; }
    public int KillPoint { get; set; }
    public int LineMobCount { get; private set; }
    public long WaveEndTick { get; private set; }
    public bool UserInputLocked { get; set; }
    public BaseObject SelectedUnit { get { return SelectedUnits.Count > 0 ? SelectedUnits.First().Key : null; } }
    public event System.Action EventSelectUnit = null;
    public event System.Action EventDeSelectUnit = null;

    private GameObject StageRoot = null;
    private Vector3[] WayPoints = new Vector3[4];
    private List<long> LineMobIDs = new List<long>();
    private Dictionary<BaseObject, bool> SelectedUnits = new Dictionary<BaseObject, bool>();
    private Dictionary<UpgradeType, int> UpgradePower = new Dictionary<UpgradeType, int>();
    private List<BaseObject> mUnitsForLevelup = new List<BaseObject>();
    private Vector3 mFirstCreateDir = Vector3.right;
    private RUIFormInGame mInGameUI = null;

    protected override void Awake()
    {
        base.Awake();
        CleanUpGame();
    }
    void Start()
    {
        InGameInput.Instance.EventClick += OnClickUnit;
        InGameInput.Instance.EventDoubleClick += OnDoubleClickUnit;
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
        StartCoroutine(KillPointMining());

        // 1초마다 한번씩 확인해서 현재 Mob개수가 최대 개수를 초과하면 게임 실패...
        this.ExRepeatCoroutine(1, () =>
        {
            if(LineMobCount > LineMobLimit)
                FinishGame(false);
        });
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

    public int GetMineralForRarePercentUpgrade()
    {
        int needMineral = 
        (mCurrentRarePercentIndex == 0 ? 200 : 
        (mCurrentRarePercentIndex == 1 ? 600 : 
        (mCurrentRarePercentIndex == 2 ? 2000 : 
        (mCurrentRarePercentIndex == 3 ? 6000 : 
        (mCurrentRarePercentIndex == 4 ? 20000 : 60000)))));
        return needMineral;
    }
    public float[] GetCurrnetRarePercentTable()
    {
        return mRarePercentTable[mCurrentRarePercentIndex];
    }
    public float[] GetNextRarePercentTable()
    {
        if(mCurrentRarePercentIndex + 1 < mRarePercentTable.Count)
            return mRarePercentTable[mCurrentRarePercentIndex + 1];
        return null;
    }

    public bool TryRaiseRarePercent()
    {
        int needMineral = GetMineralForRarePercentUpgrade();
        if (Mineral < needMineral)
            return false;
        else
            Mineral -= needMineral;

        mCurrentRarePercentIndex++;
        mCurrentRarePercentIndex = Mathf.Min(mCurrentRarePercentIndex, mRarePercentTable.Count - 1);
        return true;
    }
    
    int mCurrentRarePercentIndex = 0;
    List<float[]> mRarePercentTable = new List<float[]>()
    {
        new float[] {100.0f, 0.5f, 0, 0, 0},
        new float[] {100.0f, 5.0f, 0.5f, 0, 0},
        new float[] {100.0f, 20.0f, 2.5f, 0.5f, 0},
        new float[] {100.0f, 40.0f, 10.0f, 2.0f, 0.5f},
        new float[] {100.0f, 60.0f, 20.0f, 5.0f, 1.0f},
        new float[] {100.0f, 80.0f, 40.0f, 10.0f, 2.0f},
    };
    public int GetRareRandomLevel()
    {
        float[] curRareTable = mRarePercentTable[mCurrentRarePercentIndex];
        int rem = (UnityEngine.Random.Range(0, 100000) % 10000);
        for(int lv = 5; lv > 1; lv--)
        {
            int hitPercent = (int)(curRareTable[lv - 1] * 100.0f);
            if(rem < hitPercent)
                return lv;
        }
        return 1;
    }
    public bool TryCreateRandomUnit()
    {
        if (KillPoint < KillPointForNewUnit)
            return false;

        KillPoint -= KillPointForNewUnit;
        BaseObject unit = CreateRandomUnit();
        
        int level = GetRareRandomLevel();
        unit.SpecProp.Level = level;
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
        Vector3 dest = pos + (mFirstCreateDir * 0.5f);
        mFirstCreateDir = MyUtils.RotateVector(mFirstCreateDir, Vector3.forward, 45);
        UserCharactor data = UserCharactors.Inst.GetDataOfId(unitResourceID);
        GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity, StageRoot.transform);
        BaseObject baseObj = obj.GetBaseObject();
        baseObj.Unit.ResourceID = unitResourceID;
        baseObj.MotionManager.FindMotion<MotionMove>().Destination = dest;
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

        // 강화된 적 유닛이 죽었을 경우 해당 위치에 아군 유닛이 보상으로 지급된다.
        if(deathMob.GetComponentInChildren<UnitEnemy>().IsEnforced)
            CreateRandomUnitReward(pos, deathMob.GetComponentInChildren<UnitEnemy>().WaveNumber);

        GameObject deathPoint = Instantiate(DeathPointPrefab, pos, Quaternion.identity, transform);
        float ranPosX = UnityEngine.Random.Range(-0.5f, 0.5f);
        deathPoint.transform.DOMoveY(pos.y + 1, 0.15f).SetEase(Ease.OutQuad);
        deathPoint.transform.DOMoveY(pos.y, 0.15f).SetEase(Ease.InQuad).SetDelay(0.15f);
        deathPoint.transform.DOMoveX(pos.x + ranPosX, 0.3f).SetEase(Ease.Linear);
        int minStep = MineralStep;

        Transform kps = mInGameUI.MineralSet;
        Vector3 diff = (deathPoint.transform.position - kps.transform.position);
        diff.z = 0;
        deathPoint.transform.DOMove(kps.transform.position, diff.magnitude * 0.1f).SetEase(Ease.InQuad).SetDelay(0.8f).OnComplete(() =>
        {
            Mineral += minStep;
            Destroy(deathPoint);
            mInGameUI.MineralSet.DOKill();
            mInGameUI.MineralSet.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f).From(1).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            mInGameUI.ShowMineralRasingEffect(minStep);
        });
    }
    public void CreateRandomUnitReward(Vector3 pos, int mobWaveNum)
    {
        int level = mobWaveNum < 15 ? 2 : (mobWaveNum < 25 ? 3 : 4);
        BaseObject unit = CreateRandomUnit();
        unit.transform.position = pos;
        unit.SpecProp.Level = level;
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
        bool IsEnforcedEnemy = false;
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
                UnitEnemy enemy = CreateLineMob(IsEnforcedEnemy);
                if(enemy.SubMobPrefab != null)
                {
                    // Boss같은 유닛일 경우 Boss위치에서 하위몹이 지속 생성되도록 수행시키고 바로 다음 웨이브로 넘어간다
                    StartSubMobGenerator(enemy);
                    break;
                }
                else
                {
                    IsEnforcedEnemy = false;
                    mobBurstCount++;
                    yield return newWaitForSeconds.Cache(LineMobBurstIntervalSec);
                }
            }

            //한 웨이브 끝나고 대기시간 후 다음 웨이브 시작
            WaveEndTick = 0;
            yield return newWaitForSeconds.Cache(WaveIntervalSec);
            WaveNumber++;

            // Wave가 10이상이면 두개Wave당 한번씩 강화 유닛을 출몰 시킨다(강화유닛 제거시 보상유닛 지급됨...)
            // if(WaveNumber > 10 & WaveNumber % 2 == 0)
            //     IsEnforcedEnemy = true;

            if(NextWaveNumberForTest > 0)
            {
                WaveNumber = NextWaveNumberForTest;
                NextWaveNumberForTest = 0;
            }

            if (WaveNumber > LineMobIDs.Count)
            {
                yield return new WaitUntil(() => LineMobCount <= 0);
                FinishGame(true);
                break;
            }
        }
    }
    private IEnumerator KillPointMining()
    {
        yield return null;
        
        mInGameUI = FindObjectOfType<RUIFormInGame>();
        
        while(true)
        {
            yield return newWaitForSeconds.Cache(KillPointIntervalSec);
            KillPoint++;
        }
    }
    private UnitEnemy CreateLineMob(bool IsEnforced)
    {
        LineMobCount++;
        long id = LineMobIDs[WaveNumber - 1];
        EnemyCharactor mobData = EnemyCharactors.Inst.GetDataOfId(id);
        GameObject enemyObj = Instantiate(mobData.prefab, WayPoints[3], Quaternion.identity, StageRoot.transform);
        UnitEnemy enemy = enemyObj.GetComponentInChildren<UnitEnemy>();
        enemy.ResourceID = id;
        enemy.WaveNumber = WaveNumber;
        enemy.IsEnforced = IsEnforced;
        return enemy;
    }
    private void StartSubMobGenerator(UnitEnemy bossMob)
    {
        int subEnemyWaveNumber = WaveNumber;
        this.ExRepeatCoroutine(LineMobBurstIntervalSec, () =>
        {
            if(bossMob != null && !bossMob.GetBaseObject().Health.IsDead)
            {
                UnitEnemy subEnemy = Instantiate(bossMob.SubMobPrefab, bossMob.transform.position, Quaternion.identity, StageRoot.transform);
                subEnemy.ResourceID = bossMob.SubMobPrefab.ResourceID;
                subEnemy.WaveNumber = subEnemyWaveNumber;

                LineMobCount++;
            }
        });
    }

    public Vector3[] GetWayPoints() 
    {
        return WayPoints;
    }

    public void OnMergeForLevelup()
    {
        if(IsMergeableForLevelUp())
        {
            List<BaseObject[]> groups = GroupBySameUnits();
            foreach(BaseObject[] sameUnits in groups)
            {
                int loopCnt = sameUnits.Length / 3;
                for(int i = 0; i < loopCnt; ++i)
                {
                    MergeForLevelup(sameUnits[(i*3) + 0], sameUnits[(i*3) + 1], sameUnits[(i*3) + 2]);
                }
            }
        }

        DeSelectAll();
    }
    private void MergeForLevelup(BaseObject unitA, BaseObject unitB, BaseObject unitC)
    {
        int nextLevel = unitA.SpecProp.Level + 1;

        unitA.MotionManager.SwitchMotion<MotionDisappear>();
        unitB.MotionManager.SwitchMotion<MotionDisappear>();
        unitC.MotionManager.SwitchMotion<MotionDisappear>();
        
        List<UserCharactor> list = new List<UserCharactor>();
        foreach(UserCharactor charData in UserCharactors.Inst.Enums())
        {
            if(charData.prefab.GetComponent<BaseObject>().SpecProp.DamageType != unitA.SpecProp.DamageType)
                continue;

            list.Add(charData);
        }
        
        int randomIndex = UnityEngine.Random.Range(0, list.Count);

        //BaseObject newUnit = CreateUnit(list[randomIndex].ID);
        BaseObject newUnit = CreateUnit(unitA.Unit.ResourceID);
        newUnit.SpecProp.Level = nextLevel;
        // newUnit.SynSpec.MergeSynergySpecs(unitA.SynSpec, unitB.SynSpec, unitC.SynSpec);
    }
    public void OnMergeForReunit()
    {
        if(IsMergeableForReUnit())
        {
            List<BaseObject[]> groups = GroupBySameUnits();
            foreach(BaseObject[] sameUnits in groups)
            {
                int loopCnt = sameUnits.Length / 2;
                for(int i = 0; i < loopCnt; ++i)
                {
                    MergeForReunit(sameUnits[(i*2) + 0], sameUnits[(i*2) + 1]);
                }
            }
        }

        DeSelectAll();
    }
    private void MergeForReunit(BaseObject unitA, BaseObject unitB)
    {
        unitA.MotionManager.SwitchMotion<MotionDisappear>();
        unitB.MotionManager.SwitchMotion<MotionDisappear>();
        
        List<UserCharactor> list = new List<UserCharactor>();
        foreach(UserCharactor charData in UserCharactors.Inst.Enums())
        {
            if(charData.ID == unitA.Unit.ResourceID)
                continue;
                
            // if(unitA.SpecProp.Level > 1 && charData.prefab.GetComponent<BaseObject>().SpecProp.DamageType != unitA.SpecProp.DamageType)
            //     continue;

            list.Add(charData);
        }
        
        BaseObject newUnit = CreateUnit(list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)].ID : unitA.Unit.ResourceID);
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
            DeSelectAll();
            SelectUnit(unit);
            EventSelectUnit?.Invoke();
        }
    }
    private void OnDoubleClickUnit(BaseObject target)
    {
        if(InGameInput.Instance.DownObject != null && InGameInput.Instance.DownObject.gameObject.layer == LayerID.Player)
        {
            DeSelectAll();
            
            long targetUnitID = InGameInput.Instance.DownObject.Unit.ResourceID;
            int targetUnitLevel = InGameInput.Instance.DownObject.SpecProp.Level;
            UnitPlayer[] allPlayerUnits = StageRoot.transform.GetComponentsInChildren<UnitPlayer>();
            foreach(UnitPlayer playerUnit in allPlayerUnits)
            {
                if(playerUnit.ResourceID == targetUnitID && playerUnit.GetBaseObject().SpecProp.Level == targetUnitLevel)
                {
                    SelectUnit(playerUnit.GetBaseObject());
                }
            }
        }
        
    }
    private void OnDragStart(Vector3 worldPos)
    {

    }
    private void OnDragging(Vector3 worldPos)
    {
        if(InGameInput.Instance.DownObject != null && InGameInput.Instance.DownObject.gameObject.layer == LayerID.Player)
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
        if (InGameInput.Instance.DownObject != null && InGameInput.Instance.DownObject.gameObject.layer == LayerID.Player)
        {
            BaseObject target = InGameInput.Instance.DownObject;
            UserInput ui = target.GetComponentInChildren<UserInput>();
            if (ui != null)
            {
                // 이동시키는 유닛이 선택된 상태이면 같이 선택된 모든 유닛 이동
                if(ui.IsSelected && SelectedUnits.Count > 1)
                {
                    Vector3 cenPos = Vector3.zero;
                    foreach(var item in SelectedUnits)
                    {
                        BaseObject unit = item.Key;
                        cenPos += unit.transform.position;
                    }
                    cenPos /= SelectedUnits.Count;

                    foreach(var item in SelectedUnits)
                    {
                        BaseObject unit = item.Key;
                        Vector3 dir = unit.transform.position - cenPos;
                        float dist = dir.magnitude / 3.0f;
                        Vector3 dest = worldEndPos + (dir.normalized * dist);
                        unit.GetComponentInChildren<UserInput>().OnMove(dest);
                    }
                        
                }
                else // 아니면 드래깅했던 유닛 하나만 이동
                {
                    ui.OnMove(worldEndPos);
                }
            }
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
        EventDeSelectUnit?.Invoke();
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
        List<BaseObject[]> groups = GroupBySameUnits();
        foreach(BaseObject[] group in groups)
        {
            if(group.Length >= 3)
                return true;
        }
        return false;
    }
    public bool IsMergeableForReUnit()
    {
        List<BaseObject[]> groups = GroupBySameUnits();
        foreach(BaseObject[] group in groups)
        {
            if(group.Length >= 2)
                return true;
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
    


    // 현재 선택된 유닛기준으로 Merge가능한 유닛별로 리스팅해서 반환(같은유닛, 같은레벨이여야 합성 가능)
    public List<BaseObject[]> GroupBySameUnits()
    {
        Dictionary<long, List<BaseObject>> container = new Dictionary<long, List<BaseObject>>();

        BaseObject[] units = SelectedUnits.Keys.ToArray();
        foreach(var item in SelectedUnits)
        {
            BaseObject unit = item.Key;
            long key = unit.Unit.ResourceID + unit.SpecProp.Level;
            if(!container.ContainsKey(key))
                container[key] = new List<BaseObject>();

            container[key].Add(unit);
        }

        List<BaseObject[]> ret = new List<BaseObject[]>();
        foreach(var item in container)
            ret.Add(item.Value.ToArray());

        return ret;
    }
}
