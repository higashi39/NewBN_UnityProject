using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PresentAction : MonoBehaviour
{

    [SerializeField] public int clickCount;

    [SerializeField] ParticleSystem ps;
    private Vector3 mousePos;

    NewPresentBoxManager newPresentBoxManager;
    NewPresentBox newPresentBox;

    void Start()
    {
        clickCount = 0;
        ps.Stop();
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        newPresentBox = FindObjectOfType<NewPresentBox>();
    }

    void Update()
    {
        if (newPresentBoxManager.presentTimeNow < 5.0f && newPresentBoxManager.presentTimeNow > 0.0f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ++clickCount;
                mousePos = Input.mousePosition;
                mousePos.z = 3f;
                Instantiate(ps, Camera.main.ScreenToWorldPoint(mousePos), Quaternion.identity);

                ps.Play();
            }
        }

        if (clickCount > 14)
        {
            clickCount = 0;
            StartCoroutine(newPresentBoxManager.GetPresent());
            Debug.Log("15回クリックした");
        }
    }

    public void Clicked()
    {
        Debug.Log("クリックした");
    }
}
