using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RUIFormLobby : RUiForm
{
    [SerializeField] private AudioClip LobbyBackround = null;

    [SerializeField] private Button UnitCardSelector = null;
    [SerializeField] private Button Shop = null;
    [SerializeField] private Button Setting = null;
    [SerializeField] private Button GameStart = null;
    [SerializeField] private Button GoldUp = null;
    [SerializeField] private Text Gold = null;

    #region override functions
    public override void BindEvent()
    {
        base.BindEvent();

        UnitCardSelector.onClick.AddListener(OnSelector);
        Shop.onClick.AddListener(OnShop);
        Setting.onClick.AddListener(OnSetting);
        GameStart.onClick.AddListener(OnGameStart);
        GoldUp.onClick.AddListener(OnClickGoldUp);
    }
    public override void UpdateForm(UiFormParam param)
    {
        base.UpdateForm(param);

        Gold.text = param.gold.ToString();
    }
    public override void Show()
    {
        base.Show();
        RGame.Get<RSoundManager>().PlayBackgroundMusic(LobbyBackround);
    }
    #endregion

    #region event handler
    private void OnSelector()
    {
        RGame.Get<RUIManager>().SwitchToForm<RUIFormUnitSelector>(default);
    }
    private void OnShop()
    {
        RUiMessageBox.PopUp("Not Ready : OnShop", null);
    }
    private void OnSetting()
    {
        RUiMessageBox.PopUp("Not Ready : OnSetting", null);
    }
    private void OnGameStart()
    {
        List<string> userUnits = new List<string>();
        UnitUser[] units = RGame.Get<RGameObjectManager>().GetPrefabsInfo<UnitUser>();
        foreach (UnitUser unit in units)
            userUnits.Add(unit.name);

        RGame.Get<RGameSystemManager>().StartGame(userUnits.ToArray());
        RGame.Get<RUIManager>().SwitchToForm<RUIFormInGame>(default);
    }
    private void OnClickGoldUp()
    {
        int gold = int.Parse(Gold.text);
        gold++;
        Gold.text = gold.ToString();
    }
    #endregion


}
