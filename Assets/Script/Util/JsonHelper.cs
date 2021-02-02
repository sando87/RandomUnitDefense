using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static T FromJson<T>(string json)
    {
        T obj = JsonUtility.FromJson<T>(json);
        return obj;
    }
    public static T[] FromJsonArray<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }
    public static string ToJson<T>(T[] array, bool prettyPrint = true)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
    public static string ToJson<T>(T obj, bool prettyPrint = true)
    {
        return JsonUtility.ToJson(obj, prettyPrint);
    }
    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
