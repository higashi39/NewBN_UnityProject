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

    //SFX���~�܂�
    public void StopSFXSound()
    {
        if (sound.isPlaying)
        {
            sound.Stop();
        }
    }
}
