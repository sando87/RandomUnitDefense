using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RUIFormInGame : RUiForm
{
    [SerializeField] private AudioClip InGameBackround = null;

    [SerializeField] private Text KillPoint = null;
    [SerializeField] private Text Mineral = null;
    [SerializeField] private Text MineralPerSec = null;
    [SerializeField] private Text WaveNumber = null;
    [SerializeField] private Text RemainTimer = null;
    [SerializeField] private Text LineMobCount = null;
    [SerializeField] private RectTransform RemainGauge = null;
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
    [SerializeField] private Text UpgradeText = null;
    [SerializeField] private Text DescriptionText = null;

    [SerializeField] private Button BtnLevelUp = null;
    [SerializeField] private Button BtnReUnit = null;
    [SerializeField] private Button BtnRefund = null;

    [SerializeField] private RectTransform SelectArea = null;

    [SerializeField] private Text KillPointCostForNewUnit = null;
    [SerializeField] private Text KillPointCostForMineralUp = null;
    [SerializeField] private Text KillPointCostForPercentUp = null;

    [SerializeField] private GameObject RarePercentPanel = null;
    [SerializeField] private Button UpgradePercentBtn = null;
    [SerializeField] private Text RarePercentDisplay = null;
    
    private InGameSystem GameMgr = null;
    public Transform MineralSet { get { return Mineral.transform.parent; } }

    public override void Init()
    {
        base.Init();
        GameMgr = InGameSystem.Instance;
        KillPointCostForNewUnit.text = InGameSystem.KillPointForNewUnit.ToString();
        KillPointCostForMineralUp.text = InGameSystem.KillPointForMineralUp.ToString();
    }

    void Start()
    {
        GameMgr.EventSelectUnit += OnSelectUnit;
        GameMgr.EventDeSelectUnit += OnDeSelectUnit;
    }

    public override void BindEvent()
    {
        base.BindEvent();
        CreateUnit.onClick.AddListener(OnClickCreateUnit);
        RaiseMineral.onClick.AddListener(OnClickRaiseMineral);
        OpenUpgradePanel.onClick.AddListener(OnClickTogglePercentPanel);
        CloseUpgradePanel.onClick.AddListener(OnClickHidePercentPanel);
        UpgradePercentBtn.onClick.AddListener(OnClickRaiseRarePercent);

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
        RemainTimer.text = (GameMgr.WaveEndTick < DateTime.Now.Ticks) ? "" : TimeSpan.FromTicks(DateTime.Now.Ticks - GameMgr.WaveEndTick).ToString(@"mm\:ss");
        LineMobCount.text = GameMgr.LineMobCount.ToString() + "/" + InGameSystem.LineMobLimit;
        RemainGauge.sizeDelta = new Vector2(350.0f * (float)GameMgr.LineMobCount / InGameSystem.LineMobLimit, 50);

        WeaponA.text = GameMgr.GetUpgradeCount(UpgradeType.Melee).ToString();
        WeaponB.text = GameMgr.GetUpgradeCount(UpgradeType.Gun).ToString();
        WeaponC.text = GameMgr.GetUpgradeCount(UpgradeType.Magic).ToString();
        WeaponD.text = GameMgr.GetUpgradeCount(UpgradeType.TypeD).ToString();
        WeaponE.text = GameMgr.GetUpgradeCount(UpgradeType.Missile).ToString();

        UpdateUnitDetail();
    }

    public void ShowMineralRasingEffect(int mineral)
    {
        MineralPerSec.transform.DOKill();
        MineralPerSec.text = "+" + mineral;
        MineralPerSec.transform.DOLocalMoveY(-50, 1).From(-20);
        MineralPerSec.DOFade(0, 1).From(1).SetEase(Ease.InQuad);
    }

    private void UpdateUnitDetail()
    {
        if (!UnitDetailPanel.gameObject.activeSelf)
            return;

        BaseObject selectedUnit = GameMgr.SelectedUnit;
        if(selectedUnit == null)
        {
            UnitDetailPanel.gameObject.SetActive(false);
            return;
        }

        if(selectedUnit.gameObject.layer == LayerID.Player)
        {
            string damage = selectedUnit.SpecProp.Damage.ToString();
            DamageText.text = damage + " (" + selectedUnit.SpecProp.DamageInfo + ")";
            UpgradeText.text = selectedUnit.SpecProp.DamageType.ToString();
            DescriptionText.text = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).skillDescription;
            DescriptionText.text += "\n";
            DescriptionText.text += selectedUnit.BuffProp.ToPropInfo();
            UnitPhoto.sprite = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).image;

            UpdateMergeButtonState();
        }
    }
    private void UpdateMergeButtonState()
    {
        bool mergableForLevelup = GameMgr.IsMergeableForLevelUp();
        BtnLevelUp.gameObject.SetActive(mergableForLevelup && GameMgr.KillPoint >= InGameSystem.KillPointForLevelup);
        bool mergableForReunit = GameMgr.IsMergeableForReUnit();
        BtnReUnit.gameObject.SetActive(mergableForReunit);
        BtnRefund.gameObject.SetActive(true);

    }

    
    private void OnSelectUnit()
    {
        UpgradePanel.gameObject.SetActive(false);
        UnitDetailPanel.gameObject.SetActive(true);
    }
    private void OnDeSelectUnit()
    {
        UpgradePanel.gameObject.SetActive(true);
        UnitDetailPanel.gameObject.SetActive(false);
    }

    public void ShowSelectArea(Rect worldArea)
    {
        SelectArea.gameObject.SetActive(true);
        SelectArea.sizeDelta = worldArea.size * 100;
        SelectArea.transform.position = worldArea.center;
    }
    public void HideSelectArea()
    {
        SelectArea.gameObject.SetActive(false);
    }


    private void OnClickRaiseRarePercent()
    {
        GameMgr.TryRaiseRarePercent();
        KillPointCostForPercentUp.text = GameMgr.GetMineralForRarePercentUpgrade().ToString();
        UpdateRarePercentDisplayInfo();
    }
    private void UpdateRarePercentDisplayInfo()
    {
        float[] curPercentTable = GameMgr.GetCurrnetRarePercentTable();
        float[] nextPercentTable = GameMgr.GetCurrnetRarePercentTable();
        if(nextPercentTable != null)
        {
            string displayInfo = "레벨별 소환 확률\n(현재 -> 다음)";
            for(int i = 0; i < curPercentTable.Length; ++i)
            {
                int level = i + 2;
                displayInfo += ("\n" + level + "Lv : " + curPercentTable[i] + "% -> " + nextPercentTable[i] + "%");

            }
            RarePercentDisplay.text = displayInfo;
        }
        else
        {
            string displayInfo = "레벨별 소환 확률\n(현재 -> Max)";
            for(int i = 0; i < curPercentTable.Length; ++i)
            {
                int level = i + 2;
                displayInfo += ("\n" + level + "Lv : " + curPercentTable[i] + "%");

            }
            RarePercentDisplay.text = displayInfo;
        }
    }
    private void OnClickCreateUnit()
    {
        GameMgr.TryCreateRandomUnit();
        // if (!GameMgr.TryCreateRandomUnit())
        //     RUiMessageBox.PopUp("Need " + InGameSystem.KillPointForNewUnit + " kill point.", null);
    }
    private void OnClickRaiseMineral()
    {
        GameMgr.RaiseMineralStep();
        // if (!GameMgr.RaiseMineralStep())
        //     RUiMessageBox.PopUp("Need " + InGameSystem.KillPointForMineralUp + " kill point.", null);
    }
    private void OnClickTogglePercentPanel()
    {
        bool toState = !RarePercentPanel.gameObject.activeSelf;
        RarePercentPanel.gameObject.SetActive(toState);
    }
    private void OnClickHidePercentPanel()
    {
        RarePercentPanel.gameObject.SetActive(false);
    }
    private void OnClickUpgradeWeapon()
    {
        bool ret = true;
        Button selBtn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (selBtn == UpgradeWeaponA)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Melee);
        else if (selBtn == UpgradeWeaponB)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Gun);
        else if (selBtn == UpgradeWeaponC)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Magic);
        else if (selBtn == UpgradeWeaponD)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeD);
        else if (selBtn == UpgradeWeaponE)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Missile);

        // if (!ret)
        //     RUiMessageBox.PopUp("Not enough minerals.", null);
    }


    public void OnClickLevelUp()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        GameMgr.OnMergeForLevelup();
    }
    public void OnClickChange()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        GameMgr.OnMergeForReunit();
    }
    public void OnClickRefund()
    {
        UnitDetailPanel.gameObject.SetActive(false);
        GameMgr.RefundUnit();
    }

}
