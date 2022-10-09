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
        GAME_START, //�����A�j���[�V�����i321�����Ȃ�ŏ���GAME_START�j
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
    [SerializeField] float gameTimeMax; //1����Q�[���̎���
    [SerializeField] float gameTime;    //���̎���

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
    // �v���[���g�̃X�N���v�g
    PresentBoxManager presentBoxManager;        // �ȑO��PresentManager
    NewPresentBoxManager newPresentBoxManager;

    // Start is called before the first frame update
    void Start()
    {
        //  �|�[�Y�p�l����false�ɂ���
        //-------------------------------
        // edit higashi
        //-------------------------------
        // �|�[�Y�͖����ɂ���̂ŁA�����V�ѕ��Ɏg�p�����Ă��炢�܂�
        pnlPause.SetActive(true);
        //  
        pnlStart.SetActive(false);
        pnlEnd.SetActive(false);
        txtEndTitle.gameObject.SetActive(false);
        //txtEndScore.gameObject.SetActive(false);
        btnEndMainMenu.gameObject.SetActive(false);
        btnEndRestart.gameObject.SetActive(false);
        //  �Q�[�����Ԑݒ�
        gameTime = gameTimeMax;
        //  �v���C���[�Q��
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
        txtNextPage.text = "����";
        nextPageClickNum = 0;
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        txtBackPage.text = "�߂�";
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
                        //�Q�[���I��
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

    //  �Q�[���X�e�[�^�X��ύX����
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
    //�@�SActiveUI�������邩�ǂ���
    public void SetEnableAllUI(bool isEnable = true)
    {
        canvas.SetActive(isEnable);
    }
    //  gameTime�̏���
    void FormatTimeText()
    {
        // .ToString("00") -> 00 80 60
        string timeFormat = gameTime.ToString("00");
        txtTime.text = timeFormat;
    }
    //  �c��v���[���g����UI���X�V����
    public void UpdateUIPresentLeft(int presentLeft)
    {
        //txtPresentLeft.text = "�c��v���[���g���F" + presentLeft;
    }
    //  �|�[�Y�{�^�������֐�

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
        //txtEndScore.text = "�X�R�A�F" + playerMain.GetPlayerScore().ToString();
        //txtEndScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        btnEndMainMenu.gameObject.SetActive(true);
        btnEndRestart.gameObject.SetActive(true);
    }


    #region Button Function
    //public void PressBtnPause()
    //{
    //    //�|�[�Y����
    //    if (GameStatus == GAME_STATUS.GAME_NORMAL)
    //    {
    //        Time.timeScale = 0;
    //        pnlPause.SetActive(true);
    //        ChangeGameStatus(GAME_STATUS.GAME_PAUSED);
    //        txtPause.text = "Resume";
    //        return;
    //    }

    //    //�Q�[������
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
                    "\n��肭�g���āA��������̃v���[���g�������悤�I";
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
        Debug.Log("�ς���");
    }
}
