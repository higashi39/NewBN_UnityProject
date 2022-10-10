using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [field: Header("GameA Animation Settings")]
    //Present���o���� CanvasPresentAppear���o��܂ł̑҂���
    //���̎��̓v���[���g���A�j���[�V����������(��֍s���Ɖ�]����)
    //�A�j���[�V�����̒l����ݒ肷��̂�prefab/present/PresentBoxDisplay
    [field: SerializeField] float AnimationPresentAppearWaitTime { set; get; } = 1.5f;


    [field: Header("Game Properties")]
    //���Q�[���̏��
    [field: SerializeField] public GAME_STATUS GameStatus { private set; get; }
    //���̃Q�[������
    [field: SerializeField] float GameTime { set; get; }
    //�v���C���[�̍ŏ����W�iMapGenerator�Őݒ�j
    //�Q�[���I��������v���C���[�͂��̍��W�ɖ߂�
    [field: SerializeField] public Vector3 PlayerFirstPos { set; get; }
    //��������v���[���g����
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
        // �|�[�Y�͖����ɂ���̂ŁA�����V�ѕ��Ɏg�p�����Ă��炢�܂�
        pnlPause.SetActive(true);

        pnlStart.SetActive(false);
        pnlEnd.SetActive(false);
        txtEndTitle.gameObject.SetActive(false);
        btnEndMainMenu.gameObject.SetActive(false);
        btnEndRestart.gameObject.SetActive(false);
        pnlPresentGet.SetActive(false);

        //�Q�[�����Ԑݒ�
        GameTime = GameTimeMax;

        //�v���C���[�̓��͂�false�ɂ���
        playerMain.SetPlayerInputDisabled();

        //���Ԃ�UI�����킹�������ɂ���
        FormatTimeText();

        //-------------------------------
        // add higashi
        //-------------------------------
        HowToPlay();
        imgBoard.SetActive(true);
        howToPlay.gameObject.SetActive(true);
        GameStatus = GAME_STATUS.GAME_HOWTOPLAY;
        txtNextPage.text = "����";
        nextPageClickNum = 0;
        txtBackPage.text = "�߂�";
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
                        //�Q�[���I��
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
    //�Q�[���I���A�j���V����
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
    //�@�SActiveUI�������邩�ǂ���
    public void SetEnableAllUI(bool isEnable = true)
    {
        canvas.SetActive(isEnable);
    }
    //  gameTime�̏���
    void FormatTimeText()
    {
        // .ToString("00") -> 00 80 60
        string timeFormat = GameTime.ToString("00");
        txtTime.text = timeFormat;
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
            pnlPause.SetActive(true);
            GameStatus = GAME_STATUS.GAME_PAUSED;
            txtPause.text = "Resume";
            return;
        }

        //�Q�[������
        if (GameStatus == GAME_STATUS.GAME_PAUSED)
        {
            GameStatus = GAME_STATUS.GAME_NORMAL;
            pnlPause.SetActive(false);
            Time.timeScale = 1;
            txtPause.text = "Pause";
            return;
        }
    }

    //���C�����j���[�ɖ߂�
    //pnlEnd�̃{�^���ɐݒ肷��
    public void PressBtnMainMenu()
    {
        SFXButton.Instance.PlayButtonPressSFX();
        SceneManager.LoadScene("MainMenu");
    }

    //�Q�[�������Z�b�g����
    //pnlEnd�̃{�^���ɐݒ肷��
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
                txtExplainSubTitle.text = "1. �}�E�X�J�[�\���ňړ�";
                txtExplainDetail.text = "�M���[�N�̓}�E�X�J�[�\���̂�������Ɉړ������\n��肭�U�����Ă����悤";
                break;

            case 1:
                btnBackPage.gameObject.SetActive(true);
                txtExplainSubTitle.text = "2. ���N���b�N�z�[���h�œ����J��";
                txtExplainDetail.text = "���̂���Ƃ���ō��N���b�N����莞�ԃz�[���h����ƁA\n����������" +
                    "\n��������Ɗm���Ńv���[���g���I\n��������̃v���[���g���W�߂悤";
                break;
            case 2:
                txtExplainSubTitle.text = "3. �v���[���g���J���悤";
                txtExplainDetail.text = "�v���[���g����������J���Ă݂悤\n�v���[���g�̊J�����̓v���[���g�ɂ���ėl�X" +
                    "\n�N���b�N������A������������...\n�������ԓ��ɑf�����J���Ă݂悤";
                break;
            case 3:
                txtExplainSubTitle.text = "���Q�[�W�̐����� ������@�̏[�d";
                txtExplainDetail.text = "���������Ă���ƁA�F�̑�����@�Q�[�W�������" +
                    "\n�Q�[�W����ɂȂ�ƁA��������Ȃ��Ȃ��Ă��܂��I" +
                    "�}�b�v�̏㑤�ɂ���Ƃ̑O�܂ōs���āA�[�d���悤�I";
                btnNextPage.transform.localPosition = new Vector3(230, -205, 0);
                txtNextPage.text = "����";
                break;
            case 4:
                txtExplainSubTitle.text = "���Q�[�W�̐����� �M���[�N�̃X�L��";
                txtExplainDetail.text = "���������Ă���Ɖ��F�̃X�L���Q�[�W�����܂��" +
                    "\n�Q�[�W�������ς��ɂȂ�����ԂŉE�N���b�N�������ƁA\n�X�L������������āA�M���[�N�̃X�s�[�h���������I" +
                    "\n��C�ɑ������邱�Ƃ��ł��邩��A��肭�g���āA��������̃v���[���g�������悤�I";
                btnNextPage.gameObject.SetActive(true);
                btnNextPage.transform.localPosition = new Vector3(0, -205, 0);
                txtNextPage.text = "�X�^�[�g";
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

        Debug.Log("�ς���");
    }



}
