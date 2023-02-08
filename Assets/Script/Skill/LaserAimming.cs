using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAimming : MonoBehaviour
{
    [SerializeField] GameObject Laser = null;
    [SerializeField] GameObject Outro = null;

    public GameObject Target { get; set; } = null;
    public Vector3 Destination { get; set; } = Vector3.zero;
    
    Vector3 CurrentDestPoint { get { return (Target != null) ? Target.transform.position : Destination; } }

    public static LaserAimming Play(Vector3 startPos, GameObject target, string vfxName)
    {
        LaserAimming prefab = ResourcesCache.Load<LaserAimming>("Prefabs/Effects/" + vfxName);
        LaserAimming obj = Instantiate(prefab, startPos, Quaternion.identity);
        obj.Target = target;
        obj.UpdateTranform();
        return obj;
    }
    public static LaserAimming Play(Vector3 startPos, Vector3 dest, string vfxName)
    {
        LaserAimming prefab = ResourcesCache.Load<LaserAimming>("Prefabs/Effects/" + vfxName);
        LaserAimming obj = Instantiate(prefab, startPos, Quaternion.identity);
        obj.Destination = dest;
        obj.Target = null;
        obj.UpdateTranform();
        return obj;
    }

    void Update() 
    {
        UpdateTranform();
    }

    private void UpdateTranform()
    {
        Vector3 dir = CurrentDestPoint - transform.position;
        dir.z = 0;
        transform.localScale = Vector3.one;
        transform.right = dir.normalized;
        Laser.transform.localScale = new Vector3(Target == null ? 0 : dir.magnitude, 1, 1);
        Outro.transform.position = CurrentDestPoint;
    }

}
