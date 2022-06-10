using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Televator : MonoBehaviour, IMapEditorObject
{
    [SerializeField] GameObject TopGroup = null;
    [SerializeField] GameObject MidGroup = null;
    [SerializeField] GameObject BottomGroup = null;
    [SerializeField] GameObject DroppableGround = null;
    [SerializeField] GameObject VFX = null;
    [SerializeField] float _MoveUpSpeed = 15;

    public float MoveUpSpeed { get { return _MoveUpSpeed; } }

    void Start()
    {
        UpdateTelevatorState();
    }

    private void UpdateTelevatorState()
    {
        TopGroup.SetActive(false);
        MidGroup.SetActive(true);
        BottomGroup.SetActive(false);
        DroppableGround.SetActive(false);
        VFX.SetActive(false);

        if (FindTopTelevator() == null)
        {
            MidGroup.SetActive(false);
            TopGroup.SetActive(true);
            DroppableGround.SetActive(true);
        }

        if (FindBottomTelevator() == null)
        {
            MidGroup.SetActive(false);
            BottomGroup.SetActive(true);
            VFX.SetActive(true);
            int groundCount = CalcGroundCount(this);
            Vector3 scale = VFX.transform.localScale;
            scale.y = 0.05f * groundCount;
            VFX.transform.localScale = scale;
        }
    }

    private Televator FindTopTelevator()
    {
        RaycastHit[] hits = Physics.RaycastAll(this.GetBaseObject().Body.Center, Vector3.up, this.GetBaseObject().Body.Size.y, 1 << LayerID.Props);
        foreach(RaycastHit hit in hits)
        {
            Televator tele = hit.collider.GetComponentInBaseObject<Televator>();
            if (tele != null)
                return tele;
        }
        return null;
    }
    private Televator FindBottomTelevator()
    {
        RaycastHit[] hits = Physics.RaycastAll(this.GetBaseObject().Body.Center, -Vector3.up, this.GetBaseObject().Body.Size.y, 1 << LayerID.Props);
        foreach (RaycastHit hit in hits)
        {
            Televator tele = hit.collider.GetComponentInBaseObject<Televator>();
            if (tele != null)
                return tele;
        }
        return null;
    }

    private int CalcGroundCount(Televator bottomTelevator)
    {
        int count = 1;
        Televator tele = bottomTelevator;
        while(true)
        {
            tele = tele.FindTopTelevator();
            if(tele == null)
                break;
            else
                count++;
        }
        return count;
    }

    public void OnInitMapEditor()
    {
        enabled = false;
    }
}
