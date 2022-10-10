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

    [SerializeField]NewPresentBoxManager newPresentBoxManager;
    NewPresentBox newPresentBox;

    private void Awake()
    {
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        newPresentBox = FindObjectOfType<NewPresentBox>();
    }

    void Start()
    {
        clickCount = 0;
        ps.Stop();

    }

    void Update()
    {
        if(newPresentBoxManager.presentTimeNow == 5.0f)
        {
            clickCount = 0;
        }


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

        if(newPresentBoxManager.presentTimeNow < 0.0f  && clickCount < 15)
        {
            clickCount = 0;
            StartCoroutine(newPresentBoxManager.NotGetPresent());
        }
    }

    public void Clicked()
    {
        Debug.Log("クリックした");
    }
}
