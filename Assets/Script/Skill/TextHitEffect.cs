using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextHitEffect : MonoBehaviour
{
    private GravityMovement mMovement = null;
    private TextMeshPro mTextMesh = null;

    void Awake() 
    {
        mTextMesh = GetComponent<TextMeshPro>();
        mMovement = GetComponent<GravityMovement>();

        // 텍스트가 튕겨져 올랐다가 떨어지는 형태로 세팅
        // mMovement.GravityAcc = new Vector3(0, -50, 0);
        // mMovement.ExtForce = new Vector3(10, 20, 0);
    }

    public void SetText(int number)
    {
        mTextMesh.text = number.ToString();
    }

}
