using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 처음 오브젝트가 배치된 곳으로부터 카메라 이동에 따라가도록 추가적으로 움직임 제어
// 뒤에 배경들이 카메라로부터 거리에 따른 원근감 표현을 위해 구현

public class DistantObjectFromCamera : MonoBehaviour
{
    // 카메라로부터의 거리를 0~1로 환산한 값(0이면 처음 위치에 고정되고 1이면 카메라위치를 항상 따라간다)
    [Range(0, 1)] [SerializeField] float Distance;

    // 기준이 되는 카메라 대상
    private Camera mTargetCamera;

    // 처음 객체의 시작 위치
    private Vector3 mStartPosition = Vector3.zero;

    void Start()
    {
        mTargetCamera = Camera.main;
        mStartPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 curPos = transform.position;
        curPos.x = mStartPosition.x * (1 - Distance) + mTargetCamera.transform.position.x * Distance;
        curPos.y = mStartPosition.y * (1 - Distance) + mTargetCamera.transform.position.y * Distance;
        transform.position = curPos;
        transform.Snap();
    }
}
