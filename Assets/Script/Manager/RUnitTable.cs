using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LineMob
{
    public string Name;
    public float HP;
    public float Armor;
    public float MoveSpeed;

    public Animator Anim { get; set; }
    public Sprite Photo { get; set; }
}

public class RUnitTable : RManager
{
    const string LineMobsFileName = "UnitTable/Mobs";

    [SerializeField] private List<Animator> MobAnims = new List<Animator>();
    [SerializeField] private List<Sprite> MobPhotos = new List<Sprite>();

    private Dictionary<string, LineMob> Mobs = new Dictionary<string, LineMob>();

    public override void Init()
    {
        base.Init();
        LoadMobTable();
    }

    public LineMob GetMobInfo(string name)
    {
        if (!Mobs.ContainsKey(name))
        {
            Debug.Log("No Mob Content");
            return null;
        }
            
        return Mobs[name];
    }

    public LineMob[] GetTotalMobList()
    {
        return new List<LineMob>(Mobs.Values).ToArray();
    }

    private void LoadMobTable()
    {
        TextAsset jsonText = Resources.Load<TextAsset>(LineMobsFileName);
        LineMob[] mobs = JsonHelper.FromJsonArray<LineMob>(jsonText.text);
        foreach (LineMob mob in mobs)
        {
            Animator animator = MobAnims.Find((anim) => { return anim.name == mob.Name; });
            if(animator == null)
                Debug.Log("No MobAnim for " + mob.Name);

            Sprite image = MobPhotos.Find((img) => { return img.name == mob.Name; });
            if (image == null)
                Debug.Log("No MobPhoto for " + mob.Name);

            mob.Anim = animator;
            mob.Photo = image;
            Mobs[mob.Name] = mob;
        }
    }


    //이건 개발자가 일일이 조작하기 귀찮을 경우 테스트용으로 사용
    private void SaveMobTable()
    {
        List<LineMob> mobs = new List<LineMob>();
        for(int i = 0; i < 10; ++i)
        {
            LineMob mob = new LineMob();
            mob.Name = "fenrir";
            mob.HP = 100 + i * 50;
            mob.Armor = i;
            mob.MoveSpeed = 1;
            mobs.Add(mob);
        }
        
        //LineMob[] mobs = new List<LineMob>(Mobs.Values).ToArray();
        string jsonText = JsonHelper.ToJson<LineMob>(mobs.ToArray());

        string outputpath = Application.persistentDataPath + "/" + LineMobsFileName + ".json";
        try
        {
            File.WriteAllText(outputpath, jsonText);
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
        
    }


}
