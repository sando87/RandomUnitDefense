using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Vector3 Offset = new Vector3(0, 0, -10);

    private Camera mCamera = null;
    private GameObject mTarget = null;

    public Camera Camera { get { return mCamera; } }
    public float CurrentHeight { get { return mCamera.orthographicSize * 2.0f; } }
    public float CurrentWidth { get { return CurrentHeight * mCamera.aspect; } }
    public float ToHeight(float width) { return width / mCamera.aspect; }
    public Rect CameraArea
    {
        get
        {
            Rect area = new Rect();
            area.size = new Vector2(CurrentWidth, CurrentHeight);
            area.center = new Vector2(transform.position.x, transform.position.y);
            return area;
        }
    }
    public Rect LimitArea { get; set; }

    void Start()
    {
        mCamera = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        if(mTarget != null)
        {
            transform.position = mTarget.transform.position + Offset;
            LimitCameraMovement();
            transform.Snap();
            // transform.LookAt(Target.transform);
        }
    }

    public void SetTarget(GameObject target)
    {
        mTarget = target;
    }
    private void LimitCameraMovement()
    {
        Rect limitedArea = CameraArea.LimitRectMovement(LimitArea);
        transform.ExSetPosition2D(limitedArea.center);
    }
    public void ShakeCamera(float strength)
    {
        float duration = strength * 0.5f;
        mCamera.transform.DOShakePosition(duration, strength).SetEase(Ease.OutQuart);
    }

}
