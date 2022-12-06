using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    [SerializeField] float MaxMoveSpeed = 10;

    public Action<BaseObject> EventHit { get; set; } = null;

    float mMoveSpeed = 10;
    float mSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        mSize = transform.localScale.x;
        StartCoroutine(CoStartMoving());
    }

    void Update()
    {
        transform.position += transform.right * mMoveSpeed * Time.deltaTime;
    }

    private IEnumerator CoStartMoving()
    {
        float time = 0;
        while(mSize > 0.1f && time < 3)
        {
            Collider[] cols = InGameUtils.DetectAround(transform.position, 0.2f, 1 << LayerID.Enemies);
            if(cols.Length > 0)
            {
                time = 0;
                mSize -= 0.1f;
                mMoveSpeed -= 0.5f;
                transform.localScale = new Vector3(mSize, mSize, 1);
                EventHit?.Invoke(cols[0].GetBaseObject());
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                time += Time.deltaTime;
                if(mMoveSpeed < MaxMoveSpeed)
                {
                    mMoveSpeed += (5 * Time.deltaTime);
                }

                yield return null;
            }
        }

        Destroy(gameObject);
    }

}
