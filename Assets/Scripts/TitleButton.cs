using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    [Header("Canvas / UI")]
    //[SerializeField] GameObject canvas;
    [SerializeField] GameObject pnlMainMenu;
    [SerializeField] GameObject pnlHowToPlay;
    [SerializeField] Button btnStart;
    [SerializeField] Button btnHowToPlay;
    [SerializeField] Button btnBack;
    [SerializeField] Text txtTitle;

    [SerializeField] GameObject cmrMainCamara;
    public float cameraRotateSpeed = 3.0f;
    bool isRotateX = false;

    // Start is called before the first frame update
    void Start()
    {
        // óVÇ—ï˚ÇÕç≈èâfalse
        //pnlHowToPlay.SetActive(false);
        //boardHowToPlay.SetActive(false);
    }

    //public void PushGameStart()
    //{
    //    //SceneManager.LoadScene("PlayScene");
    //}

    public void PushStart()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        btnStart.gameObject.SetActive(false);
        //cmrMainCamara.transform.rotation = Quaternion.Euler(-30, 0, 0);
        isRotateX = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotateX)
        {
            cameraRotateSpeed-= 0.3f;
            cmrMainCamara.transform.rotation = Quaternion.Euler(cameraRotateSpeed, 0, 0);
            if(cameraRotateSpeed < -120)
            {
                isRotateX = false;
            }
        }
    }
}
