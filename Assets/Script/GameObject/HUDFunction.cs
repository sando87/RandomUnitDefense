using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HUDFunction : MonoBehaviour, IUserInputReciever
{
    public Action EventClick;

    public void OnClick()
    {
        EventClick?.Invoke();
    }

    public void OnDragAndDrop(Vector3 dropWorldPos)
    {
    }

    public void OnDragging(Vector3 draggingWorldPos)
    {
    }
}
