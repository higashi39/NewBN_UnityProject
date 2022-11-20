using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PresentAction : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] public int clickCount;

    [SerializeField] ParticleSystem ps;
    private Vector3 mousePos;

    [SerializeField] NewPresentBoxManager newPresentBoxManager;
    NewPresent_Pinata newPresentBox;

    [field: Header("SFX")]
    [field: SerializeField] SFXPlay SfxPlayClickPinata { set; get; }

    private void Awake()
    {
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        newPresentBox = FindObjectOfType<NewPresent_Pinata>();
    }

    void Start()
    {
        clickCount = 0;
        ps.Stop();

    }

    void Update()
    {
        if (newPresentBoxManager.presentTimeNow == 5.0f)
        {
            clickCount = 0;
        }


        //if (newPresentBoxManager.presentTimeNow < 5.0f && newPresentBoxManager.presentTimeNow > 0.0f)
        //{
        //    //if (Input.GetMouseButtonDown(0))
        //    //{
        //    //    Click();
        //    //    //++clickCount;
        //    //    //mousePos = Input.mousePosition;
        //    //    //mousePos.z = 3f;
        //    //    //Instantiate(ps, Camera.main.ScreenToWorldPoint(mousePos), Quaternion.identity);

        //    //    //ps.Play();
        //    //}
        //}

        if (clickCount > 14)
        {
            clickCount = 0;
            StartCoroutine(newPresentBoxManager.GetPresent());
            Debug.Log("15‰ñƒNƒŠƒbƒN‚µ‚½");
        }

        if (newPresentBoxManager.presentTimeNow < 0.0f && clickCount < 15)
        {
            clickCount = 0;
            StartCoroutine(newPresentBoxManager.NotGetPresent());
        }
    }

    void Click()
    {
        if (newPresentBoxManager.presentTimeNow < 5.0f && newPresentBoxManager.presentTimeNow > 0.0f)
        {
            ++clickCount;
            mousePos = Input.mousePosition;
            mousePos.z = 3f;
            Instantiate(ps, Camera.main.ScreenToWorldPoint(mousePos), Quaternion.identity);

            ps.Play();

            SfxPlayClickPinata.PlaySFXSoundForce();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }
}
