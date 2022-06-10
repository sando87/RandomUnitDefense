using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] GameObject DroppableGround = null;

    private BaseObject mBaseObject = null;

    void Start()
    {
        mBaseObject = this.GetBaseObject();

        if(IsTopLadder())
            DroppableGround.SetActive(true);
        else
            DroppableGround.SetActive(false);
    }

    private bool IsTopLadder()
    {
        RaycastHit[] hits = Physics.RaycastAll(mBaseObject.Body.Center, Vector3.up, mBaseObject.Body.Size.y, 1 << LayerID.Props);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponentInBaseObject<Ladder>() != null)
            {
                return false;
            }
        }
        return true;
    }
}
