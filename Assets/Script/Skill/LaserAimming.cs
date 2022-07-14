using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAimming : MonoBehaviour
{
    [SerializeField] GameObject Laser = null;
    [SerializeField] GameObject Outro = null;

    public GameObject Target { get; set; } = null;

    private Vector3 mDestPos = Vector3.zero;

    public static LaserAimming Play(Vector3 startPos, GameObject target)
    {
        LaserAimming prefab = ResourcesCache.Load<LaserAimming>("Prefabs/Effects/GunnerLaser");
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
        if (Target != null)
            mDestPos = Target.transform.position;

        Vector3 dir = mDestPos - transform.position;
        dir.z = 0;
        transform.right = dir.normalized;
        Laser.transform.localScale = new Vector3(dir.magnitude, 1, 1);
        Outro.transform.position = mDestPos;
    }
}
