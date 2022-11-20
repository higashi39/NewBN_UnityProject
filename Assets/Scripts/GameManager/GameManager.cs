using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

    [field: Header("Game Animation Settings")]
    //Presentが出たら CanvasPresentAppearを出るまでの待つ時間
    //その時はプレゼントがアニメーションがある(上へ行くと回転する)
    //アニメーションの値がを設定するのはprefab/present/PresentBoxDisplay
    [field: SerializeField] float AnimationPresentAppearWaitTime { set; get; } = 1.5f;


    [field: Header("Game Properties")]
    //今ゲームの状態
    [field: SerializeField] public GAME_STATUS GameStatus { private set; get; }
    //今のゲーム時間
    [field: SerializeField] float GameTime { set; get; }
    [field: SerializeField] int GameTimeBefore { set; get; }
    //プレイヤーの最初座標（MapGeneratorで設定）
    //ゲーム終了したらプレイヤーはこの座標に戻る
    [field: SerializeField] public Vector3 PlayerFirstPos { set; get; }
    //もらったプレゼントが数
    [field: SerializeField] public int PresentGetNum { set; get; } = 0;

    [field: Header("Canvas / UI In Game")]
    [field: SerializeField] GameObject pnlBlock;
    [field: SerializeField] Text txtTimeLeft;
    [field: SerializeField] Text txtTimeUp;
    [field: SerializeField] Text txtTime;
    [field: SerializeField] Text txtPause;
    [field: SerializeField] GameObject pnlStart;
    [field: SerializeField] Text txtStart;
    [field: SerializeField] GameObject pnlEnd;
    [field: SerializeField] GameObject pnlPresentGet;
    [field: SerializeField] Image imgPresentGet;
    [field: SerializeField] Text txtPresentGet;
    [field: SerializeField] Text txtResultGameEnd;
    [field: SerializeField] Image imgPresentGetEndGame;
    [field: SerializeField] Text txtPresentGetNumEndGame;
    [field: SerializeField] Button btnEndMainMenu;
    [field: SerializeField] Button btnEndRestart;

    [field: SerializeField] Image imgEnergyMask;
    [field: SerializeField] RawImage imgEnergyFill;
    [field: SerializeField] Image imgEnergyFront;

    [field: SerializeField] Image imgSkillMask;
    [field: SerializeField] RawImage imgSkillFill;
    [field: SerializeField] Image imgSkillFront;

    [field: Header("UI Main Animation Time Settings")]
    [field: SerializeField] float timeAnimationTimeFadeIn = 1.0f;
    [field: SerializeField] float timeAnimationTimeFadeOut = 1.0f;

    [field: Header("Game Time Animation Settings")]
    [field: SerializeField] float gameTimeLeftStartAnimation = 5.0f;
    [field: SerializeField] float gameTimeLeftAnimationTime = 0.9f;

    [field: Header("UI Time Up Animation Settings")]
    [field: SerializeField] float gameTimeUpAnimationTime = 1.5f;
    [field: SerializeField] float gameTimeUpAnimationTimeAfterDelay = 2.0f;

    [field: Header("UI End Game Animation Settings")]
    [field: SerializeField] float endGameEndPanelAnimationTime = 1.0f;
    [field: SerializeField] float endGameNextAnimationTime = 1.0f;
    [field: SerializeField] float endGameLastButtonDelay = 1.5f;
    [field: SerializeField] float endGameLastButtonPress = 1.0f;


    //-------------------------------
    // add higashi
    //-------------------------------
    [field: Header("Canvas / UI Tutorial")]
    [SerializeField] GameObject imgBoard;
    [SerializeField] GameObject howToPlay;

    [Header("How To Play")]
    [SerializeField] Text txtExplainTitle;
    [SerializeField] Text txtExplainSubTitle;
    [SerializeField] Text txtExplainDetail;
    [SerializeField] Button btnNextPage;
    [SerializeField] Button btnBackPage;
    [SerializeField] Button btnStartGame;
    [SerializeField] int nextPageClickNum = 0;
    List<HowToPlayTextHelper> TextSubTitleList;

    [field: Header("Script References")]
    PlayerMain playerMain;
    PlayerUI playerUI;
    NewPresentBoxManager newPresentBoxManager;

    [field: Header("How To Play Text Animation")]
    Coroutine animationHowToPlayChangePageCoroutine;
    [field: SerializeField] float HowToPlayStartDelayTime { set; get; } = 1.5f;
    [field: SerializeField] float HowToPlayGoingInTime { set; get; } = 2.5f;
    [field: SerializeField] float HowToPlayFadeTime { set; get; } = 1.0f;
    [field: SerializeField] float HowToPlayGoingOutTime { set; get; } = 2.5f;
    [field: SerializeField] float HowToPlayAnimationAfterWaitTime { set; get; } = 0.5f;

    [field: Header("SFX")]
    [field: SerializeField] SFXPlay SfxPlaySecondLeft { set; get; }
    [field: SerializeField] SFXPlay SfxPlayGameEnd { set; get; }

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
        SetGameStartUI();

        //ゲーム時間設定
        GameTime = GameTimeMax;

        //プレイヤーの入力をfalseにする
        playerMain.SetPlayerInputDisabled();

        //時間のUIを合わせた書式にする
        FormatTimeText();

        //-------------------------------
        // add higashi
        //-------------------------------
        PrepareHowToPlayText();
        nextPageClickNum = 0;
        CheckHowToPlayButtonActive();
        GameStatus = GAME_STATUS.GAME_HOWTOPLAY;

        //End Scene Transition
        if (SceneTransitionAnimation.instance != null)
        {
            SceneTransitionAnimation.instance.ShowScene();
        }
        else
        {
            HowToPlayStartDelayTime = 0.0f;
        }

        GameTutorialIndicator.instance.IsShowTutorialThisTime = GameTutorialIndicator.instance.IsShowTutorial;
        if (GameTutorialIndicator.instance.IsShowTutorialThisTime)
        {
            GameTutorialIndicator.instance.IsShowTutorial = false;
            StartCoroutine(StartPnlHowToPlayAnimation());
        }
        else
        {
            pnlBlock.SetActive(false);
            StartCoroutine(StartGameAnimation());
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameStatus)
        {
            case GAME_STATUS.GAME_NORMAL:
                {
                    GameTime -= Time.deltaTime;
                    FormatTimeText();
                    if (GameTime <= 0.0f)
                    {
                        //--------------------
                        //ゲーム終了
                        //--------------------
                        GameTime = 0.0f;
                        GameStatus = GAME_STATUS.GAME_ENDED;
                        playerMain.SetPlayerInputDisabled();
                        StartCoroutine(EndGameAnimation());
                        //playerMain.transform.position = PlayerFirstPos;
                    }
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
        if (!GameTutorialIndicator.instance.IsShowTutorialThisTime)
        {
            yield return new WaitForSeconds(HowToPlayStartDelayTime);
        }

        SetEnableAllUI();

        int counterTime = 4;    // 3 2 1 GO!
        const float ONE_SECOND = 1.0f;
        const float SMOOTH_TIME = 0.2f;
        GameStatus = GAME_STATUS.GAME_START;

        Vector3 scaleStart = Vector3.zero;
        Vector3 scaleTarget = new Vector3(1.4f, 1.4f, 1.4f);
        pnlStart.SetActive(true);
        do
        {
            txtStart.transform.localScale = scaleStart;
            txtStart.transform.DOScale(scaleTarget, ONE_SECOND).SetEase(Ease.OutBack);

            string nextText = (counterTime - 1).ToString(); //3 2 1
            if (counterTime <= 1)
            {
                nextText = "GO";
            }
            txtStart.text = nextText;
            --counterTime;
            yield return new WaitForSeconds(ONE_SECOND + SMOOTH_TIME);
        }
        while (counterTime > 0);
        yield return new WaitForSeconds(SMOOTH_TIME);
        pnlStart.SetActive(false);
        playerMain.SetPlayerInputDisabled(false);

        GameStatus = GAME_STATUS.GAME_NORMAL;
    }
    //ゲーム終了アニメション
    IEnumerator EndGameAnimation()
    {
        SfxPlaySecondLeft.StopSFXSound();
        SfxPlayGameEnd.PlaySFXSoundFromStartSecond(2.0f);
        SetEnableAllUI(false);

        {
            //Init
            pnlEnd.transform.localScale = Vector3.zero;
            txtResultGameEnd.transform.localScale = Vector3.zero;
            imgPresentGetEndGame.transform.localScale = Vector3.zero;
            txtPresentGetNumEndGame.transform.localScale = Vector3.zero;
            txtPresentGetNumEndGame.text = PresentGetNum.ToString();
            btnEndMainMenu.interactable = false;
            btnEndRestart.interactable = false;
            btnEndMainMenu.transform.localScale = Vector3.zero;
            btnEndRestart.transform.localScale = Vector3.zero;
        }

        {
            txtTimeUp.gameObject.SetActive(true);
            Vector3 scaleTarget = Vector3.one;
            txtTimeUp.transform.DOScale(scaleTarget, gameTimeUpAnimationTime).SetEase(Ease.OutBounce);
        }
        yield return new WaitForSeconds(gameTimeUpAnimationTime);


        playerMain.transform.position = PlayerFirstPos;
        yield return new WaitForSeconds(gameTimeUpAnimationTimeAfterDelay);

        pnlEnd.SetActive(true);
        pnlEnd.transform.DOScale(Vector3.one, endGameEndPanelAnimationTime).SetEase(Ease.InSine);
        txtTimeUp.transform.DOScale(Vector3.zero, endGameEndPanelAnimationTime).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(endGameEndPanelAnimationTime);

        txtResultGameEnd.transform.DOScale(Vector3.one, endGameNextAnimationTime).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(endGameNextAnimationTime);
        imgPresentGetEndGame.transform.DOScale(Vector3.one, endGameNextAnimationTime).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(endGameNextAnimationTime);
        txtPresentGetNumEndGame.transform.DOScale(Vector3.one, endGameNextAnimationTime).SetEase(Ease.OutBack);
        //yield return new WaitForSeconds(endGameNextAnimationTime);

        yield return new WaitForSeconds(endGameLastButtonDelay);
        btnEndMainMenu.transform.DOScale(Vector3.one, endGameNextAnimationTime).SetEase(Ease.OutBack);
        btnEndRestart.transform.DOScale(Vector3.one, endGameNextAnimationTime).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(endGameNextAnimationTime);
        btnEndMainMenu.interactable = true;
        btnEndRestart.interactable = true;

        {
            var scriptButtonAnimation = btnEndMainMenu.GetComponent<ButtonAnimation>();
            scriptButtonAnimation.enabled = true;
            scriptButtonAnimation.IsCanAnimation = true;
        }

        {
            var scriptButtonAnimation = btnEndRestart.GetComponent<ButtonAnimation>();
            scriptButtonAnimation.enabled = true;
            scriptButtonAnimation.IsCanAnimation = true;
        }

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
    void SetGameStartUI()
    {
        const float INSTAN_FADE_TIME = 0.0f;

        txtPresentGet.text = PresentGetNum.ToString();

        pnlBlock.SetActive(true);
        //-----------------------------------------
        txtTime.DOFade(0, INSTAN_FADE_TIME);
        txtTimeLeft.DOFade(0, INSTAN_FADE_TIME);
        //-----------------------------------------
        imgEnergyFill.DOFade(0, INSTAN_FADE_TIME);
        imgEnergyFront.DOFade(0, INSTAN_FADE_TIME);
        imgEnergyMask.DOFade(0, INSTAN_FADE_TIME);
        //-----------------------------------------
        imgSkillFill.DOFade(0, INSTAN_FADE_TIME);
        imgSkillFront.DOFade(0, INSTAN_FADE_TIME);
        imgSkillMask.DOFade(0, INSTAN_FADE_TIME);
        //-----------------------------------------
        imgPresentGet.DOFade(0, INSTAN_FADE_TIME);
        txtPresentGet.DOFade(0, INSTAN_FADE_TIME);
    }

    //　全ActiveUIを見せるかどうか
    public void SetEnableAllUI(bool isEnable = true)
    {
        StartCoroutine(EnableAllUIAnimation(isEnable));
    }

    IEnumerator EnableAllUIAnimation(bool isEnable)
    {
        float fadeTarget = 1.0f;
        float fadeTime = timeAnimationTimeFadeIn;
        if (!isEnable)
        {
            fadeTarget = 0.0f;
            fadeTime = timeAnimationTimeFadeOut;
        }

        txtTime.DOFade(fadeTarget, fadeTime);
        txtTimeLeft.DOFade(fadeTarget, fadeTime);
        //-----------------------------------------
        imgEnergyFill.DOFade(fadeTarget, fadeTime);
        imgEnergyFront.DOFade(fadeTarget, fadeTime);
        imgEnergyMask.DOFade(fadeTarget, fadeTime);
        //-----------------------------------------
        imgSkillFill.DOFade(fadeTarget, fadeTime);
        imgSkillFront.DOFade(fadeTarget, fadeTime);
        imgSkillMask.DOFade(fadeTarget, fadeTime);
        //-----------------------------------------
        imgPresentGet.DOFade(fadeTarget, fadeTime);
        txtPresentGet.DOFade(fadeTarget, fadeTime);

        yield return null;
    }

    //  gameTimeの書式
    void FormatTimeText()
    {
        // .ToString("00") -> 00 80 60
        //string timeFormat = GameTime.ToString("00");
        //var gameTime = int.Parse(timeFormat);
        var gameTime = (int)GameTime;
        txtTime.text = gameTime.ToString();

        if (gameTime <= gameTimeLeftStartAnimation)
        {
            SfxPlaySecondLeft.PlaySFXSound();
            if (gameTime != GameTimeBefore)
            {
                //StartCoroutine(ReduceGameTimeAnimation());
                {
                    //Init
                    Vector3 scaleStart = new Vector3(1.5f, 1.5f, 1.5f);
                    txtTime.transform.localScale = scaleStart;
                }

                txtTime.transform.DOScale(Vector3.one, gameTimeLeftAnimationTime).SetEase(Ease.OutBack);
            }
        }
        GameTimeBefore = gameTime;
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
            pnlBlock.SetActive(true);
            GameStatus = GAME_STATUS.GAME_PAUSED;
            txtPause.text = "Resume";
            return;
        }

        //ゲーム続き
        if (GameStatus == GAME_STATUS.GAME_PAUSED)
        {
            GameStatus = GAME_STATUS.GAME_NORMAL;
            pnlBlock.SetActive(false);
            Time.timeScale = 1;
            txtPause.text = "Pause";
            return;
        }
    }

    //メインメニューに戻る
    //pnlEndのボタンに設定する
    public void PressBtnMainMenu()
    {
        GameTutorialIndicator.instance.IsShowTutorial = true;
        SFXButton.Instance.PlayButtonPressSFX();
        EndGameButtonPressAnimation();
        SceneTransitionAnimation.instance.ChangeScene("MainMenu");
    }

    //ゲームをリセットする
    //pnlEndのボタンに設定する
    public void PressBtnRestart()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        EndGameButtonPressAnimation();
        SceneTransitionAnimation.instance.ChangeScene("PlayScene");
    }
    #endregion

    void EndGameButtonPressAnimation()
    {
        btnEndMainMenu.interactable = false;
        btnEndRestart.interactable = false;

        btnEndMainMenu.GetComponent<ButtonAnimation>().enabled = false;
        btnEndRestart.GetComponent<ButtonAnimation>().enabled = false;

        txtResultGameEnd.DOFade(0, endGameLastButtonPress).SetEase(Ease.OutSine);
        imgPresentGetEndGame.DOFade(0, endGameLastButtonPress).SetEase(Ease.OutSine);
        txtPresentGetNumEndGame.DOFade(0, endGameLastButtonPress).SetEase(Ease.OutSine);
        btnEndMainMenu.transform.DOScale(Vector3.zero, endGameLastButtonPress).SetEase(Ease.InBack);
        btnEndRestart.transform.DOScale(Vector3.zero, endGameLastButtonPress).SetEase(Ease.InBack);
    }

    #region How To Play Animation
    IEnumerator StartPnlHowToPlayAnimation()
    {
        yield return new WaitForSeconds(HowToPlayStartDelayTime);

        RectTransform rectHowToPlay = howToPlay.GetComponent<RectTransform>();
        var mySizeY = rectHowToPlay.sizeDelta.y;

        //Init
        {
            Vector2 targetPos = Vector2.zero;
            targetPos.y = mySizeY;

            rectHowToPlay.anchoredPosition = targetPos;

            txtExplainTitle.DOFade(0, 0.0f);
            txtExplainSubTitle.DOFade(0, 0.0f);
            txtExplainDetail.DOFade(0, 0.0f);

            btnNextPage.gameObject.SetActive(false);

            howToPlay.gameObject.SetActive(true);
        }

        //going in(from top)
        {
            rectHowToPlay.DOAnchorPosY(0, HowToPlayGoingInTime).SetEase(Ease.OutBack);
        }
        yield return new WaitForSeconds(HowToPlayGoingInTime + HowToPlayAnimationAfterWaitTime);
        btnNextPage.gameObject.SetActive(true);

        txtExplainTitle.DOFade(1, HowToPlayFadeTime);
        txtExplainSubTitle.DOFade(1, HowToPlayFadeTime);
        txtExplainDetail.DOFade(1, HowToPlayFadeTime);
    }

    IEnumerator ChangeHowToPlayPageAnimation()
    {
        //fade out
        {
            txtExplainSubTitle.DOFade(0, HowToPlayFadeTime);
            txtExplainDetail.DOFade(0, HowToPlayFadeTime);
        }
        yield return new WaitForSeconds(HowToPlayFadeTime);

        //Change text
        {
            var tmpText = TextSubTitleList[nextPageClickNum];
            txtExplainSubTitle.text = tmpText.ExplainSubTitleText;
            txtExplainDetail.text = tmpText.ExplainDetailText;
        }

        //fade in
        {
            txtExplainSubTitle.DOFade(1, HowToPlayFadeTime);
            txtExplainDetail.DOFade(1, HowToPlayFadeTime);
        }
        yield return new WaitForSeconds(HowToPlayFadeTime);
    }

    IEnumerator ClosePnlHowToPlayAnimation()
    {
        RectTransform rectHowToPlay = howToPlay.GetComponent<RectTransform>();
        var mySizeY = rectHowToPlay.sizeDelta.y;

        //going out(to top)
        {
            rectHowToPlay.DOAnchorPosY(mySizeY, HowToPlayGoingOutTime).SetEase(Ease.InBack);
        }
        yield return new WaitForSeconds(HowToPlayGoingOutTime + HowToPlayAnimationAfterWaitTime);

        howToPlay.gameObject.SetActive(false);
        pnlBlock.SetActive(false);
        StartCoroutine(StartGameAnimation());
    }

    void PrepareHowToPlayText()
    {
        TextSubTitleList = new List<HowToPlayTextHelper>();

        //1. マウスカーソルで移動
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "1. マウスカーソルで移動";
            tmp.ExplainDetailText = "ギャー君はマウスカーソルのある方向に移動するよ\n上手く誘導してあげよう";

            txtExplainSubTitle.text = tmp.ExplainSubTitleText;
            txtExplainDetail.text = tmp.ExplainDetailText;

            TextSubTitleList.Add(tmp);
        }

        //2.左クリックホールドで道を開く
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "2.左クリックホールドで道を開く";
            tmp.ExplainDetailText = "草のあるところで左クリックを一定時間ホールドすると\n草を刈れるよ！" +
                            "\n草を刈ると確率でプレゼントが手に入る！\nたくさんのプレゼントを集めよう";

            TextSubTitleList.Add(tmp);
        }

        //3. プレゼントを開けよう
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "3. プレゼントを開けよう";
            tmp.ExplainDetailText = "プレゼントを見つけたら開けてみよう！\nプレゼントの開け方はプレゼントによって違うよ" +
                            "\nクリックしたり、引っ張ったり...\n制限時間内に素早く開けてみよう！！！";

            TextSubTitleList.Add(tmp);
        }

        //＜ゲージの説明＞ 草刈り機の充電
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "＜ゲージの説明＞ 草刈り機の充電";
            tmp.ExplainDetailText = "草を刈っていると、青色の草刈り機ゲージが減るよ" +
                            "\nゲージが空になると、草を刈れなくなってしまう・・・\n" +
                            "マップの上側にある家の前か、庭にある充電器で\n充電しよう！";

            TextSubTitleList.Add(tmp);
        }

        //＜ゲージの説明＞ ギャー君のスキル
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "＜ゲージの説明＞ ギャー君のスキル";
            tmp.ExplainDetailText = "草を刈っていると黄色のスキルゲージがたまるよ" +
                            "\nゲージがいっぱいになった状態で右クリックを押すと、\nスキルが発動されて、ギャー君のスピードがあがるよ！" +
                            "\n一気に草を刈ることができるから、\n上手く使って、たくさんのプレゼントを見つけよう！";

            TextSubTitleList.Add(tmp);
        }

    }

    public void ClickNextPage()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        nextPageClickNum++;
        animationHowToPlayChangePageCoroutine = StartCoroutine(ChangeHowToPlayPageAnimation());
        CheckHowToPlayButtonActive();
    }

    public void ClickBackPage()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        nextPageClickNum--;
        animationHowToPlayChangePageCoroutine = StartCoroutine(ChangeHowToPlayPageAnimation());
        CheckHowToPlayButtonActive();
    }

    public void PressButtonStartGame()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        btnStartGame.gameObject.SetActive(false);
        btnBackPage.gameObject.SetActive(false);
        btnNextPage.gameObject.SetActive(false);

        StartCoroutine(ClosePnlHowToPlayAnimation());
    }

    void CheckHowToPlayButtonActive()
    {
        switch (nextPageClickNum)
        {
            case 0:
                btnStartGame.gameObject.SetActive(false);
                btnBackPage.gameObject.SetActive(false);
                btnNextPage.gameObject.SetActive(true);
                break;

            case 1:
                btnBackPage.gameObject.SetActive(true);
                break;
            case 3:
                btnNextPage.gameObject.SetActive(true);
                btnStartGame.gameObject.SetActive(false);
                break;
            case 4:
                btnNextPage.gameObject.SetActive(false);
                btnStartGame.gameObject.SetActive(true);
                break;
        }
    }
    #endregion

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

struct HowToPlayTextHelper
{
    public string ExplainSubTitleText;
    public string ExplainDetailText;
}
