using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 프로젝트 전역적으로 사용되는 함수들
public static class InGameUtils
{
    public static BaseObject GetBaseObject(this GameObject obj)
    {
        return obj.GetComponentInParent<BaseObject>();
    }
    public static BaseObject GetBaseObject(this MonoBehaviour obj)
    {
        return obj.GetComponentInParent<BaseObject>();
    }
    public static BaseObject GetBaseObject(this Collider obj)
    {
        return obj.GetComponentInParent<BaseObject>();
    }
    public static T GetComponentInBaseObject<T>(this Collider obj) where T : MonoBehaviour
    {
        BaseObject baseObj = obj.GetBaseObject();
        if (baseObj != null)
        {
            return baseObj.GetComponentInChildren<T>();
        }
        return null;
    }
    public static Vector3 ZeroZ(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }
    public static Vector3 GetRandomPos(Vector3 center, float radius)
    {
        float ranX = UnityEngine.Random.Range(center.x - radius, center.x + radius);
        float ranY = UnityEngine.Random.Range(center.y - radius, center.y + radius);
        float ranZ = UnityEngine.Random.Range(center.z - radius, center.z + radius);
        return new Vector3(ranX, ranY, ranZ);
    }

    // 100% 효과면 value의 2배, -100%이면 value의 0.5배로 동작
    public static float Multiply(this int value, int percent)
    {
        float rate = (100 + Mathf.Abs(percent)) * 0.01f;
        return (percent > 0) ? (value * rate) : (value / rate);
    }

    public static Vector3? RaycastPointOnGround()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
        {
            return hit.point;
        }
        return null;
    }
    public static BaseObject RaycastEnemyObject()
    {
        bool isEnemy = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100);
        return isEnemy ? hit.collider.GetComponentInParent<BaseObject>() : null;
    }
    public static Vector3 RotateVector(Vector3 vec, Vector3 axis, float degree)
    {
        return Quaternion.AngleAxis(degree, axis.normalized) * vec;
    }
    public static Vector3[] CalcMultiDirections(Vector3 centerDir, Vector3 axis, int count, float stepDegree)
    {
        centerDir.Normalize();
        axis.Normalize();

        if (count <= 1)
            return new Vector3[1] { centerDir };

        List<Vector3> rets = new List<Vector3>();
        float totalDeg = stepDegree * (count - 1);
        Vector3 startDir = RotateVector(centerDir, axis, -totalDeg * 0.5f);
        rets.Add(startDir);
        for(int i = 1; i < count; ++i)
        {
            Vector3 dir = RotateVector(startDir, axis, stepDegree * i);
            rets.Add(dir);
        }
        return rets.ToArray();
    }
    public static Collider[] OverlapSphere(Vector3 center, float radius, int layerMask)
    {
        return Physics.OverlapSphere(center, radius, layerMask);
    }
    public static Collider[] OverlapCollider(Collider collider)
    {
        Collider[] rets = null;
        if (collider is BoxCollider)
        {
            BoxCollider box = (BoxCollider)collider;
            rets = Physics.OverlapBox(box.transform.position + box.center, box.size * 0.5f, box.transform.rotation);
        }
        else if (collider is CapsuleCollider)
        {
            CapsuleCollider capsule = (CapsuleCollider)collider;
            Vector3 dir = capsule.direction == 0 ? capsule.transform.right :
            (capsule.direction == 1 ? capsule.transform.up : capsule.transform.forward);

            Vector3 pt1 = capsule.transform.position + capsule.center + dir * capsule.height * 0.5f;
            Vector3 pt2 = capsule.transform.position + capsule.center - dir * capsule.height * 0.5f;
            rets = Physics.OverlapCapsule(pt1, pt2, capsule.radius);
        }
        else if(collider is SphereCollider)
        {
            SphereCollider sphere = (SphereCollider)collider;
            rets = Physics.OverlapSphere(sphere.transform.position + sphere.center, sphere.radius);
        }
        else
        {
            LOG.warn();
        }
        return rets;
    }

    public static bool IsCloseEnough(Vector3 posA, Vector3 posB, float distance)
    {
        return (posA - posB).sqrMagnitude <= (distance * distance);
    }
}
