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
    }


}
