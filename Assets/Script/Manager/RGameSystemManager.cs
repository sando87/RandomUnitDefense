using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UpgradeType
{
    None, TypeA, TypeB, TypeC, TypeD, TypeE
}

public class RGameSystemManager : RManager
{
    public const float MineralIntervalSec = 1.0f;
    public const float WaveIntervalSec = 10.0f;
    public const int MobCountPerWave = 60;
    public const float LineMobBurstIntervalSec = 1.5f;
    public const int KillPointCost = 5;
    public const int LineMobLimit = 80;
    public const int StartKillPoint = 20;

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
    private List<string> LineMobs = new List<string>();
    private List<string> StartingMembers = new List<string>();
    private Dictionary<UpgradeType, int> UpgradePower = new Dictionary<UpgradeType, int>();

    public override void Init()
    {
        base.Init();
        CleanUpGame();
    }

    public void StartGame(string[] startingMembers)
    {
        CleanUpGame();

        KillPoint = StartKillPoint;
        StageRoot = Instantiate(StagePrefab);
        HUDObject = Instantiate(HUDPrefab, transform);
        WayPoints[0] = StageRoot.transform.Find("WayPoint_LB").position;
        WayPoints[1] = StageRoot.transform.Find("WayPoint_RB").position;
        WayPoints[2] = StageRoot.transform.Find("WayPoint_RT").position;
        WayPoints[3] = StageRoot.transform.Find("WayPoint_LT").position;
        StartingMembers.AddRange(startingMembers);

        UnitMob[] mobs = RGame.Get<RGameObjectManager>().GetPrefabsInfo<UnitMob>();
        foreach(UnitMob mob in mobs)
            LineMobs.Add(mob.name);

        StartCoroutine(UserInputInterpreter());
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

        StartingMembers.Clear();
        LineMobs.Clear();
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
    public UnitBase CreateRandomUnit()
    {
        int randomIndex = UnityEngine.Random.Range(0, StartingMembers.Count);
        string unitName = StartingMembers[randomIndex];
        return CreateUnit(unitName);
    }
    public UnitBase CreateUnit(string unitName)
    {
        Vector3 pos = StageRoot.transform.position;
        pos.x += UnityEngine.Random.Range(-1.0f, 1.0f);
        pos.y += UnityEngine.Random.Range(-1.0f, 1.0f);
        RGame.Get<RGameObjectManager>().AcquireRGameObject(unitName, out RGameObject obj);
        obj.transform.SetParent(StageRoot.transform);
        obj.transform.position = pos;
        return obj.GetComponent<UnitBase>();
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
                RGame.Get<RUIManager>().SwitchToForm<RUIFormLobby>(default);
            });
        }
        else
        {
            RUiMessageBox.PopUp("Game Over..", (isOK) => {
                CleanUpGame();
                RGame.Get<RUIManager>().SwitchToForm<RUIFormLobby>(default);
            });
        }
    }
    public T[] DetectAroundUnit<T>(Vector3 pos, float range) where T : UnitBase
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos, range);
        if (hitColliders == null || hitColliders.Length <= 0)
            return null;

        List<T> list = new List<T>();
        foreach (var hitCollider in hitColliders)
        {
            Vector2 dir = hitCollider.transform.position - pos;
            if (dir.magnitude >= range)
                continue;

            T mob = hitCollider.GetComponent<T>();
            if (mob != null && mob.CurrentState != UnitState.Death)
                list.Add(mob);
        }

        return list.Count > 0 ? list.ToArray() : null;
    }
    public int GetUpgradeCount(UpgradeType type) { return UpgradePower[type]; }

    // Unity System Input을 최초로 받아 게임에서 사용하기 쉬운형태로 변환 후 호출해줌
    // Collider 컴포넌트가 게임오브젝트에 활성화되어 있어야 함
    private IEnumerator UserInputInterpreter()
    {
        yield return null;
        List<IUserInputReciever> DownRecievers = new List<IUserInputReciever>();
        Vector3 DownPosition = Vector3.zero;
        bool IsDragged = false;

        while(true)
        {
            if (UserInputLocked)
            {
                if(DownRecievers.Count > 0)
                {
                    if (IsDragged)
                    {
                        Vector3 worldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        foreach(var recv in DownRecievers)
                            recv?.OnDragAndDrop(worldPt);
                    }
                    else
                    {
                        foreach (var recv in DownRecievers)
                            recv?.OnClick();
                    }
                }

                DownRecievers.Clear();
                DownPosition = Vector3.zero;
                IsDragged = false;
                yield return null;
                continue;
            }

            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                yield return null;
                continue;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(worldPt);
                if (hit != null && hit.gameObject.activeSelf)
                {
                    IUserInputReciever[] recievers = hit.gameObject.GetComponents<IUserInputReciever>();
                    if(recievers != null)
                    {
                        DownRecievers.AddRange(recievers);
                        DownPosition = worldPt;
                        IsDragged = false;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector3 worldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (IsDragged)  // Drag & Drop Event 발생
                {
                    foreach (var recv in DownRecievers)
                        recv?.OnDragAndDrop(worldPt);
                }
                else if(DownRecievers.Count > 0)    // Click Event 발생
                {
                    foreach (var recv in DownRecievers)
                        recv?.OnClick();
                }
                else  //빈곳 클릭시 
                {
                    HUDObject.Hide();
                }

                DownRecievers.Clear();
                DownPosition = Vector3.zero;
                IsDragged = false;
            }
            else if (Input.GetMouseButton(0))
            {
                if (DownRecievers.Count > 0) //현재 Down이 된 상태일때만 진입
                {
                    if (IsDragged) //사용자가 Dragging하는 매프래임마다 진입
                    {
                        Vector3 curWorldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        foreach (var recv in DownRecievers)
                            recv?.OnDragging(curWorldPt);
                    }
                    else //사용자가 처음 Drag하는 순간에만 진입
                    {
                        Vector3 curWorldPt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if ((curWorldPt - DownPosition).magnitude >= RSystemConfig.DragThreshold)
                            IsDragged = true;
                    }
                    
                }
            }

            yield return null;
        }

    }
    private IEnumerator LineMobGenerator()
    {
        RemainSecond = WaveIntervalSec;
        yield return Yielders.GetWaitForSeconds(WaveIntervalSec);
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
                yield return Yielders.GetWaitForSeconds(LineMobBurstIntervalSec);

                if (LineMobCount > LineMobLimit)
                    FinishGame(false);
            }

            //한 웨이브 끝나고 대기시간 후 다음 웨이브 시작
            RemainSecond = WaveIntervalSec;
            yield return Yielders.GetWaitForSeconds(WaveIntervalSec);
            WaveNumber++;

            if (WaveNumber > LineMobs.Count)
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
            yield return Yielders.GetWaitForSeconds(MineralIntervalSec);
            Mineral += MineralStep;
            RemainSecond -= MineralIntervalSec;
            RemainSecond = Mathf.Max(0, RemainSecond);
        }
    }
    private bool CreateLineMob()
    {
        LineMobCount++;
        string name = LineMobs[WaveNumber - 1];
        RGame.Get<RGameObjectManager>().AcquireRGameObject(name, out RGameObject obj);
        obj.transform.SetParent(StageRoot.transform);
        obj.transform.position = WayPoints[3];
        return true;
    }

    public Vector3[] GetWayPoints() { return WayPoints; }

}
