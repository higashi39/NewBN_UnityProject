using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    //�Q�[�����
    public enum GAME_STATUS
    {
        GAME_HOWTOPLAY,
        GAME_START, //�����A�j���[�V�����i321�����Ȃ�ŏ���GAME_START�j
        GAME_PAUSED,
        GAME_NORMAL,
        GAME_PRESENT_APPEAR,
        GAME_ENDED,
    }

    [field: Header("Game Settings")]
    //1����Q�[���̎���
    [field: SerializeField] float GameTimeMax { set; get; }

    [field: Header("Game Animation Settings")]
    //Present���o���� CanvasPresentAppear���o��܂ł̑҂���
    //���̎��̓v���[���g���A�j���[�V����������(��֍s���Ɖ�]����)
    //�A�j���[�V�����̒l����ݒ肷��̂�prefab/present/PresentBoxDisplay
    [field: SerializeField] float AnimationPresentAppearWaitTime { set; get; } = 1.5f;


    [field: Header("Game Properties")]
    //���Q�[���̏��
    [field: SerializeField] public GAME_STATUS GameStatus { private set; get; }
    //���̃Q�[������
    [field: SerializeField] float GameTime { set; get; }
    [field: SerializeField] int GameTimeBefore { set; get; }
    //�v���C���[�̍ŏ����W�iMapGenerator�Őݒ�j
    //�Q�[���I��������v���C���[�͂��̍��W�ɖ߂�
    [field: SerializeField] public Vector3 PlayerFirstPos { set; get; }
    //��������v���[���g����
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
        // �|�[�Y�͖����ɂ���̂ŁA�����V�ѕ��Ɏg�p�����Ă��炢�܂�
        SetGameStartUI();

        //�Q�[�����Ԑݒ�
        GameTime = GameTimeMax;

        //�v���C���[�̓��͂�false�ɂ���
        playerMain.SetPlayerInputDisabled();

        //���Ԃ�UI�����킹�������ɂ���
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
                        //�Q�[���I��
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

    //GameStatus��GAME_STATUS.GAME_PRESENT_APPEAR�ɂ���
    //�v���C���[�̓��͂�off�ɂ���
    //UI���B���āA�v���[���g�����ꂽ�A�j���V�������Ă�
    public void SetGameStatusPresentAppear()
    {
        GameStatus = GAME_STATUS.GAME_PRESENT_APPEAR;
        playerMain.SetPlayerInputDisabled();
        SetEnableAllUI(false);
        StartCoroutine(BoxAppearAnimation());
    }

    #region Animation
    //�Q�[���X�^�[�g�A�j���V����
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
    //�Q�[���I���A�j���V����
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
    //�v���[���g�������ꂽ�A�j���V����
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
    //�v���[���g�����炤���ǂ����iPresentScene���画�f�j
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

    //�@�SActiveUI�������邩�ǂ���
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

    //  gameTime�̏���
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
    //  �|�[�Y�{�^�������֐�
    public void PressBtnPause()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        //�|�[�Y����
        if (GameStatus == GAME_STATUS.GAME_NORMAL)
        {
            Time.timeScale = 0;
            pnlBlock.SetActive(true);
            GameStatus = GAME_STATUS.GAME_PAUSED;
            txtPause.text = "Resume";
            return;
        }

        //�Q�[������
        if (GameStatus == GAME_STATUS.GAME_PAUSED)
        {
            GameStatus = GAME_STATUS.GAME_NORMAL;
            pnlBlock.SetActive(false);
            Time.timeScale = 1;
            txtPause.text = "Pause";
            return;
        }
    }

    //���C�����j���[�ɖ߂�
    //pnlEnd�̃{�^���ɐݒ肷��
    public void PressBtnMainMenu()
    {
        GameTutorialIndicator.instance.IsShowTutorial = true;
        SFXButton.Instance.PlayButtonPressSFX();
        EndGameButtonPressAnimation();
        SceneTransitionAnimation.instance.ChangeScene("MainMenu");
    }

    //�Q�[�������Z�b�g����
    //pnlEnd�̃{�^���ɐݒ肷��
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

        //1. �}�E�X�J�[�\���ňړ�
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "1. �}�E�X�J�[�\���ňړ�";
            tmp.ExplainDetailText = "�M���[�N�̓}�E�X�J�[�\���̂�������Ɉړ������\n��肭�U�����Ă����悤";

            txtExplainSubTitle.text = tmp.ExplainSubTitleText;
            txtExplainDetail.text = tmp.ExplainDetailText;

            TextSubTitleList.Add(tmp);
        }

        //2.���N���b�N�z�[���h�œ����J��
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "2.���N���b�N�z�[���h�œ����J��";
            tmp.ExplainDetailText = "���̂���Ƃ���ō��N���b�N����莞�ԃz�[���h�����\n����������I" +
                            "\n��������Ɗm���Ńv���[���g����ɓ���I\n��������̃v���[���g���W�߂悤";

            TextSubTitleList.Add(tmp);
        }

        //3. �v���[���g���J���悤
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "3. �v���[���g���J���悤";
            tmp.ExplainDetailText = "�v���[���g����������J���Ă݂悤�I\n�v���[���g�̊J�����̓v���[���g�ɂ���ĈႤ��" +
                            "\n�N���b�N������A������������...\n�������ԓ��ɑf�����J���Ă݂悤�I�I�I";

            TextSubTitleList.Add(tmp);
        }

        //���Q�[�W�̐����� ������@�̏[�d
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "���Q�[�W�̐����� ������@�̏[�d";
            tmp.ExplainDetailText = "���������Ă���ƁA�F�̑�����@�Q�[�W�������" +
                            "\n�Q�[�W����ɂȂ�ƁA��������Ȃ��Ȃ��Ă��܂��E�E�E\n" +
                            "�}�b�v�̏㑤�ɂ���Ƃ̑O���A��ɂ���[�d���\n�[�d���悤�I";

            TextSubTitleList.Add(tmp);
        }

        //���Q�[�W�̐����� �M���[�N�̃X�L��
        {
            HowToPlayTextHelper tmp;
            tmp.ExplainSubTitleText = "���Q�[�W�̐����� �M���[�N�̃X�L��";
            tmp.ExplainDetailText = "���������Ă���Ɖ��F�̃X�L���Q�[�W�����܂��" +
                            "\n�Q�[�W�������ς��ɂȂ�����ԂŉE�N���b�N�������ƁA\n�X�L������������āA�M���[�N�̃X�s�[�h���������I" +
                            "\n��C�ɑ������邱�Ƃ��ł��邩��A\n��肭�g���āA��������̃v���[���g�������悤�I";

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

        Debug.Log("�ς���");
    }

}

struct HowToPlayTextHelper
{
    public string ExplainSubTitleText;
    public string ExplainDetailText;
}
