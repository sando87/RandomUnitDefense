using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMissile : MonoBehaviour
{
    [SerializeField] private GameObject Explosion = null;
    [SerializeField] private float MoveSpeed = 5;

    public BaseObject Target { get; private set; }
    public Action<BaseObject> EventHit { get; set; }

    // Start is called before the first frame update
    public void Launch(BaseObject target)
    {
        Target = target;
        StartCoroutine(MoveToDestination());
    }

    private IEnumerator MoveToDestination()
    {
        Vector3 destination = Vector3.zero;
        while (true)
        {
            if (Target != null)
                destination = Target.Body.Center;

            Vector3 dir = destination - transform.position;
            dir.z = 0;
            dir.Normalize();
            Vector3 nextPos = transform.position + (dir * MoveSpeed * Time.deltaTime);
            Vector3 nextDir = destination - nextPos;
            nextDir.z = 0;
            if (Vector3.Dot(dir, nextDir) < 0) //목표지점을 지나친 경우
            {
                transform.position = destination;
                break;
            }
            else
            {
                HeadToTarget(nextPos);
                transform.position = nextPos;
            }
            yield return null;
        }

        if (Target != null)
            EventHit?.Invoke(Target);

        if(Explosion != null)
            Explosion.SetActive(true);
            
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 1.0f);
    }

    private void HeadToTarget(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        dir.z = 0;
        dir.Normalize();

        Quaternion qua = Quaternion.FromToRotation(Vector3.right, dir);
        transform.localRotation = qua;
    }


}
