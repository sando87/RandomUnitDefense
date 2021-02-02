using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Recycler
{

    public class PrefabHolder : MonoBehaviour
    {
        [SerializeField]
        List<RGameObject> _prefabs = new List<RGameObject>();

        public RGameObject Get(string name)
        {
            var find = _prefabs.Find((x) =>
            {
                if (x)
                    return x.name.Equals(name);
                return false;
            });
            return find;
        }

        public T[] GetList<T>()
        {
            List<T> rets = new List<T>();
            foreach(RGameObject prefab in _prefabs)
            {
                T comp = prefab.GetComponent<T>();
                if (comp != null)
                    rets.Add(comp);
            }
            return rets.ToArray();
        }
    }


}
