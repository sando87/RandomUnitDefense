using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Disappearable : MonoBehaviour
{
    [SerializeField] float CoolTime = 3.0f;
    [SerializeField] float HideTime = 3.0f;

    private BaseObject mBaseObject = null;
    private float mCooltime = 0;

    void Start() 
    {
        mBaseObject = this.GetBaseObject();
    }

    void Update()
    {
        // 플레이어가 이 블럭위에서 3초동안 가만히 있으면 블럭 숨기는 로직
        if(IsPlayerOnThisBlock())
        {
            if(mCooltime > CoolTime)
            {
                mCooltime = 0;
                Hide(HideTime);
            }
            else
            {
                mCooltime += Time.deltaTime;
            }
        }
        else
        {
            mCooltime = 0;
        }
    }

    private bool IsPlayerOnThisBlock()
    {
        return Physics.Raycast(mBaseObject.Body.Center, Vector3.up, Consts.BlockSize, 1 << LayerID.Player);
    }

    private void Hide(float sec)
    {
        // 블럭 숨긴 뒤 sec 초뒤에 다시 나타나게함..
        CancelInvoke();
        Invoke("ShowAgain", sec);
        mBaseObject.gameObject.SetActive(false);
    }

    private void ShowAgain()
    {
        mCooltime = 0;
        mBaseObject.gameObject.SetActive(true);
    }

}
