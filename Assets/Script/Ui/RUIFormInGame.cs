using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private InGameSystem GameMgr = null;

    public override void Init()
    {
        base.Init();
        GameMgr = InGameSystem.Instance;
        KillPointCostForNewUnit.text = InGameSystem.KillPointForNewUnit.ToString();
        KillPointCostForMineralUp.text = InGameSystem.KillPointForMineralUp.ToString();
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

#if UNITY_EDITOR
        if (InputWrapper.Instance.IsKeyDownTrigger_F1() && Application.isPlaying)
        {
            UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
        }
#endif

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
        if(selectedUnit.gameObject.layer == LayerID.Player)
        {
            string damage = selectedUnit.SpecProp.Damage.ToString();
            DamageText.text = damage + " (" + selectedUnit.SpecProp.DamageInfo + ")";
            UpgradeText.text = selectedUnit.SpecProp.DamageType.ToString();
            DescriptionText.text = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).skillDescription;
            DescriptionText.text += "\n";
            DescriptionText.text += selectedUnit.BuffProp.ToPropInfo();
            UnitPhoto.sprite = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).image;
        }
        else if(selectedUnit.gameObject.layer == LayerID.Enemies)
        {
            float curHP = selectedUnit.Health.CurrentHealth;
            float maxHP = selectedUnit.Health.MaxHP;
            DamageText.text = curHP + " / " + maxHP;
            UpgradeText.text = selectedUnit.SpecProp.Armor.ToString();
            // DescriptionText.text = UserCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).skillDescription;
            // DescriptionText.text += "\n";
            // DescriptionText.text += selectedUnit.BuffProp.ToPropInfo();
            UnitPhoto.sprite = EnemyCharactors.Inst.GetDataOfId(selectedUnit.Unit.ResourceID).image;
        }
    }
    private void UpdateMergeButtonState()
    {
        bool mergableForLevelup = GameMgr.DetectMergeableUnits(InGameSystem.MergeCountLevelup) != null;
        BtnLevelUp.gameObject.SetActive(mergableForLevelup && GameMgr.KillPoint >= InGameSystem.KillPointForLevelup);
        bool mergableForReunit = GameMgr.DetectMergeableUnits(InGameSystem.MergeCountReunit) != null;
        BtnReUnit.gameObject.SetActive(mergableForReunit);
        BtnRefund.gameObject.SetActive(true);

    }
    private void OnSelectUnits(BaseObject[] units)
    {
        UpgradePanel.gameObject.SetActive(false);
        UnitDetailPanel.gameObject.SetActive(true);
        SelectArea.gameObject.SetActive(false);
        UpdateMergeButtonState();
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
            RUiMessageBox.PopUp("Need " + InGameSystem.KillPointForNewUnit + " kill point.", null);
    }
    private void OnClickRaiseMineral()
    {
        if (!GameMgr.RaiseMineralStep())
            RUiMessageBox.PopUp("Need " + InGameSystem.KillPointForMineralUp + " kill point.", null);
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
            ret = GameMgr.UpgradeWeapon(UpgradeType.Melee);
        else if (selBtn == UpgradeWeaponB)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Gun);
        else if (selBtn == UpgradeWeaponC)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Magic);
        else if (selBtn == UpgradeWeaponD)
            ret = GameMgr.UpgradeWeapon(UpgradeType.TypeD);
        else if (selBtn == UpgradeWeaponE)
            ret = GameMgr.UpgradeWeapon(UpgradeType.Missile);

        if (!ret)
            RUiMessageBox.PopUp("Not enough minerals.", null);
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
