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

    InGameSystem GameMgr = null;
    public BaseObject TargetUnit { get; private set; } = null;
    List<BaseObject> DetectedUnits = new List<BaseObject>();

    private void Start()
    {
        GameMgr = InGameSystem.Instance;

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

        transform.rotation = Quaternion.identity;
    }

    public void Show(BaseObject unit)
    {
        TargetUnit = unit;
        Vector3 pos = TargetUnit.Body.Center;
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
            if (obj.Unit.ResourceID == TargetUnit.Unit.ResourceID && obj.SpecProp.Level == TargetUnit.SpecProp.Level)
                DetectedUnits.Add(obj);
        }
    }

    public void OnClickLevelUp()
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
        BaseObject newUnit = GameMgr.CreateUnit(TargetUnit.Unit.ResourceID);
        newUnit.SpecProp.Level = TargetUnit.SpecProp.Level + 1;
        Hide();
    }
    public void OnClickChange()
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
    public void OnClickRefund()
    {
        TargetUnit.MotionManager.SwitchMotion<MotionDisappear>();
        GameMgr.AddMinerals(100 * TargetUnit.SpecProp.Level * TargetUnit.SpecProp.Level);
        Hide();
    }

}
