﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_RoundMain : RUiForm
{
    [SerializeField] private Button Button_CreateObjTemp = null;

    public override void BindEvent()
    {
        base.BindEvent();
        Button_CreateObjTemp.onClick.AddListener(OnClick_CreateObj);
    }

    private void OnClick_CreateObj()
    {
        //RGame.Get<RGameObjectManager>().AcquireRGameObject(Recycler.RecycleId.SampleSquare, out RGameObject obj);
        if(UnityEngine.Random.Range(0, 2) == 0)
            RGame.Get<RGameObjectManager>().AcquireRGameObject(Recycler.RecycleId.fenrir, out RGameObject obj);
        else
            RGame.Get<RGameObjectManager>().AcquireRGameObject(Recycler.RecycleId.marine, out RGameObject obj);
    }
}
