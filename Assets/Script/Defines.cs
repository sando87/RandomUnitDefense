using System;
using UnityEngine;


[Serializable]
public class UserSaveData
{
    // 이전버전과의 호환성을 위해 존재
    [SerializeField] private int dataVersion = GamePlayData.DataVersion;
    // 유저 ID
    [SerializeField] private string UserID = "";
    // 이용약관 동의 여부
    [SerializeField] private bool isAgreedTerms = false;
    //배경private음악 볼륨
    [SerializeField] private float musicVolume = 0.5f;
    //효과음 볼륨
    [SerializeField] private float soundVolume = 0.5f;
    //언어 설정
    [SerializeField] private LanguageType language = LanguageType.English;


    public int DataVersion { get => dataVersion; set { dataVersion = value; GamePlayData.Save(); } }
    public string UserID1 { get => UserID; set { UserID = value; GamePlayData.Save(); } }
    public bool IsAgreedTerms { get => isAgreedTerms; set { isAgreedTerms = value; GamePlayData.Save(); } }
    public float MusicVolume { get => musicVolume; set { musicVolume = value; GamePlayData.Save(); } }
    public float SoundVolume { get => soundVolume; set { soundVolume = value; GamePlayData.Save(); } }
    public LanguageType Language { get => language; set { language = value; GamePlayData.Save(); } }


    public void ResetUserSettingToDefault()
    {
        musicVolume = 0.5f;
        soundVolume = 0.5f;

        GamePlayData.Save();
    }
}

public class Consts
{
    public const string VFXPath = "VFX/";
    public const string SFXPath = "Sound/SFX/";
}

public class AnimParam
{
    public static readonly int ActionType = Animator.StringToHash("ActionType");
    public static readonly int DoActionTrigger = Animator.StringToHash("DoActionTrigger");
    public static readonly int VerticalDegreeIndex = Animator.StringToHash("VerticalDegreeIndex");
    public static readonly int VerticalIndexFloat = Animator.StringToHash("VerticalIndexFloat");
    public static readonly int ActionLoopSpeed = Animator.StringToHash("actionLoopSpeed");
}

public enum AnimActionID
{
    Idle = 0,
    Move = 1,
    Death = 2,
    ActionA = 3,
    ActionB = 4,
    ActionLoop = 5,
    Stun = 6,
}

public class LayerID
{
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int Enemies = LayerMask.NameToLayer("Enemies");
    public static readonly int ThemeBackground = LayerMask.NameToLayer("BackgroundMap");
    public static readonly int Ignorable = LayerMask.NameToLayer("Ignorable");
    public static readonly int WorldUI = LayerMask.NameToLayer("WorldUI");
}

public class SortingLayerID
{
    public static readonly int Background = SortingLayer.NameToID("Background");
    public static readonly int Default = SortingLayer.NameToID("Default");
    public static readonly int Between = SortingLayer.NameToID("Between");
    public static readonly int PlatformsBack = SortingLayer.NameToID("PlatformsBack");
    public static readonly int Player = SortingLayer.NameToID("Player");
    public static readonly int PlatformsFront = SortingLayer.NameToID("PlatformsFront");
    public static readonly int Foreground = SortingLayer.NameToID("Foreground");
    public static readonly int VisibleParticles = SortingLayer.NameToID("VisibleParticles");
    public static readonly int UI = SortingLayer.NameToID("UI");
}

[System.Serializable]
public struct Percent //단위 [%]
{
    [SerializeField] private int value; // ~ -100 ~ 0 ~ 100% ~
    public Percent(int val) { value = val; }
    public int Value { get => value; }
    private float Factor { get => (1 + (Mathf.Abs(value) * 0.01f)); }  // return 1.0 ~ 2.0 ~
    private float Multiplier { get => value > 0 ? Factor : (1 / Factor); } // 100% => x2, -100% => x0.5
    public void SetZero() { value = 0; }
    
    // Percent형을 int형으로 암시적 형변환 가능 예) int damage = new Percent(_damage);
    public static implicit operator int(Percent info) => info.value;

    // int형을 Percent로 암시적 형변환 가능 예) Percent info = 1;
    public static implicit operator Percent(int percent) => new Percent(percent);

    public static float operator *(float val, Percent per) { return val * per.Multiplier; }
    public static float operator *(int val, Percent per) { return val * per.Multiplier; }
    public static Percent operator +(Percent a, int b) { return new Percent(a.value + b); }
    public static Percent operator -(Percent a, int b) { return new Percent(a.value - b); }
    public static Percent operator *(Percent a, float b) { return new Percent((int)(a.value * b)); }
    public static Percent operator +(Percent a, Percent b) { return new Percent(a.value + b.value); }
    public static Percent operator -(Percent a, Percent b) { return new Percent(a.value - b.value); }
    public override string ToString() { return value.ToString(); }
}