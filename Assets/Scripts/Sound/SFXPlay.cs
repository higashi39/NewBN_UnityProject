using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlay : MonoBehaviour
{
    [field: Header("Component References")]
    AudioSource sound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }

    //SFXを再生する
    //もし再生中場合、リセットしない
    public void PlaySFXSound()
    {
        if (!sound.isPlaying)
        {
            sound.Play();
        }
    }

    public void PlaySFXSoundForce()
    {
        sound.Play();
    }

    public void PlaySFXSoundFromStartSecond(float time)
    {
        if (!sound.isPlaying)
        {
            sound.time = time;
            sound.Play();
        }
    }

    //SFXを止まる
    public void StopSFXSound()
    {
        if (sound.isPlaying)
        {
            sound.Stop();
        }
    }
}
