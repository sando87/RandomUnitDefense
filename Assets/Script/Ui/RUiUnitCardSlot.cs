using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RUiUnitCardSlot : MonoBehaviour
{
    private RUiUnitCard EquippedCard = null;

    public RUiUnitCard SwitchCard(RUiUnitCard card)
    {
        RUiUnitCard prvCard = null;
        if (EquippedCard != null)
        {
            EquippedCard.StartingMember = false;
            prvCard = EquippedCard;
        }
            
        card.StartingMember = true;
        EquippedCard = card;
        GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;
        return prvCard;
    }

    public void SetSlotActive(bool isActive)
    {
        GetComponent<Button>().enabled = isActive;
        transform.GetChild(0).gameObject.SetActive(isActive);
    }
}
