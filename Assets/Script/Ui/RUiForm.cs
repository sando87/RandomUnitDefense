using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UiFormParam
{
    public string title;
    public string message;
    public int gold;
}

public abstract class RUiForm : RUiObject
{
    public virtual void Init() { }  //Form 로드시 초기 한번만 수행
    public virtual void BindEvent() { } //Init수행 후 초기 한번만 수행(각종 이벤트 등록)
    public virtual void UpdateForm(UiFormParam param) { }   //Form이 보여질때마다 수행(UI정보를 갱신하는 작업)
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
