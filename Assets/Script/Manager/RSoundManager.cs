using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSoundManager : RManager
{
    [SerializeField] private AudioSource Player = null;

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
}
