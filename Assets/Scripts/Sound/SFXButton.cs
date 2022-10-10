using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXButton : MonoBehaviour
{

    [field: Header("Instance")]
    [field: SerializeField] public static SFXButton Instance { set; get; }

    [field: Header("Component References")]
    AudioSource audio;

    private void Awake()
    {
        if (Instance == null)
        {
            audio = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonPressSFX()
    {
        audio.Play();
    }

}
