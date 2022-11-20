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

    //SFX���Đ�����
    //�����Đ����ꍇ�A���Z�b�g���Ȃ�
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

    //SFX���~�܂�
    public void StopSFXSound()
    {
        if (sound.isPlaying)
        {
            sound.Stop();
        }
    }
}
