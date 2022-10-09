using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GAME_STATUS
    {
        GAME_HOWTOPLAY,
        GAME_START, //もしアニメーション（321がやるなら最初にGAME_START）
        //GAME_PAUSED,
        GAME_NORMAL,
        GAME_PRESENT_APPEAR,
        GAME_PRESENT_OPEN,
        GAME_ENDED,
    }

    [field: SerializeField] public GAME_STATUS GameStatus { private set; get; }

    [Header("Canvas / UI")]
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject pnlPause;
    [SerializeField] Text txtTime;
    //[SerializeField] Text txtPresentLeft;
    [SerializeField] Text txtPause;
    [SerializeField] GameObject pnlStart;
    [SerializeField] Text txtStart;
    [SerializeField] GameObject pnlEnd;
    [SerializeField] Text txtEndTitle;
    //[SerializeField] Text txtEndScore;
    [SerializeField] Button btnEndMainMenu;
    [SerializeField] Button btnEndRestart;



    [Header("Game Settings")]
    [SerializeField] float gameTimeMax; //1っ回ゲームの時間
    [SerializeField] float gameTime;    //今の時間

    [Header("Player References")]
    PlayerMain playerMain;
    Vector3 playerFirstPos;


    //-------------------------------
    // add higashi
    //-------------------------------
    [SerializeField] GameObject imgBoard;
    [SerializeField] GameObject howToPlay;
    [Header("How To Play")]
    [SerializeField] Text txtExplainSubTitle;
    [SerializeField] Text txtExplainDetail;
    [SerializeField] Button btnNextPage;
    [SerializeField] Button btnBackPage;
    [SerializeField] Text txtNextPage;
    [SerializeField] Text txtBackPage;
    [SerializeField] int nextPageClickNum;

    //[Header("About Present")]

    [Header("Another References")]
    // プレゼントのスクリプト
    PresentBoxManager presentBoxManager;        // 以前のPresentManager
    NewPresentBoxManager newPresentBoxManager;

    // Start is called before the first frame update
    void Start()
    {
        //  ポーズパネルはfalseにする
        //-------------------------------
        // edit higashi
        //-------------------------------
        // ポーズは無しにするので、これを遊び方に使用させてもらいます
        pnlPause.SetActive(true);
        //  
        pnlStart.SetActive(false);
        pnlEnd.SetActive(false);
        txtEndTitle.gameObject.SetActive(false);
        //txtEndScore.gameObject.SetActive(false);
        btnEndMainMenu.gameObject.SetActive(false);
        btnEndRestart.gameObject.SetActive(false);
        //  ゲーム時間設定
        gameTime = gameTimeMax;
        //  プレイヤー参照
        playerMain = FindObjectOfType<PlayerMain>();
        playerFirstPos = playerMain.transform.position;

        //StartCoroutine(StartGameAnimation());

        //-------------------------------
        // add higashi
        //-------------------------------
        imgBoard.SetActive(true);
        howToPlay.gameObject.SetActive(true);
        //btnToGameStart.gameObject.SetActive(true);
        //PressBtnToGameStart();
        GameStatus = GAME_STATUS.GAME_HOWTOPLAY;
        txtNextPage.text = "次へ";
        nextPageClickNum = 0;
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        txtBackPage.text = "戻る";
        btnBackPage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameStatus)
        {
            case GAME_STATUS.GAME_HOWTOPLAY:
                {
                    HowToPlay();
                }
                break;

            case GAME_STATUS.GAME_NORMAL:
                {

                    gameTime -= Time.deltaTime;
                    if (gameTime <= 0.0f)
                    {
                        //--------------------
                        //ゲーム終了
                        //--------------------
                        gameTime = 0.0f;
                        ChangeGameStatus(GAME_STATUS.GAME_ENDED);
                        playerMain.SetPlayerInputEnable(false);
                        StartCoroutine(EndGameAnimation());
                        playerMain.transform.position = playerFirstPos;
                    }
                    FormatTimeText();
                }
                break;
        }

    }

    //  ゲームステータスを変更する
    public void ChangeGameStatus(GAME_STATUS status)
    {
        GameStatus = status;
    }

    public void SetGameStatusPresentAppear()
    {
        GameStatus = GAME_STATUS.GAME_PRESENT_APPEAR;
        ChangeGameStatus(GameManager.GAME_STATUS.GAME_PRESENT_APPEAR);
        playerMain.SetPlayerInputEnable(false);
        SetEnableAllUI(false);

    }


    #region UI
    //　全ActiveUIを見せるかどうか
    public void SetEnableAllUI(bool isEnable = true)
    {
        canvas.SetActive(isEnable);
    }
    //  gameTimeの書式
    void FormatTimeText()
    {
        // .ToString("00") -> 00 80 60
        string timeFormat = gameTime.ToString("00");
        txtTime.text = timeFormat;
    }
    //  残りプレゼント数のUIを更新する
    public void UpdateUIPresentLeft(int presentLeft)
    {
        //txtPresentLeft.text = "残りプレゼント数：" + presentLeft;
    }
    //  ポーズボタン押す関数

    IEnumerator StartGameAnimation()
    {
        FormatTimeText();
        playerMain.SetPlayerInputEnable(false);
        howToPlay.SetActive(false);
        imgBoard.SetActive(false);
        pnlPause.SetActive(false);
        GameStatus = GAME_STATUS.GAME_START;
        pnlStart.SetActive(true);
        txtStart.text = "3";
        yield return new WaitForSeconds(1.0f);
        txtStart.text = "2";
        yield return new WaitForSeconds(1.0f);
        txtStart.text = "1";
        yield return new WaitForSeconds(1.0f);
        txtStart.text = "GO";
        yield return new WaitForSeconds(1.0f);
        pnlStart.SetActive(false);
        playerMain.SetPlayerInputEnable();
        GameStatus = GAME_STATUS.GAME_NORMAL;
    }

    IEnumerator EndGameAnimation()
    {
        pnlEnd.SetActive(true);
        txtEndTitle.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        //txtEndScore.text = "スコア：" + playerMain.GetPlayerScore().ToString();
        //txtEndScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        btnEndMainMenu.gameObject.SetActive(true);
        btnEndRestart.gameObject.SetActive(true);
    }


    #region Button Function
    //public void PressBtnPause()
    //{
    //    //ポーズする
    //    if (GameStatus == GAME_STATUS.GAME_NORMAL)
    //    {
    //        Time.timeScale = 0;
    //        pnlPause.SetActive(true);
    //        ChangeGameStatus(GAME_STATUS.GAME_PAUSED);
    //        txtPause.text = "Resume";
    //        return;
    //    }

    //    //ゲーム続き
    //    if (GameStatus == GAME_STATUS.GAME_PAUSED)
    //    {
    //        ChangeGameStatus(GAME_STATUS.GAME_NORMAL);
    //        pnlPause.SetActive(false);
    //        Time.timeScale = 1;
    //        txtPause.text = "Pause";
    //        return;
    //    }
    //}

    public void PressBtnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PressBtnRestart()
    {
        SceneManager.LoadScene("PlayScene");
    }
    #endregion

    #endregion

    void HowToPlay()
    {
        switch (nextPageClickNum)
        {
            case 0:
                btnBackPage.gameObject.SetActive(false);
                btnNextPage.gameObject.SetActive(true);
                txtExplainSubTitle.text = "1. マウスカーソルで移動";
                txtExplainDetail.text = "ギャー君はマウスカーソルのある方向に移動するよ\n上手く誘導してあげよう";
                break;

            case 1:
                btnBackPage.gameObject.SetActive(true);
                txtExplainSubTitle.text = "2. 左クリックホールドで道を開く";
                txtExplainDetail.text = "草のあるところで左クリックを一定時間ホールドすると、\n草を刈れるよ" +
                    "\n草を刈ると確率でプレゼントが！\nたくさんのプレゼントを集めよう";
                break;
            case 2:
                txtExplainSubTitle.text = "3. プレゼントを開けよう";
                txtExplainDetail.text = "プレゼントを見つけたら開けてみよう\nプレゼントの開け方はプレゼントによって様々" +
                    "\nクリックしたり、引っ張ったり...\n制限時間内に素早く開けてみよう";
                break;
            case 3:
                txtExplainSubTitle.text = "＜ゲージの説明＞ 草刈り機の充電";
                txtExplainDetail.text = "草を刈っていると、青色の草刈り機ゲージが減るよ" +
                    "\nゲージが空になると、草を刈れなくなってしまう！" +
                    "マップの上側にある家の前まで行って、充電しよう！";
                btnNextPage.transform.localPosition = new Vector3(230, -205, 0);
                txtNextPage.text = "次へ";
                break;
            case 4:
                txtExplainSubTitle.text = "＜ゲージの説明＞ ギャー君のスキル";
                txtExplainDetail.text = "草を刈っていると黄色のスキルゲージがたまるよ" +
                    "\nゲージがいっぱいになった状態で右クリックを押すと、\nスキルが発動されて、ギャー君のスピードがあがるよ！" +
                    "\n上手く使って、たくさんのプレゼントを見つけよう！";
                btnNextPage.gameObject.SetActive(true);
                btnNextPage.transform.localPosition = new Vector3(0, -205, 0);
                txtNextPage.text = "スタート";
                break;
            case 5:
                StartCoroutine(StartGameAnimation());
                break;
        }
    }

    public void ClickNextPage()
    {
        nextPageClickNum++;
    }

    public void ClickBackPage()
    {
        nextPageClickNum--;
    }

    public IEnumerator BackGamePlay()
    {
        yield return new WaitForSeconds(3.0f);
        newPresentBoxManager.decoration.SetActive(false);
        newPresentBoxManager.pnlPresentTime.SetActive(false);
        newPresentBoxManager.pnlPresentScene.SetActive(false);
        ChangeGameStatus(GAME_STATUS.GAME_NORMAL);
        Time.timeScale = 1;
        playerMain.SetPlayerInputEnable(true);
        SetEnableAllUI(true);
        Debug.Log("変わるよ");
    }
}
