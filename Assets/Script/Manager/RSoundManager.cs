using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioClipType
{
    None, ShowHUD, ClickButton
}

public class RSoundManager : RManager
{
    [SerializeField] private AudioSource Player = null;

    [SerializeField] private AudioClip ShowHUD = null;
    [SerializeField] private AudioClip ClickButton = null;

    public override void Init()
    {
        base.Init();
    }

    public bool ToggleMute()
    {
        Player.mute = !Player.mute;
        return Player.mute;
    }

    public void PlayBackgroundMusic(AudioClip bkMusic)
    {
        Player.clip = bkMusic;
        Player.loop = true;
        Player.Play();
    }


    public void PlaySFX(AudioClip sound)
    {
        Player.PlayOneShot(sound);
    }

    public void PlaySFX(AudioClipType type)
    {
        switch(type)
        {
            case AudioClipType.ShowHUD: Player.PlayOneShot(ShowHUD); break;
            case AudioClipType.ClickButton: Player.PlayOneShot(ClickButton); break;
            default: break;
        }
    }
}
