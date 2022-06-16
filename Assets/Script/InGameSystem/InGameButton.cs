using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InGameButtonClick : UnityEvent { }

public class InGameButton : MonoBehaviour
{
    public InGameButtonClick EventClick;

    private Camera mWorldCam = null;
    private Collider mDownCollider = null;
    private Collider mTargetCollider = null;

    void Start()
    {
        mWorldCam = Camera.main;
        mTargetCollider = GetComponent<Collider>();
    }

    void OnEnable()
    {
        InputWrapper.Instance.EventDownTriggered += OnDownTriggered;
        InputWrapper.Instance.EventUpTriggered += OnUpTriggered;
    }

    void OnDisable()
    {
        InputWrapper.Instance.EventDownTriggered -= OnDownTriggered;
        InputWrapper.Instance.EventUpTriggered -= OnUpTriggered;
    }


    private void OnDownTriggered(InputType obj)
    {
        mDownCollider = null;
        Vector3 worldPt = Camera.main.ScreenToWorldPoint(InputWrapper.Instance.MousePosition());
        RaycastHit[] hits = MyUtils.RaycastAllFromTo(mWorldCam.transform.position, worldPt, 1 << gameObject.layer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == mTargetCollider)
            {
                mDownCollider = mTargetCollider;
                break;
            }
        }
    }
    private void OnUpTriggered(InputType obj)
    {
        if(mDownCollider == null) return;

        Vector3 worldPt = Camera.main.ScreenToWorldPoint(InputWrapper.Instance.MousePosition());
        RaycastHit[] hits = MyUtils.RaycastAllFromTo(mWorldCam.transform.position, worldPt, 1 << gameObject.layer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == mDownCollider)
            {
                EventClick?.Invoke();
                break;
            }
        }

        mDownCollider = null;
    }


}
