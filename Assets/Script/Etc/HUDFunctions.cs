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
    public BaseObject TargetUnit { get; private set; } = null;
    List<BaseObject> DetectedUnits = new List<BaseObject>();


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

    public void Show(BaseObject unit)
    {
        TargetUnit = unit;
        Vector3 pos = TargetUnit.Body.Center;
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
        Collider[] cols = InGameUtils.DetectAround(TargetUnit.transform.position, DetectRange, 1 << TargetUnit.gameObject.layer);
        foreach(Collider col in cols)
        {
            BaseObject obj = col.GetBaseObject();
            if (obj.UnitUser.ResourceID == TargetUnit.UnitUser.ResourceID && obj.SpecProp.Level == TargetUnit.SpecProp.Level)
                DetectedUnits.Add(obj);
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

        DetectedUnits[0].MotionManager.SwitchMotion<MotionDisappear>();
        DetectedUnits[1].MotionManager.SwitchMotion<MotionDisappear>();
        DetectedUnits[2].MotionManager.SwitchMotion<MotionDisappear>();
        BaseObject newUnit = GameMgr.CreateUnit(TargetUnit.UnitUser.ResourceID);
        newUnit.SpecProp.Level = TargetUnit.SpecProp.Level + 1;
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

        DetectedUnits[0].MotionManager.SwitchMotion<MotionDisappear>();
        DetectedUnits[1].MotionManager.SwitchMotion<MotionDisappear>();
        BaseObject unit = GameMgr.CreateRandomUnit();
        unit.SpecProp.Level = TargetUnit.SpecProp.Level;
        Hide();
    }
    private void OnClickRefund()
    {
        TargetUnit.MotionManager.SwitchMotion<MotionDisappear>();
        GameMgr.AddMinerals(100 * TargetUnit.SpecProp.Level * TargetUnit.SpecProp.Level);
        Hide();
    }

}
