using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserInputReciever
{
    void OnClick();
    void OnDragAndDrop(Vector3 worldPos);
    void OnDragging(Vector3 worldPos);
}
