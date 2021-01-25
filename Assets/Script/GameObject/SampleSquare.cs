using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSquare : RGameObject
{
    public override void Init()
    {
        StartCoroutine(CoUpdate());
    }

    public override void Release()
    {
    }
    private List<Vector3> targets = new List<Vector3>()
    {
        new Vector3(0.0f, 3.0f),
        new Vector3(0.0f, 4.0f),
        new Vector3(4.0f, 2.0f),
        new Vector3(2.0f, 1.0f),
    };
    private IEnumerator CoUpdate()
    {
        float updateTime = 0.0f;
        var target = targets[Random.Range(0, targets.Count)];
        while(updateTime <= 4.0f)
        {
            updateTime += Time.deltaTime;
            transform.position = Vector3.Lerp(Vector3.zero, target, updateTime / 4.0f);
            yield return null;
        }
        Yielders.GetWaitForSeconds(0.5f);
        RGame.Get<RGameObjectManager>().ReleaseRGameObject(this.GetInstanceID());
    }
}
