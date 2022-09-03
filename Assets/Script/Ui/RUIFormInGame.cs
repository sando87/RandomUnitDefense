using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RUIFormInGame : RUiForm
{
    [SerializeField] private AudioClip InGameBackround = null;

    [SerializeField] private Text KillPoint = null;
    [SerializeField] private Text Mineral = null;
    [SerializeField] private Text WaveNumber = null;
    [SerializeField] private Text RemainTimer = null;
    [SerializeField] private Text LineMobCount = null;
    [SerializeField] private Image RemainGauge = null;
    [SerializeField] private Image UpgradePanel = null;
    [SerializeField] private Text WeaponA = null;
    [SerializeField] private Text WeaponB = null;
    [SerializeField] private Text WeaponC = null;
    [SerializeField] private Text WeaponD = null;
    [SerializeField] private Text WeaponE = null;

    [SerializeField] private Button CreateUnit = null;
    [SerializeField] private Button RaiseMineral = null;
    [SerializeField] private Button OpenUpgradePanel = null;
    [SerializeField] private Button CloseUpgradePanel = null;

    [SerializeField] private Button UpgradeWeaponA = null;
    [SerializeField] private Button UpgradeWeaponB = null;
    [SerializeField] private Button UpgradeWeaponC = null;
    [SerializeField] private Button UpgradeWeaponD = null;
    [SerializeField] private Button UpgradeWeaponE = null;

    [SerializeField] private Image UnitDetailPanel = null;
    [SerializeField] private Image UnitPhoto = null;
    [SerializeField] private Text DamageText = null;
    [SerializeField] private Text DescriptionText = null;

    [SerializeField] private Button BtnLevelUp = null;
    [SerializeField] private Button BtnReUnit = null;
    [SerializeField] private Button BtnRefund = null;

    [SerializeField] private RectTransform SelectArea = null;

    private InGameSystem GameMgr = null;
    private List<BaseObject> mSameUnits = new List<BaseObject>();

    public override void Init()
    {
        base.Init();
        GameMgr = InGameSystem.Instance;
    }

    void Start()
    {
        InGameInput.Instance.EventSelectUnits += OnSelectUnits;
        InGameInput.Instance.EventDeSelectUnits += OnDeselectUnits;
        InGameInput.Instance.EventDrawSelectArea += OnDrawSelectArea;
    }

    public override void BindEvent()
    {
        base.BindEvent();
        CreateUnit.onClick.AddListener(OnClickCreateUnit);
        RaiseMineral.onClick.AddListener(OnClickRaiseMineral);
        OpenUpgradePanel.onClick.AddListener(OnClickToggleUpgradePanel);
        CloseUpgradePanel.onClick.AddListener(OnClickCloseUpgradePanel);

        UpgradeWeaponA.onClick.AddListener(OnClickUpgradeWeapon);
        UpgradeWeaponB.onClick.AddListener(OnClickUpgradeWeapon);
        UpgradeWeaponC.onClick.AddListener(OnClickUpgradeWeapon);
        UpgradeWeaponD.onClick.AddListener(OnClickUpgradeWeapon);
        UpgradeWeaponE.onClick.AddListener(OnClickUpgradeWeapon);
    }
    public override void Show()
    {
        base.Show();
    }


    private void Update()
    {
        UpdateForm(default);
    }

    public override void UpdateForm(UiFormParam param)
    {
        base.UpdateForm(param);

        KillPoint.text = GameMgr.KillPoint.ToString();
        Mineral.text = GameMgr.Mineral.ToString();
        WaveNumber.text = "Wave " + GameMgr.WaveNumber.ToString();
        RemainTimer.text = TimeSpan.FromSeconds(GameMgr.RemainSecond).ToString(@"mm\:ss");
        LineMobCount.text = GameMgr.LineMobCount.ToString() + "/" + InGameSystem.LineMobLimit;
        RemainGauge.fillAmount = (float)GameMgr.LineMobCount / InGameSystem.LineMobLimit;

        WeaponA.text = GameMgr.GetUpgradeCount(UpgradeType.TypeA).ToString();
        WeaponB.text = GameMgr.GetUpgradeCount(UpgradeType.TypeB).ToString();
        WeaponC.text = GameMgr.GetUpgradeCount(UpgradeType.TypeC).ToString();
        WeaponD.text = GameMgr.GetUpgradeCount(UpgradeType.TypeD).ToString();
        WeaponE.text = GameMgr.GetUpgradeCount(UpgradeType.TypeE).ToString();

        UpdateUnitDetail();
    }

    private void UpdateUnitDetail()
    {
        if (!UnitDetailPanel.gameObject.activeSelf)
            return;

        BaseObject selectedUnit = GameMgr.SelectedUnit;
        string damage = selectedUnit.SpecProp.Damage.ToString();
        DamageText.text = damage;
        DescriptionText.text = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).skillDescription;
        UnitPhoto.sprite = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).image;

        mSameUnits.Clear();
        GameMgr.DetectSameUnit(selectedUnit, mSameUnits);
        if (mSameUnits.Count > 0)
        {
            BtnLevelUp.gameObject.SetActive(mSameUnits.Count >= 3);
            BtnReUnit.gameObject.SetActive(mSameUnits.Count >= 2);
            BtnRefund.gameObject.SetActive(true);
        }
    }
    private void OnSelectUnits(UserInput[] units)
    {
        UpgradePanel.gameObject.SetActive(false);
        UnitDetailPanel.gameObject.SetActive(true);
        SelectArea.gameObject.SetActive(false);
    }
    private void OnDeselectUnits()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        SelectArea.gameObject.SetActive(false);
    }
    private void OnDrawSelectArea(Rect worldArea)
    {
        SelectArea.gameObject.SetActive(true);
        SelectArea.sizeDelta = worldArea.size * 100;
        SelectArea.transform.position = worldArea.center;
    }


    private void OnClickCreateUnit()
    {
        if (!GameMgr.TryCreateRandomUnit())
            RUiMessageBox.PopUp("Need 5 kill point.", null);
    }
    private void OnClickRaiseMineral()
    {
        if (!GameMgr.RaiseMineralStep())
            RUiMessageBox.PopUp("Need 5 kill point.", null);
    }
    private void OnClickToggleUpgradePanel()
    {
        bool toState = !UpgradePanel.gameObject.activeSelf;
        UpgradePanel.gameObject.SetActive(toState);
        UnitDetailPanel.gameObject.SetActive(!toState && GameMgr.SelectedUnit != null);
    }
    private void OnClickCloseUpgradePanel()
    {
        UpgradePanel.gameObject.SetActive(false);
        UnitDetailPanel.gameObject.SetActive(GameMgr.SelectedUnit != null);
    }
    private void OnClickUpgradeWeapon()
    {
        bool ret = true;
        Button selBtn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (selBtn == UpgradeWeaponA)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeA);
        else if (selBtn == UpgradeWeaponB)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeB);
        else if (selBtn == UpgradeWeaponC)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeC);
        else if (selBtn == UpgradeWeaponD)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeD);
        else if (selBtn == UpgradeWeaponE)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeE);

        if (!ret)
            RUiMessageBox.PopUp("Not enough minerals.", null);
    }


    public void OnClickLevelUp()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        GameMgr.MergeForLevelup();
    }
    public void OnClickChange()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        GameMgr.MergeForReunit();
    }
    public void OnClickRefund()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        GameMgr.MergeForRefund();
    }

}
