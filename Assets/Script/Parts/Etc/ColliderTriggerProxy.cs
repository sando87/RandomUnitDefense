using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTriggerProxy : MonoBehaviour
{
    [SerializeField] private LayerMask LayersToTrig;

    public event Action<Collider> EventTriggerEnter;
    public event Action<Collider> EventTriggerStay;
    public event Action<Collider> EventTriggerExit;

    public Collider Other { get; private set; } = null;
    public Vector3 HitPoint { get; private set; } = Vector3.zero;

    public void ClearEvents()
    {
        EventTriggerEnter = null;
        EventTriggerStay = null;
        EventTriggerExit = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!enabled) return;
        if(!MyUtils.IsIncludeLayer(other.gameObject.layer, LayersToTrig)) return;

        Vector3 origin = transform.position - Consts.BlockSize * transform.right;
        if(Physics.Raycast(origin, transform.right, out RaycastHit hit, Consts.BlockSize * 2.0f, LayersToTrig))
        {
            HitPoint = hit.point - (transform.right * 0.05f);
        }
        else
        {
            HitPoint = transform.position;
        }

        Other = other;
        EventTriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enabled) return;
        if (!MyUtils.IsIncludeLayer(other.gameObject.layer, LayersToTrig)) return;

        EventTriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        if (!MyUtils.IsIncludeLayer(other.gameObject.layer, LayersToTrig)) return;

        EventTriggerExit?.Invoke(other);
        if(Other == other)
        {
            Other = null;
        }
    }

    public void AddLayerToTrig(int layer)
    {
        LayersToTrig |= (1 << layer);
    }
    public void AddLayerToTrig(string layerName)
    {
        AddLayerToTrig(LayerMask.NameToLayer(layerName));
    }
    public void SetLayerMask(int layerMask)
    {
        LayersToTrig = layerMask;
    }
}
