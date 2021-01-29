using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RUiMessageBox : MonoBehaviour
{
    [SerializeField] private Text Message = null;
    [SerializeField] private Button Ok = null;
    [SerializeField] private Button Cancle = null;

    private Action<bool> EventReturn = null;

    public static RUiMessageBox PopUp(string message, Action<bool> eventReturn)
    {
        RUIManager mgr = RGame.Get<RUIManager>();
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Ui/UIMessageBox", typeof(GameObject));
        GameObject objMenu = GameObject.Instantiate(prefab, mgr.Canvas.transform);
        RUiMessageBox box = objMenu.GetComponent<RUiMessageBox>();
        box.Init(message, eventReturn);
        return box;
    }

    private void Init(string message, Action<bool> eventReturn)
    {
        EventReturn = eventReturn;
        Message.text = message;
        Ok.onClick.AddListener(OnOK);
        Cancle.onClick.AddListener(OnCancle);
    }
    private void OnOK()
    {
        EventReturn?.Invoke(true);
        Destroy(gameObject);
    }
    private void OnCancle()
    {
        EventReturn?.Invoke(false);
        Destroy(gameObject);
    }
}
