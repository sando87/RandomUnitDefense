﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDShower : MonoBehaviour
{
    void Start()
    {
        UserInput userInput = this.GetBaseObject().CharacterInput as UserInput;
        if(userInput != null)
        {
            userInput.EventSelected += OnSelect;
            userInput.EventDeSelected += OnDeSelect;
        }
    }

    private void OnSelect()
    {
        //유닛 클릭시 Merge 선택창 UI 띄우기
        RGame.Get<RSoundManager>().PlaySFX(AudioClipType.ShowHUD);
        RGame.Get<RGameSystemManager>().HUDObject.Show(this.GetBaseObject());
    }

    private void OnDeSelect()
    {
        RGame.Get<RGameSystemManager>().HUDObject.Hide();
    }

}
