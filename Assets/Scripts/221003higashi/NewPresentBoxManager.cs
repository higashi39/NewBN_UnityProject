using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewPresentBoxManager : MonoBehaviour
{
    [Header("References")]
    GameManager gameManager;
    NewPresentBox newPresentBox;

    [SerializeField] public GameObject pnlPresentScene;
    [SerializeField] public GameObject pnlPresentTime;
    [SerializeField] public GameObject presentBox;
    [SerializeField] public GameObject decoration;
    RectTransform imgPresentDecorTransform;

    // �������Ԋ֘A
    [Header("Time Text")]
    [SerializeField] public Text txtPresentTime;           // �v���[���g�̐�������
    [SerializeField] public float presentTimeMax;          // �v���[���g�̐������Ԃ̍ő�
    [SerializeField] public float presentTimeNow;          // ���̎���
    public bool isTime;                             // ���Ԃ�i�߂邩�ǂ���
    public float scaleChangeTime, changeSpeed;      // �T�C�Y���ς�鎞�� / ���̃X�s�[�h 
    public bool enlarge;                            // �T�C�Y��傫�����邩�A���������邩�̃t���O

    public bool isIndicatePresent;

    PresentGenerator presentGenerator;
    public bool presentSceneSetActive;

    // �v���[���g�֘A
    [Header("Present")]
    public float frameRotate;                       // �v���[���g����]����l
    public float frameSize;                         // �v���[���g�̃T�C�Y��ύX����l
    bool isRotate = true;                           // �v���[���g����]���邩�ǂ���

    // UI�̐F�̕ϐ�
    public float colorR = 1.0f;
    public float colorG = 1.0f;
    public float colorB = 1.0f;


    public bool isCalledTimeUpAnimation = false;           // ���Ԃ��I�������v���[���g�Q�b�g���Ȃ����A

    //[Header("Present Box Info")]
    //public Text txtPresentInfo;

    // Start is called before the first frame update
    void Start()
    {
        presentSceneSetActive = false;

        // �e�I�u�W�F�N�g�̕\���̏�����
        pnlPresentScene.SetActive(false);
        pnlPresentTime.SetActive(false);
        presentBox.SetActive(false);
        decoration.SetActive(false);
        imgPresentDecorTransform = decoration.GetComponent<RectTransform>();

        //Vector3 pos = GetComponent<RectTransform>().anchoredPosition;
        //pos.x = 0;
        //pos.y = 1000;
        //pos.z = 0;
        //imgPresentDecorTransform.anchoredPosition = pos;

        // �������Ԃ̏�����
        presentTimeNow = presentTimeMax;
        isTime = false;
        enlarge = true;
        isIndicatePresent = false;

        newPresentBox = FindObjectOfType<NewPresentBox>();
        gameManager = FindObjectOfType<GameManager>();
        presentGenerator = FindObjectOfType<PresentGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (presentSceneSetActive)
        {
            Vector3 pos = decoration.GetComponent<RectTransform>().anchoredPosition;
            pos.x = 0;
            pos.y = 1000;
            pos.z = 0;
            imgPresentDecorTransform.anchoredPosition = pos;

            // �v���[���g�����Z�b�g
            presentBox.transform.localScale = Vector3.zero;

            //// �N���b�N�J�E���g�����Z�b�g
            //presentAction.clickCount = 0;

            StartCoroutine(TryPresent());
            presentSceneSetActive = false;
            isCalledTimeUpAnimation = false;
        }

        // �������Ԃ̏���
        OpenPresentTimeText();

        // �������Ԃ�\������t���O����������
        if (isTime)
        {
            // �������Ԃ̏������s��
            TimeStart();
        }

        if (isIndicatePresent)
        {
            IndicatePresent();
        }
    }

    // PresentTime�̏���
    void OpenPresentTimeText()
    {
        string timeformat = "Time:" + presentTimeNow.ToString("0.00");
        txtPresentTime.text = timeformat;
    }

    // �������Ԃ̏���
    void TimeStart()
    {
        // �������Ԃ����炷
        presentTimeNow -= Time.deltaTime;
        // ��萔�������Ԃ���������
        if (isIndicatePresent)
        {
            // �������Ԃ�UI��召��������A�F��ς����肷��
            if (presentTimeNow <= 3.0f)
            {

                // �ύX�̃X�p��
                changeSpeed = Time.deltaTime * 0.1f;
                if (scaleChangeTime < 0)
                {
                    enlarge = true;
                }
                if (scaleChangeTime > 0.6f)
                {
                    enlarge = false;
                }

                if (enlarge == true)
                {
                    scaleChangeTime += Time.deltaTime;
                    txtPresentTime.transform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed);
                    colorG--;
                    colorB--;
                }
                else
                {
                    scaleChangeTime -= Time.deltaTime;
                    txtPresentTime.transform.localScale -= new Vector3(changeSpeed, changeSpeed, changeSpeed);
                    colorG++;
                    colorB++;
                }
                // UI�̐F�̕\��
                txtPresentTime.color = new Color(colorR, colorG, colorB);

                // �������Ԃ�0�b�ɂȂ�����
                if (presentTimeNow <= 0.0f)
                {
                    // --------------------------
                    // �J�����ԏI��(�Q�[���ɖ߂�)
                    // --------------------------
                    presentTimeNow = 0.0f;
                    if (!isCalledTimeUpAnimation)
                    {
                        isCalledTimeUpAnimation = true;
                        StartCoroutine(NotGetPresent());
                    }

                    // �F���傫�����߂�
                    txtPresentTime.transform.localScale = new Vector3(1, 1, 1);
                    txtPresentTime.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    // �v���[���g�J���̏���
    public IEnumerator TryPresent()
    {
        // �v���[���g��ʂ̕\��
        pnlPresentScene.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        // �eUI�̕\��
        pnlPresentTime.SetActive(true);
        presentBox.SetActive(true);
        presentTimeNow = presentTimeMax;       // �������Ԃ�MAX�ɂ���

        isIndicatePresent = true;                     // �v���[���g���o��������
        yield return new WaitForSeconds(2.0f);
        // �������Ԃ̃X�^�[�g
        isTime = true;
    }

    // �v���[���g�\���̏���
    public void IndicatePresent()
    {
        // �v���[���g�I�u�W�F�N�g�̃T�C�Y��ύX����
        frameSize += 0.1f;
        // ��]��������(�ŏ���true)
        presentBox.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
        if (!isRotate)
        {
            //*****************************************************
            //�Ō�̓J�����̌����ɖ߂��Ă�������(�����J�����̉�]�A�j���[�V�����������)
            //���܁A�J������UI��Layer��ScreenSpace-Camera�̂Œ���
            //*****************************************************
            //presentBox.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            presentBox.transform.rotation *= Quaternion.AngleAxis(frameRotate, Vector3.back);
        }

        // �T�C�Y���ő�܂ōs������
        if (frameSize > 5.3f)
        {
            frameSize = 5.3f;

            // ��]���~�߂邽�߂�isRotate��false�ɂ���
            frameRotate -= 0.1f;
            if (frameRotate <= 0.0f)
            {
                frameRotate = 0.0f;
                isRotate = false;
            }
        }
    }

    // �f�R���[�V�������Q�b�g�����Ƃ��̏���
    public IEnumerator GetPresent()
    {
        isTime = false;                 // �������Ԃ��~�߂�
        //newPresentBoxManager.DecorListRandom();
        presentBox.SetActive(false);
        decoration.SetActive(true);
        imgPresentDecorTransform.DOAnchorPosY(0.0f, 1.0f).SetEase(Ease.OutBounce);
        isIndicatePresent = false;

        colorR = 1.0f;
        colorG = 1.0f;
        colorB = 1.0f;

        txtPresentTime.transform.localScale = new Vector3(1, 1, 1);
        txtPresentTime.color = new Color(1.0f, 1.0f, 1.0f);

        presentBox.transform.localScale = Vector3.zero;
        changeSpeed = 0.0f;
        scaleChangeTime = 0.0f;
        frameRotate = 0.0f;
        frameSize = 0.0f;

        gameManager.GetPresentFromBox();
        //StartCoroutine(gameManager.BackGamePlay());
        //StartCoroutine(FinishOpenPresent());
        //yield return new WaitForSeconds(5.0f);
        //Debug.Log("�����ĂȂ�...");
        //pnlPresentTime.SetActive(false);
        //pnlPresentScene.SetActive(false);
        yield return null;
    }

    public IEnumerator NotGetPresent()
    {
        //RectTransform notGetPresent = newPresentBoxManager.presentBox.GetComponent<RectTransform>();
        //float oriPosY = imgPresent.transform.position.y;

        //Debug.Log("�v���[���g���J�����Ƃ��ł��Ȃ������I");
        ////imgPresent.transform.DOShakeScale(2.0f, 0.15f, 4, 20);
        ////yield return new WaitForSeconds(2.0f);
        ////notGetPresent.DOAnchorPosY(oriPosY, 1.25f).SetEase(Ease.InBack);
        //yield return new WaitForSeconds(1.25f);
        //Debug.Log("����܂����I");

        //pnlPresentScene.SetActive(false);
        //pnlPresentTime.SetActive(false);
        ////StartCoroutine(gameManager.BackGamePlay());
        isTime = false;
        presentBox.transform.DOShakeScale(2.0f, 1.0f, 4, 20);
        yield return new WaitForSeconds(2.0f);
        isIndicatePresent = false;

        colorR = 1.0f;
        colorG = 1.0f;
        colorB = 1.0f;

        txtPresentTime.transform.localScale = new Vector3(1, 1, 1);
        txtPresentTime.color = new Color(1.0f, 1.0f, 1.0f);

        presentBox.SetActive(false);
        presentBox.transform.localScale = Vector3.zero;
        changeSpeed = 0.0f;
        scaleChangeTime = 0.0f;
        frameRotate = 0.0f;
        frameSize = 0.0f;
        gameManager.GetPresentFromBox(false);
        yield return null;
    }
}
