using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct UiFormParam
{

}


public abstract class RUiForm : RUiObject
{
    public virtual void Init(UiFormParam param) { }
    public virtual void BindEvent() { }
    public void Open()
    {

    }
    public void Close()
    {

    }
}
