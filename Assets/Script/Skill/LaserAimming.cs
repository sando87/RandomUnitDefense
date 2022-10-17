using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAimming : MonoBehaviour
{
    [SerializeField] GameObject Laser = null;
    [SerializeField] GameObject Outro = null;

    public GameObject Target { get; set; } = null;

    public static LaserAimming Play(Vector3 startPos, GameObject target, string vfxName)
    {
        LaserAimming prefab = ResourcesCache.Load<LaserAimming>("Prefabs/Effects/" + vfxName);
        LaserAimming obj = Instantiate(prefab, startPos, Quaternion.identity);
        obj.Target = target;
        obj.UpdateTranform();
        return obj;
    }

    void Update() 
    {
        UpdateTranform();
    }

    private void UpdateTranform()
    {
        if (Target == null)
        {
            transform.localScale = Vector3.zero;
            return;
        }
            
        Vector3 dir = Target.transform.position - transform.position;
        dir.z = 0;
        transform.localScale = Vector3.one;
        transform.right = dir.normalized;
        Laser.transform.localScale = new Vector3(dir.magnitude, 1, 1);
        Outro.transform.position = Target.transform.position;
    }
}
