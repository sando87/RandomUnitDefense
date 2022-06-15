using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserCharactors", menuName = "Scriptable Object Asset/UserCharactors")]
public class UserCharactors : ScriptableObjectDictionary<long, UserCharactor>
{
    private static UserCharactors mInst = null;
    public static UserCharactors Inst
    {
        get
        {
            if (mInst == null)
            {
                mInst = Resources.Load<UserCharactors>("Database/UserCharactors");
            }
            return mInst;
        }
    }
    protected override long GetID(UserCharactor data)
    {
        return data.ID;
    }
}

[System.Serializable]
public class UserCharactor
{
    [Identifier] public long ID;
    public string name;
    public Sprite image;
    public GameObject prefab;
    public string skillDescription;
}
