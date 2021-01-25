using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGame : MonoBehaviour
{
    static private bool _IsAwake = false;
    static private Dictionary<Type, RManager> _Mangers = null;

    static public string Region
    {
        get
        {
            if (Application.systemLanguage == SystemLanguage.Korean)
                return "KOR";
            else
                return "ENG";
        }
    }


    public static T Get<T>()
    {
        if (!_IsAwake)
        {
            GameObject gameInstanceObj = GameObject.Find("GameInstance");
            if (gameInstanceObj == null)
                gameInstanceObj = new GameObject("GameInstance");

            gameInstanceObj.AddComponent<RGame>();

            new RGame().InitManager();
        }

        if (_Mangers.ContainsKey(typeof(T)))
            return (T)Convert.ChangeType(_Mangers[typeof(T)], typeof(T));

        return default(T);
    }


    private void Awake()
    {
        InitManager();
    }


    private void InitManager()
    {
        if (_Mangers == null)
        {
            GameObject parentObj = GameObject.Find("Manager");
            if (parentObj == null)
                parentObj = new GameObject("Manager");

            _Mangers = new Dictionary<Type, RManager>();

            RManager[] managerList = FindObjectsOfType<RManager>();
            foreach (RManager manager in managerList)
            {
                manager.Init();
                _Mangers.Add(manager.GetType(), manager);
            }

            var types = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(RManager));

            foreach (Type type in types)
            {
                if (_Mangers.ContainsKey(type))
                    continue;
                
                var manager = new GameObject().AddComponent(type) as RManager;
                manager.gameObject.name = type.ToString();
                manager.transform.parent = parentObj.transform;
                manager.Init();

                _Mangers.Add(manager.GetType(), manager);
            }

            DontDestroyOnLoad(parentObj);

            _IsAwake = true;
            DontDestroyOnLoad(this);
        }
    }
}
