using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewPresentBoxManager : MonoBehaviour
{
    [Header("References")]
    GameManager gameManager;
    NewPresent_Pinata newPresentBox;

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
    // �������Ԃ�UI�̐F�̕ϐ�
    public float colorR = 1.0f;
    public float colorG = 1.0f;
    public float colorB = 1.0f;


    public bool isIndicatePresent;

    public bool presentSceneSetActive;


    // �v���[���g�֘A
    [Header("Present")]
    //public float frameRotate;                       // �v���[���g����]����l
    //public float frameSize;                         // �v���[���g�̃T�C�Y��ύX����l
    //bool isRotate = true;                           // �v���[���g����]���邩�ǂ���

    public bool isCalledTimeUpAnimation = false;           // ���Ԃ��I�������v���[���g�Q�b�g���Ȃ����A

    // ������p�p�[�e�B�N���V�X�e��
    public ParticleSystem[] particles;

    // �v���[���g���X�g�֘A
    [Header("Present List")]
    [SerializeField] public List<GameObject> presentList;       // �v���[���g���X�g
    private GameObject presentRandomObj;                        // �I�΂ꂽ�v���[���g�����X�g�̉��Ԗڂ�      
    //private int presentChoiceNum;                             

    //------------------------------------------------------
    // 221113 �v���[���g�o����@�̎d��
    // �G���[���ł邩���Ȃ̂ŁA��������͂��Ă���
    //Transform���L���b�V�����Ă����B
    Transform tf;

    //�y�C�ӂ̒l�z���[�V�����̒����B
    [Header("���[�V�����̒���(��������)")]
    [SerializeField]
    float duration = 0.8f;

    //�y�C�ӂ̒l�z�A�N�e�B�u������Ă��烂�[�V�����J�n����܂ł̒x���B
    [Header("���[�V�����J�n�܂ł̃f�B���C")]
    [SerializeField]
    [Range(0, 10.0f)]
    float delay = 0;

    //�y�C�ӂ̒l�z�ŏ��̑傫���B
    [Header("�ŏ�Scale(���[�V�����J�n���̃T�C�Y)")]
    [SerializeField]
    Vector3 scaleMin = new Vector3(0, 0, 0);
    //�y�C�ӂ̒l�z�ő�̑傫���BUI�̃f�t�H���gScale * 1.3���炢�̑傫���ɂ���Ǝ��܂肪�ǂ��Ȃ�B
    [Header("�ő�Scale(�f�t�H���gScale * 1.3���x�̒l�𐄏�)")]
    [SerializeField]
    Vector3 scaleMax = new Vector3(1.3f, 1.3f, 1.3f);

    //�f�t�H���gScale���L���B
    Vector3 defaultScale;

    //�R���[�`���Ǘ��p�B
    Coroutine popout;

    //Sin�̋Ȑ���ǂ����܂Ŏg�p�B
    static readonly float Modifier = Mathf.PI * 0.725f;

    //�o�ߎ��ԁB
    float elapsedTime;

    //WaitForSeconds�ł����t���[��new����Ɨǂ��Ȃ��炵���̂ŁA�L���b�V�����Ă����B
    WaitForSeconds delayWait;
    //�|�[�Y��(TimeScale = 0�̎�)�Ɏg�p����ꍇ�́AWaitForSecondsRealtime���g���B
    //    WaitForSecondsRealtime delayWait;
    //------------------------------------------------------

    // �f�R���[�V�����֘A
    [Header("Decoration")]
    public List<GameObject> decorationList;
    public List<GameObject> decorationUsedList = new List<GameObject>();
    private GameObject decorationRandomObj;
    private int decorationChoiceNum;

    [field: SerializeField] bool IsSuccessOpeningPresentBefore { set; get; } = false;

    [field: Header("SFX")]
    [field: SerializeField] SFXPlay SfxPlayPresentGet { set; get; }
    [field: SerializeField] SFXPlay SfxPlayPresentMiss { set; get; }

    void Awake()
    {
        // reference
        newPresentBox = FindObjectOfType<NewPresent_Pinata>();
        gameManager = FindObjectOfType<GameManager>();

        // appearance present
        // ���������Ȃ��Ƃ����Ȃ��̂ŁA��Ƀv���[���g���w�肵�Ă���
        presentBox = GameObject.Find("Present_Pinata");
        tf = presentBox.transform;
        tf.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        defaultScale = tf.localScale;


        if (delay != 0)
        {
            delayWait = new WaitForSeconds(delay);

            //�|�[�Y���Ɏg�p����ꍇ�́AWaitForSecondsRealtime���g���B
            //            delayWait = new WaitForSecondsRealtime(delay);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        presentSceneSetActive = false;

        // �e�I�u�W�F�N�g�̕\���̏�����
        pnlPresentScene.SetActive(false);
        pnlPresentTime.SetActive(false);
        //presentBox.SetActive(false);
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

        for (int i = 0; i < presentList.Count; i++)
        {
            presentList[i].SetActive(false);
        }

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

            if (gameManager.PresentGetNum != 0 && IsSuccessOpeningPresentBefore)
            {
                // ���X�����Ă����f�R���[�V�����������
                decorationUsedList.Add(decorationRandomObj);
                decorationChoiceNum = decorationList.IndexOf(decorationRandomObj);
                decorationList.RemoveAt(decorationChoiceNum);
            }

            // �o�O�h�~�p�ɁA�f�R���[�V�������X�g�̒����O�ɂȂ�����
            // ��x�o���f�R���[�V���������Z�b�g���ă��X�g�ɓ���Ȃ���
            if (decorationList.Count == 0)
            {
                int useCountPrev = decorationUsedList.Count;
                for (int i = 0; i < useCountPrev; ++i)
                {
                    decorationList.Add(decorationUsedList[0]);
                    decorationUsedList.RemoveAt(0);
                }
            }

            //// �N���b�N�J�E���g�����Z�b�g
            //presentAction.clickCount = 0;

            // �ǂ̃v���[���g���o�邩
            presentRandomObj = presentList[Random.Range(0, presentList.Count)];
            //presentChoiceNum = presentList.IndexOf(presentRandomObj);
            //presentList.RemoveAt(presentChoiceNum);
            presentBox = presentRandomObj;
            // �v���[���g�����Z�b�g
            //presentBox.transform.localScale = Vector3.zero;
            tf = presentBox.transform;
            tf.localScale = new Vector3(0.0f, 0.0f, 0.0f);

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
        Debug.Log("TryPresent �ɓ�����");
        // �v���[���g��ʂ̕\��
        pnlPresentScene.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        // �eUI�̕\��
        pnlPresentTime.SetActive(true);
        presentBox.SetActive(true);
        presentTimeNow = presentTimeMax;       // �������Ԃ�MAX�ɂ���

        //IndicatePresent();
        //���d�N���h�~�B        
        if (popout != null)
        {
            StopCoroutine(popout);
        }

        popout = StartCoroutine(Popout());

        isIndicatePresent = true;                     // �v���[���g���o��������
        yield return new WaitForSeconds(2.0f);
        // �������Ԃ̃X�^�[�g
        isTime = true;
    }

    //// �v���[���g�\���̏���
    //public void IndicatePresent()
    //{
    //    // �v���[���g�I�u�W�F�N�g�̃T�C�Y��ύX����
    //    frameSize += 0.1f;
    //    // ��]��������(�ŏ���true)
    //    presentBox.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
    //    if (!isRotate)
    //    {
    //        //*****************************************************
    //        //�Ō�̓J�����̌����ɖ߂��Ă�������(�����J�����̉�]�A�j���[�V�����������)
    //        //���܁A�J������UI��Layer��ScreenSpace-Camera�̂Œ���
    //        //*****************************************************
    //        //presentBox.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {
    //        presentBox.transform.rotation *= Quaternion.AngleAxis(frameRotate, Vector3.back);
    //    }

    //    // �T�C�Y���ő�܂ōs������
    //    if (frameSize > 5.3f)
    //    {
    //        frameSize = 5.3f;

    //        // ��]���~�߂邽�߂�isRotate��false�ɂ���
    //        frameRotate -= 0.1f;
    //        if (frameRotate <= 0.0f)
    //        {
    //            frameRotate = 0.0f;
    //            isRotate = false;
    //        }
    //    }
    //}

    // �f�R���[�V�������Q�b�g�����Ƃ��̏���
    public IEnumerator GetPresent()
    {
        IsSuccessOpeningPresentBefore = true;
        // �ǂ̃f�R���[�V�������o�邩
        decorationRandomObj = decorationList[Random.Range(0, decorationList.Count)];
        decoration = decorationRandomObj;
        imgPresentDecorTransform = decoration.GetComponent<RectTransform>();
        // �v���[���g�����Z�b�g
        //presentBox.transform.localScale = Vector3.zero;

        SfxPlayPresentGet.PlaySFXSound();
        Debug.Log("GetPresent�ɓ�����");
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

        //presentBox.transform.localScale = Vector3.zero;
        changeSpeed = 0.0f;
        scaleChangeTime = 0.0f;
        //frameRotate = 0.0f;
        //frameSize = 0.0f;
        //yield return new WaitForSeconds(2.0f);

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
        IsSuccessOpeningPresentBefore = false;
        SfxPlayPresentMiss.PlaySFXSound();
        Debug.Log("NotGetPresent�ɓ�����");
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

        //Vector3 pos = GetComponent<RectTransform>().anchoredPosition;
        //pos.x = 0;
        //pos.y = 1000;
        //pos.z = 0;
        //imgPresentDecorTransform.anchoredPosition = pos;


        //presentBox.transform.localScale = Vector3.zero;
        changeSpeed = 0.0f;
        scaleChangeTime = 0.0f;
        //frameRotate = 0.0f;
        //frameSize = 0.0f;
        gameManager.GetPresentFromBox(false);
        yield return null;
    }

    IEnumerator Popout()
    {
        elapsedTime = 0;

        if (delay != 0)
        {
            tf.localScale = scaleMin;
            yield return delayWait;
        }

        while (true)
        {
            elapsedTime += Time.deltaTime;
            //�|�[�Y���Ɏg�p����ꍇ�́AdeltaTime�łȂ�unscaledDeltaTime���g���B
            //            elapsedTime += Time.unscaledDeltaTime;


            tf.localScale = Vector3.Lerp(scaleMin, scaleMax, Mathf.Sin(elapsedTime / duration * Modifier));


            if (duration <= elapsedTime)
            {
                //�ʏ�̃X�P�[���ɖ߂��Ă����B
                tf.localScale = new Vector3(5.3f, 5.3f, 5.3f);


                popout = null;
                yield break;
            }
            yield return null;
        }
    }
}
