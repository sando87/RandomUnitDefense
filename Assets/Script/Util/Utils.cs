using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 Random(Vector3 pos, float range)
    {
        Vector3 ret = pos;
        ret.x += UnityEngine.Random.Range(-range, range);
        ret.y += UnityEngine.Random.Range(-range, range);
        return ret;
    }
}

