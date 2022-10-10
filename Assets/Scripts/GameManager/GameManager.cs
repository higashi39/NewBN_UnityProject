using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //ゲーム状態
    public enum GAME_STATUS
    {
        GAME_HOWTOPLAY,
        GAME_START, //もしアニメーション（321がやるなら最初にGAME_START）
        GAME_PAUSED,
        GAME_NORMAL,
        GAME_PRESENT_APPEAR,
        GAME_ENDED,
    }

    [field: Header("Game Settings")]
    //1っ回ゲームの時間
    [field: SerializeField] float GameTimeMax { set; get; } 

    [field: Header("GameA Animation Settings")]
    //Presentが出たら CanvasPresentAppearを出るまでの待つ時間
    //その時はプレゼントがアニメーションがある(上へ行くと回転する)
    //アニメーションの値がを設定するのはprefab/present/PresentBoxDisplay
    [field: SerializeField] float AnimationPresentAppearWaitTime { set; get; } = 1.5f;


    [field: Header("Game Properties")]
    //今ゲームの状態
    [field: SerializeField] public GAME_STATUS GameStatus { private set; get; }
    //今のゲーム時間
    [field: SerializeField] float GameTime { set; get; }
    //プレイヤーの最初座標（MapGeneratorで設定）
    //ゲーム終了したらプレイヤーはこの座標に戻る
    [field: SerializeField] public Vector3 PlayerFirstPos { set; get; }
    //もらったプレゼントが数
    [field: SerializeField] public int PresentGetNum { set; get; } = 0;

    [field: Header("Canvas / UI In Game")]
    [field: SerializeField] GameObject canvas;
    [field: SerializeField] GameObject pnlPause;
    [field: SerializeField] Text txtTime;
    [field: SerializeField] Text txtPause;
    [field: SerializeField] GameObject pnlStart;
    [field: SerializeField] Text txtStart;
    [field: SerializeField] GameObject pnlEnd;
    [field: SerializeField] Text txtEndTitle;
    [field: SerializeField] GameObject pnlPresentGet;
    [field: SerializeField] Text txtPresentGetNumEndGame;
    [field: SerializeField] Button btnEndMainMenu;
    [field: SerializeField] Button btnEndRestart;

    //-------------------------------
    // add higashi
    //-------------------------------
    [field: Header("Canvas / UI Tutorial")]
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

    [field: Header("Script References")]
    PlayerMain playerMain;
    PlayerUI playerUI;
    NewPresentBoxManager newPresentBoxManager;

    private void Awake()
    {
        playerMain = FindObjectOfType<PlayerMain>();
        playerUI = FindObjectOfType<PlayerUI>();
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // ポーズは無しにするので、これを遊び方に使用させてもらいます
        pnlPause.SetActive(true);

        pnlStart.SetActive(false);
        pnlEnd.SetActive(false);
        txtEndTitle.gameObject.SetActive(false);
        btnEndMainMenu.gameObject.SetActive(false);
        btnEndRestart.gameObject.SetActive(false);
        pnlPresentGet.SetActive(false);

        //ゲーム時間設定
        GameTime = GameTimeMax;

        //プレイヤーの入力をfalseにする
        playerMain.SetPlayerInputDisabled();

        //時間のUIを合わせた書式にする
        FormatTimeText();

        //-------------------------------
        // add higashi
        //-------------------------------
        HowToPlay();
        imgBoard.SetActive(true);
        howToPlay.gameObject.SetActive(true);
        GameStatus = GAME_STATUS.GAME_HOWTOPLAY;
        txtNextPage.text = "次へ";
        nextPageClickNum = 0;
        txtBackPage.text = "戻る";
        btnBackPage.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        switch (GameStatus)
        {
            case GAME_STATUS.GAME_NORMAL:
                {
                    GameTime -= Time.deltaTime;
                    if (GameTime <= 0.0f)
                    {
                        //--------------------
                        //ゲーム終了
                        //--------------------
                        GameTime = 0.0f;
                        GameStatus = GAME_STATUS.GAME_ENDED;
                        playerMain.SetPlayerInputDisabled();
                        StartCoroutine(EndGameAnimation());
                        playerMain.transform.position = PlayerFirstPos;
                    }
                    FormatTimeText();
                }
                break;
        }

    }

    //GameStatusはGAME_STATUS.GAME_PRESENT_APPEARにする
    //プレイヤーの入力をoffにする
    //UIを隠して、プレゼント箱現れたアニメションを呼ぶ
    public void SetGameStatusPresentAppear()
    {
        GameStatus = GAME_STATUS.GAME_PRESENT_APPEAR;
        playerMain.SetPlayerInputDisabled();
        SetEnableAllUI(false);
        StartCoroutine(BoxAppearAnimation());
    }

    #region Animation
    //ゲームスタートアニメション
    IEnumerator StartGameAnimation()
    {
        howToPlay.SetActive(false);
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
        playerMain.SetPlayerInputDisabled(false);
        GameStatus = GAME_STATUS.GAME_NORMAL;
    }
    //ゲーム終了アニメション
    IEnumerator EndGameAnimation()
    {
        pnlEnd.SetActive(true);
        txtEndTitle.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        txtPresentGetNumEndGame.text = PresentGetNum.ToString();
        pnlPresentGet.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        btnEndMainMenu.gameObject.SetActive(true);
        btnEndRestart.gameObject.SetActive(true);
    }
    //プレゼント箱が現れたアニメション
    IEnumerator BoxAppearAnimation()
    {
        yield return new WaitForSeconds(AnimationPresentAppearWaitTime);
        PresentBoxDisplay presentBoxDisplay = FindObjectOfType<PresentBoxDisplay>();
        presentBoxDisplay.SetAnimationWait();

        newPresentBoxManager.pnlPresentScene.SetActive(true);
        newPresentBoxManager.presentSceneSetActive = true;
    }
    #endregion

    #region Get Present From Box
    //プレゼントをもらうかどうか（PresentSceneから判断）
    public void GetPresentFromBox(bool isGet = true)
    {
        if (isGet)
        {
            ++PresentGetNum;
            playerUI.UpdateCollectedPresentUI();
        }
        StartCoroutine(BackGamePlay());
    }
    #endregion

    #region UI Handler
    //　全ActiveUIを見せるかどうか
    public void SetEnableAllUI(bool isEnable = true)
    {
        canvas.SetActive(isEnable);
    }
    //  gameTimeの書式
    void FormatTimeText()
    {
        // .ToString("00") -> 00 80 60
        string timeFormat = GameTime.ToString("00");
        txtTime.text = timeFormat;
    }
    #endregion

    #region Button Function
    //  ポーズボタン押す関数
    public void PressBtnPause()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        //ポーズする
        if (GameStatus == GAME_STATUS.GAME_NORMAL)
        {
            Time.timeScale = 0;
            pnlPause.SetActive(true);
            GameStatus = GAME_STATUS.GAME_PAUSED;
            txtPause.text = "Resume";
            return;
        }

        //ゲーム続き
        if (GameStatus == GAME_STATUS.GAME_PAUSED)
        {
            GameStatus = GAME_STATUS.GAME_NORMAL;
            pnlPause.SetActive(false);
            Time.timeScale = 1;
            txtPause.text = "Pause";
            return;
        }
    }

    //メインメニューに戻る
    //pnlEndのボタンに設定する
    public void PressBtnMainMenu()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        SceneManager.LoadScene("MainMenu");
    }

    //ゲームをリセットする
    //pnlEndのボタンに設定する
    public void PressBtnRestart()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        SceneManager.LoadScene("PlayScene");
    }
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
                    "\n一気に草を刈ることができるから、上手く使って、たくさんのプレゼントを見つけよう！";
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
        SFXButton.Instance.PlayButtonPressSFX();
        nextPageClickNum++;
        HowToPlay();
    }

    public void ClickBackPage()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        nextPageClickNum--;
        HowToPlay();
    }

    public IEnumerator BackGamePlay()
    {
        yield return new WaitForSeconds(3.0f);

        newPresentBoxManager.decoration.SetActive(false);
        newPresentBoxManager.pnlPresentTime.SetActive(false);
        newPresentBoxManager.pnlPresentScene.SetActive(false);

        PresentBoxDisplay presentBoxDisplay = FindObjectOfType<PresentBoxDisplay>();
        presentBoxDisplay.SetAnimationDisappear();
        playerMain.SetPlayerInputDisabled(false);
        SetEnableAllUI(true);
        GameStatus = GAME_STATUS.GAME_NORMAL;

        Debug.Log("変わるよ");
    }



}
