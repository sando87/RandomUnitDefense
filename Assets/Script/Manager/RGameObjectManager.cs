using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGameObjectManager : RManager
{
    private Dictionary<int, RGameObject> _ActiveGameObjects = new Dictionary<int, RGameObject>();
    private Recycler.PrefabHolder _PrefabHolder = null;
    private readonly string RECYCLER_PATH = "Prefabs/RecycleObjects/@PrefabHolder";
    public override void Init()
    {
        base.Init();
        if(!_PrefabHolder)
        {
            var holder = Resources.Load(RECYCLER_PATH) as GameObject;
            if(holder)
            {
                var go = GameObject.Instantiate(holder);
                go.SetActive(false);
                go.name = "@PrefabHolder";
                go.transform.parent = this.transform;
                _PrefabHolder = go.GetComponent<Recycler.PrefabHolder>();
            }
        }
        
    }
    // 생성된 오브젝트는 캐싱하고있지 말것
    public int AcquireRGameObject(string name, RGameObject rGameObject)
    {
        rGameObject = null;
        // 오브젝트 풀 기능이 들어와야 함.
        // 일단은 그냥 클론해서 넘겨준다.
        var prefab = _PrefabHolder.Get(name);
        if(prefab)
        {
            var go = GameObject.Instantiate(prefab.gameObject);

            var rObj = go.GetComponent<RGameObject>();
            int id = rObj.GetInstanceID();
            _ActiveGameObjects.Add(id, rObj);
            return id;
        }


        return -1;
    }

    public RGameObject GetRGameObject(int id)
    {
        if (_ActiveGameObjects.TryGetValue(id, out RGameObject value))
            return value;
        return default(RGameObject);
    }
    
    public void ReleaseRGameObject(int id)
    {
        // 일단은 디스트로이 
        // 컨테이너에 반환해서 풀 유지하는 구조를 만들어보자 
        if(_ActiveGameObjects.TryGetValue(id, out RGameObject value))
        {
            _ActiveGameObjects.Remove(id);
            if(null != value)
            {
                value.Release();
                Destroy(value.gameObject);
            }
        }
    }
}
