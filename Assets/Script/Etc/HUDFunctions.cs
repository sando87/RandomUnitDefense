using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFunctions : MonoBehaviour
{
    private const int MergeCountForLevelUP = 3;
    private const int MergeCountForChange = 2;
    private const float DetectRange = 3.0f;

    [SerializeField] private GameObject MergeLevelUP_ON = null;
    [SerializeField] private GameObject MergeLevelUP_OFF = null;
    [SerializeField] private GameObject MergeChange_ON = null;
    [SerializeField] private GameObject MergeChange_OFF = null;
    [SerializeField] private GameObject Refund_ON = null;
    [SerializeField] private GameObject Refund_OFF = null;

    RGameSystemManager GameMgr = null;
    public UnitUser TargetUnit { get; private set; } = null;
    List<UnitUser> DetectedUnits = new List<UnitUser>();


    private void Start()
    {
        GameMgr = RGame.Get<RGameSystemManager>();
        MergeLevelUP_ON.GetComponent<HUDFunction>().EventClick = OnClickLevelUp;
        MergeChange_ON.GetComponent<HUDFunction>().EventClick = OnClickChange;
        Refund_ON.GetComponent<HUDFunction>().EventClick = OnClickRefund;
        Refund_ON.SetActive(true);
        Refund_OFF.SetActive(false);

        Hide();
    }

    private void Update()
    {
        DetectSameUnit();

        MergeLevelUP_ON.SetActive(DetectedUnits.Count >= MergeCountForLevelUP);
        MergeLevelUP_OFF.SetActive(DetectedUnits.Count < MergeCountForLevelUP);
        MergeChange_ON.SetActive(DetectedUnits.Count >= MergeCountForChange);
        MergeChange_OFF.SetActive(DetectedUnits.Count < MergeCountForChange);
    }

    public void Show(UnitUser unit)
    {
        TargetUnit = unit;
        Vector3 pos = TargetUnit.Center;
        pos.z = -1;
        transform.position = pos;
        transform.SetParent(TargetUnit.transform);
        gameObject.SetActive(true);
        GetComponent<Animator>().Play("HUDShow", -1, 0);
    }
    public void Hide()
    {
        transform.SetParent(GameMgr.transform);
        TargetUnit = null;
        DetectedUnits.Clear();
        gameObject.SetActive(false);
    }

    private void DetectSameUnit()
    {
        DetectedUnits.Clear();
        UnitUser[] units = GameMgr.DetectAroundUnit<UnitUser>(TargetUnit.transform.position, DetectRange);
        foreach(UnitUser unit in units)
        {
            if (unit.PrefabID == TargetUnit.PrefabID && unit.Property.Level == TargetUnit.Property.Level)
                DetectedUnits.Add(unit);
        }
    }

    private void OnClickLevelUp()
    {
        if (DetectedUnits.Count < MergeCountForLevelUP)
            return;

        DetectedUnits.Sort((unitA, unitB) =>
        {
            float distA = (TargetUnit.transform.position - unitA.transform.position).sqrMagnitude;
            float distB = (TargetUnit.transform.position - unitB.transform.position).sqrMagnitude;
            return distA > distB ? 1 : -1;
        });

        DetectedUnits[0].FSM.ChangeState(UnitState.Disappear);
        DetectedUnits[1].FSM.ChangeState(UnitState.Disappear);
        DetectedUnits[2].FSM.ChangeState(UnitState.Disappear);
        UnitBase unit = GameMgr.CreateUnit(TargetUnit.PrefabID);
        unit.Property.Level = TargetUnit.Property.Level + 1;
        Hide();
    }
    private void OnClickChange()
    {
        if (DetectedUnits.Count < MergeCountForChange)
            return;

        DetectedUnits.Sort((unitA, unitB) =>
        {
            float distA = (TargetUnit.transform.position - unitA.transform.position).sqrMagnitude;
            float distB = (TargetUnit.transform.position - unitB.transform.position).sqrMagnitude;
            return distA > distB ? 1 : -1;
        });

        DetectedUnits[0].FSM.ChangeState(UnitState.Disappear);
        DetectedUnits[1].FSM.ChangeState(UnitState.Disappear);
        UnitBase unit = GameMgr.CreateRandomUnit();
        unit.Property.Level = TargetUnit.Property.Level;
        Hide();
    }
    private void OnClickRefund()
    {
        TargetUnit.FSM.ChangeState(UnitState.Disappear);
        GameMgr.AddMinerals(100 * TargetUnit.Property.Level * TargetUnit.Property.Level);
        Hide();
    }

}
