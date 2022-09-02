using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectIndicator : MonoBehaviour
{
    GameObject mSelectCircle = null;

    void Awake()
    {
        mSelectCircle = this.GetBaseObject().Renderer.transform.Find("SelectCircle").gameObject;
        mSelectCircle.gameObject.SetActive(false);
    }

    void Start()
    {
        UserInput ui = this.GetBaseObject().GetComponentInChildren<UserInput>();
        if(ui != null)
        {
            ui.EventSelected += ShowSelectCircle;
            ui.EventDeSelected += HideSelectCircle;
        }
    }

    void ShowSelectCircle()
    {
        mSelectCircle.gameObject.SetActive(true);
    }
    void HideSelectCircle()
    {
        mSelectCircle.gameObject.SetActive(false);
    }
}
