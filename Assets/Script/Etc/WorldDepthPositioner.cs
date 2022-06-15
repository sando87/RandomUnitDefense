using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldDepthPositioner : MonoBehaviour
{
    void Update() 
    {
        UpdateWorldPosZ();
    }

    private void UpdateWorldPosZ()
    {
        //2.5D 특성상 y값이 높을수록 카메라보다 더 멀리 배치되도록 하기 위한 z값 조절(그라운드 위 모든 유닛에게 적용 필요)
        Vector3 pos = transform.position;
        pos.z = pos.y * 0.1f;
        transform.position = pos;
    }
}
