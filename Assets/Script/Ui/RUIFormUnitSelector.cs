using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RUIFormUnitSelector : RUiForm
{
    [SerializeField] private RUiUnitCard[] BenchMembers = null;
    [SerializeField] private RUiUnitCardSlot[] StartingMemberSlots = null;
    [SerializeField] private Image UnitPhoto = null;
    [SerializeField] private Text UnitDetail = null;
    [SerializeField] private Button Apply = null;
    [SerializeField] private Button Cancle = null;
    [SerializeField] private Button ReinForce = null;
    [SerializeField] private GameObject SoltsLockCover = null;
    [SerializeField] private GameObject YellowSelector = null;

    #region override functions
    public override void Init()
    {
        base.Init();
        
        //프리팹으로부터 유닛 정보 읽어와서 동적으로 초기 카드 화면 구성 필요(나중작업)
        //우선은 대기열에 있는 카드를 스타팅 슬롯에 초기 한번 세팅한다.
        int idx = 0;
        foreach (RUiUnitCardSlot slot in StartingMemberSlots)
        {
            for(; idx < BenchMembers.Length; ++idx)
            {
                if(BenchMembers[idx].StartingMember)
                {
                    slot.SwitchCard(BenchMembers[idx]);
                    ++idx;
                    break;
                }
            }
        }
    }
    public override void BindEvent()
    {
        base.BindEvent();

        foreach (RUiUnitCard card in BenchMembers)
            card.GetComponent<Button>().onClick.AddListener(OnClickBenchMember);

        foreach (RUiUnitCardSlot slot in StartingMemberSlots)
            slot.GetComponent<Button>().onClick.AddListener(OnClickStartingMember);

        Apply.onClick.AddListener(OnApply);
        Cancle.onClick.AddListener(OnCancle);
        ReinForce.onClick.AddListener(OnClickReinForceUnit);
    }
    public override void UpdateForm(UiFormParam param)
    {
        base.UpdateForm(param);

        UpdateStartingSlotState();
        UpdateUnitDetailPanel();
    }
    #endregion


    #region event handler
    private void OnClickBenchMember()
    {
        RUiUnitCard selCard = EventSystem.current.currentSelectedGameObject.GetComponent<RUiUnitCard>();
        if (!selCard.Owned)
            return;

        YellowSelector.transform.SetParent(selCard.transform, false);
        UpdateForm(default);
    }
    private void OnClickStartingMember()
    {
        RUiUnitCardSlot slot = EventSystem.current.currentSelectedGameObject.GetComponent<RUiUnitCardSlot>();
        slot.SwitchCard(YellowSelector.GetComponentInParent<RUiUnitCard>());
        UpdateForm(default);
    }
    private void OnClickReinForceUnit()
    {
        YellowSelector.GetComponentInParent<RUiUnitCard>().Damage += 1;
        UpdateForm(default);
    }
    private void OnApply()
    {
        //현재 수정된 정보를 file로 저장
        //lobby쪽 선택된 영웅 정보에 set

        RUIManager.Instance.SwitchToForm<RUIFormLobby>(default);
    }
    private void OnCancle()
    {
        RUIManager.Instance.SwitchToForm<RUIFormLobby>(default);
    }
    #endregion


    private void UpdateStartingSlotState()
    {
        RUiUnitCard curSelCard = YellowSelector.GetComponentInParent<RUiUnitCard>();
        SoltsLockCover.SetActive(curSelCard.StartingMember);
        foreach (RUiUnitCardSlot slot in StartingMemberSlots)
            slot.SetSlotActive(!curSelCard.StartingMember);

    }
    private void UpdateUnitDetailPanel()
    {
        RUiUnitCard curSelCard = YellowSelector.GetComponentInParent<RUiUnitCard>();
        UnitPhoto.sprite = curSelCard.GetComponent<Image>().sprite;
        UnitDetail.text = curSelCard.GetDetail();
    }
}
