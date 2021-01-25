using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RGameObject : MonoBehaviour
{
    private Dictionary<Type, RBaseComponent> _components = new Dictionary<Type, RBaseComponent>();

    public void AddRComponent<T>() where T : RBaseComponent, new ()
    {
        _components.Add(typeof(T), new T());
    }
    public T GetRComponent<T>() where T : RBaseComponent
    {
        if(_components.ContainsKey(typeof(T)))
            return (T)Convert.ChangeType(_components[typeof(T)], typeof(T));
        return default;
    }
    public abstract void Init();
    public abstract void Release();
}
