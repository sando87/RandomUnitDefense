using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCharactors", menuName = "Scriptable Object Asset/EnemyCharactors")]
public class EnemyCharactors : ScriptableObjectDictionary<long, EnemyCharactor>
{
    protected override long GetID(EnemyCharactor data)
    {
        return data.ID;
    }
}

[System.Serializable]
public class EnemyCharactor
{
    [Identifier] public long ID;
    public string name;
    public Sprite image;
    public GameObject prefab;
}
