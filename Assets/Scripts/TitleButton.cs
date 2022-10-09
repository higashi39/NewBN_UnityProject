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

    [SerializeField] GameObject boardHowToPlay;

    // Start is called before the first frame update
    void Start()
    {
        // óVÇ—ï˚ÇÕç≈èâfalse
        //pnlHowToPlay.SetActive(false);
        //boardHowToPlay.SetActive(false);
    }

    public void PushGameStart()
    {
        SceneManager.LoadScene("PlayScene");
    }
    
    public void PushHowToPlay()
    {
        pnlMainMenu.SetActive(false);
        pnlHowToPlay.SetActive(true);
        boardHowToPlay.SetActive(true);
    }

    public void PushBack()
    {
        pnlHowToPlay.SetActive(false);
        boardHowToPlay.SetActive(false);
        pnlMainMenu.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
