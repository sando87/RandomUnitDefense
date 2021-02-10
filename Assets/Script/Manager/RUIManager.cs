using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RUIManager : RManager
{
    private Dictionary<Type, RUiForm> Forms = new Dictionary<Type, RUiForm>();

    public override void Init()
    {
        base.Init();
        InitChildForms();
    }

    private void InitChildForms()
    {
        Transform canvasTR = transform.GetChild(0);
        for (int i = 0; i < canvasTR.childCount; ++i)
        {
            RUiForm form = canvasTR.GetChild(i).GetComponent<RUiForm>();
            form.Init();
            form.BindEvent();
            if(form.gameObject.activeSelf)
                form.UpdateForm(default);
            Forms.Add(form.GetType(), form);
        }
    }

    private void HideAll()
    {
        foreach (var form in Forms)
            form.Value.Hide();
    }

    public T GetForm<T>() where T : RUiForm
    {
        if (!Forms.ContainsKey(typeof(T)))
        {
            Debug.Log("GetForm : No Form Type : " + typeof(T).Name);
            return null;
        }

        RUiForm form = Forms[typeof(T)];
        return form as T;
    }

    public void SwitchToForm<T>(UiFormParam param)
    {
        if (!Forms.ContainsKey(typeof(T)))
        {
            Debug.Log("SwitchToForm : No Form Type : " + typeof(T).Name);
            return;
        }

        HideAll();

        RUiForm form = Forms[typeof(T)];
        form.Show();
        form.UpdateForm(param);
    }

    public Canvas Canvas { get { return GetComponentInChildren<Canvas>(); } }

}
