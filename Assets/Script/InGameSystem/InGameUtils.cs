﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static bool IsInDetectRange(this BaseObject obj, BaseObject target, float range)
    {
        Vector2 extends = new Vector2(range, range * 0.4f);

        Rect area = new Rect();
        area.size = extends * 2;
        area.center = obj.transform.position;
        return area.Contains(target.transform.position);
    }
    public static Collider[] DetectAround(this GameObject obj, float range, int layerMask)
    {
        return DetectAround(obj.transform.position, range, layerMask);
    }
    public static Collider[] DetectAround(this BaseObject obj, float range, int layerMask)
    {
        return DetectAround(obj.transform.position, range, layerMask);
    }
    public static Collider DetectMostCloseAround(this BaseObject obj, float range, int layerMask)
    {
        Collider[] cols = obj.DetectAround(range, layerMask);
        if(cols.Length > 0)
        {
            var sortCols = cols.OrderBy((col) => (obj.transform.position - col.transform.position).sqrMagnitude);
            if(sortCols.First().GetBaseObject() == obj)
                return cols.Length == 1 ? null : sortCols.ElementAt(1);
            else
                return sortCols.ElementAt(0);
        }
        return null;
    }
    public static Collider[] DetectAround(this BaseObject obj, BoxCollider boxArea, int layerMask)
    {
        return Physics.OverlapBox(boxArea.bounds.center, boxArea.bounds.extents, Quaternion.identity, layerMask);
    }
    public static Collider[] DetectAround(Vector3 pos, float range, int layerMask)
    {
        return Physics.OverlapSphere(pos, range, layerMask);
    }

    public static bool DetectAround<T>(this BaseObject obj, float range, int layerMask, List<T> rets) where T : MonoBehaviour
    {
        bool isFounded = false;
        Vector2 extends = new Vector2(range, range * 0.4f);
        Collider[] hitColliders = Physics.OverlapBox(obj.transform.position, extends, Quaternion.identity, layerMask);
        foreach (Collider hitCollider in hitColliders)
        {
            BaseObject baseObj = hitCollider.GetBaseObject();
            if (baseObj == null || baseObj == obj)
                continue;

            T mob = hitCollider.GetComponentInBaseObject<T>();
            if (mob != null)
            {
                rets.Add(mob);
                isFounded = true;
            }
        }
        return isFounded;
    }
    // 시작점에서 끝점으로가는 방향이 +y축 기준에 대한 각도를 n등분했을때 index 반환
    public static int GetVerticalIndex(Vector3 startPos, Vector3 endPos, int divideCount)
    {
        if(divideCount <= 0)
            return 0;
            
        Vector3 dir = endPos - startPos;
        dir.z = 0;
        float deg = MyUtils.GetDegree(Vector3.up, dir.normalized);
        deg = Mathf.Abs(deg);
        int stepDegree = 180 / divideCount;
        int animIndex = (int)deg / stepDegree;
        return animIndex;
    }
}
