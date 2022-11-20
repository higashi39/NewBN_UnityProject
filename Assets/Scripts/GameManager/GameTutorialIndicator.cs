using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialIndicator : MonoBehaviour
{
    public static GameTutorialIndicator instance;

    [field: Header("Property")]
    [field: SerializeField] public bool IsShowTutorial { set; get; } = true;
    [field: SerializeField] public bool IsShowTutorialThisTime { set; get; } = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

}
