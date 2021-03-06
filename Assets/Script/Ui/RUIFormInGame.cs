﻿using System;
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

    private RGameSystemManager GameMgr = null;

    public override void Init()
    {
        base.Init();
        GameMgr = RGame.Get<RGameSystemManager>();
    }

    public override void BindEvent()
    {
        base.BindEvent();
        CreateUnit.onClick.AddListener(OnClickCreateUnit);
        RaiseMineral.onClick.AddListener(OnClickRaiseMineral);
        OpenUpgradePanel.onClick.AddListener(OnClickOpenUpgradePanel);
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
        RGame.Get<RSoundManager>().PlayBackgroundMusic(InGameBackround);
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
        LineMobCount.text = GameMgr.LineMobCount.ToString() + "/" + RGameSystemManager.LineMobLimit;
        RemainGauge.fillAmount = (float)GameMgr.LineMobCount / RGameSystemManager.LineMobLimit;

        WeaponA.text = GameMgr.GetPower(UpgradeType.TypeA).ToString();
        WeaponB.text = GameMgr.GetPower(UpgradeType.TypeB).ToString();
        WeaponC.text = GameMgr.GetPower(UpgradeType.TypeC).ToString();
        WeaponD.text = GameMgr.GetPower(UpgradeType.TypeD).ToString();
        WeaponE.text = GameMgr.GetPower(UpgradeType.TypeE).ToString();

        UpdateUnitDetail();
    }

    private void UpdateUnitDetail()
    {
        UnitUser selectedUnit = GameMgr.HUDObject.TargetUnit;
        if (selectedUnit == null || UpgradePanel.gameObject.activeSelf)
        {
            UnitDetailPanel.gameObject.SetActive(false);
            return;
        }

        UnitDetailPanel.gameObject.SetActive(true);
        string damage = selectedUnit.BasicSpec.attackDamageBasic + " + " + (selectedUnit.BasicSpec.damagePerUpgrade * selectedUnit.Level);
        DamageText.text = damage;
        DescriptionText.text = selectedUnit.SkillDescription;
        UnitPhoto.sprite = selectedUnit.BasicSpec.unitPhoto;
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
    private void OnClickOpenUpgradePanel()
    {
        UpgradePanel.gameObject.SetActive(true);
    }
    private void OnClickCloseUpgradePanel()
    {
        UpgradePanel.gameObject.SetActive(false);
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

}
