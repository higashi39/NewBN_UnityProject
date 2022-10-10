using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class NewPresentBox : MonoBehaviour
{
    [Header("References")]
    NewPresentBoxManager newPresentBoxManager;
    //PresentAction presentAction;

    // �N���b�N�������ɔ���h�炷�p�ϐ�
    public float shakeDuration;
    public float strength;
    public int vibrato;
    public float randomness;
    public bool snapping;
    public bool fadeOut;

    // ������p�p�[�e�B�N���V�X�e��
    public ParticleSystem[] particles;

    GameManager gameManager;

    //// �v���[���g�֘A
    //[Header("Present")]
    //public float frameRotate;                       // �v���[���g����]����l
    //public float frameSize;                         // �v���[���g�̃T�C�Y��ύX����l
    //bool isRotate = true;                           // �v���[���g����]���邩�ǂ���

    //bool isIndicatePresent;

    //bool isClicked;

    private void Awake()
    {
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

        //isClicked = false;

        //// �e�I�u�W�F�N�g�̕\���̏�����
        //pnlPresentScene.SetActive(false);
        //imgPresent.SetActive(false);
        //pnlPresentTime.SetActive(false);

        //// �������Ԃ̏�����
        //presentTimeNow = presentTimeMax;
        //isTime = false;
        //enlarge = true;
        //isIndicatePresent = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (newPresentBoxManager.isIndicatePresent)
        //{
        //    newPresentBoxManager.IndicatePresent();
        //}

        //if (presentAction.presentTimeNow > 0.0f && presentAction.presentTimeNow <= 5.0f)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        imgPresent.transform.DOShakePosition(shakeDuration, strength, vibrato, randomness, snapping, fadeOut);
        //        ++clickCount;
        //    }

        //    if (clickCount > 15)
        //    {
        //        // ----------------
        //        // �f�R���[�V�����Q�b�g
        //        // ----------------
        //        isClicked = true;
        //    }
        //    else
        //    {
        //        // ----------------------------
        //        // �f�R���[�V�������Q�b�g�ł��Ȃ�����
        //        // ----------------------------
        //        //StartCoroutine(NotGetPresent());
        //    }
        //}

        //if(isClicked)
        //{
        //    GetPresent();
        //    newPresentBoxManager.DecorListRandom();
        //}
        //else
        //{

        //}

        //if(Input.GetKey(KeyCode.O))
        //{
        //    imgPresent.transform.DOShakePosition(shakeDuration, strength, vibrato, randomness, snapping, fadeOut);

        //}
    }

    //// �v���[���g�J���̏���
    //public IEnumerator TryPresent()
    //{
    //    // �v���[���g��ʂ̕\��
    //    newPresentBoxManager.pnlPresentScene.SetActive(true);
    //    yield return new WaitForSeconds(1.0f);
    //    // �eUI�̕\��
    //    newPresentBoxManager.pnlPresentTime.SetActive(true);
    //    newPresentBoxManager.presentBox.SetActive(true);
    //    newPresentBoxManager.presentTimeNow = newPresentBoxManager.presentTimeMax;       // �������Ԃ�MAX�ɂ���

    //    isIndicatePresent = true;                     // �v���[���g���o��������
    //    yield return new WaitForSeconds(2.0f);
    //    // �������Ԃ̃X�^�[�g
    //    newPresentBoxManager.isTime = true;
    //}

    //// �v���[���g�\���̏���
    //public void IndicatePresent()
    //{
    //    // �v���[���g�I�u�W�F�N�g�̃T�C�Y��ύX����
    //    frameSize += 0.01f;
    //    // ��]��������(�ŏ���true)
    //    newPresentBoxManager.presentBox.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
    //    if (!isRotate)
    //    {
    //        newPresentBoxManager.presentBox.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {
    //        newPresentBoxManager.presentBox.transform.rotation *= Quaternion.AngleAxis(frameRotate, Vector3.back);
    //    }

    //    // �T�C�Y���ő�܂ōs������
    //    if (frameSize > 1.0f)
    //    {
    //        frameSize = 1.0f;

    //        // ��]���~�߂邽�߂�isRotate��false�ɂ���
    //        frameRotate -= 0.1f;
    //        if (frameRotate <= 0.0f)
    //        {
    //            frameRotate = 0.0f;
    //            isRotate = false;
    //        }
    //    }
    //}


    //// �f�R���[�V�������Q�b�g�����Ƃ��̏���
    //public IEnumerator GetPresent()
    //{
    //    newPresentBoxManager.isTime = false;                 // �������Ԃ��~�߂�
    //    yield return new WaitForSeconds(0.1f);
    //    //newPresentBoxManager.DecorListRandom();
    //    RectTransform imgPresentDecorTransform = newPresentBoxManager.decoration.GetComponent<RectTransform>();
    //    imgPresentDecorTransform.DOAnchorPosY(0.0f, 1.0f).SetEase(Ease.OutBounce);
    //    StartCoroutine(newPresentBoxManager.FinishOpenPresent());
    //}

    //// �f�R���[�V�������Q�b�g�ł��Ȃ������Ƃ��̏���
    //IEnumerator NotGetPresent()
    //{
    //    //RectTransform notGetPresent = newPresentBoxManager.presentBox.GetComponent<RectTransform>();
    //    //float oriPosY = imgPresent.transform.position.y;

    //    Debug.Log("�v���[���g���J�����Ƃ��ł��Ȃ������I");
    //    //imgPresent.transform.DOShakeScale(2.0f, 0.15f, 4, 20);
    //    //yield return new WaitForSeconds(2.0f);
    //    //notGetPresent.DOAnchorPosY(oriPosY, 1.25f).SetEase(Ease.InBack);
    //    yield return new WaitForSeconds(1.25f);
    //    Debug.Log("����܂����I");

    //    newPresentBoxManager.pnlPresentScene.SetActive(false);
    //    newPresentBoxManager.pnlPresentTime.SetActive(false);
    //    StartCoroutine(gameManager.BackGamePlay());
    //    yield return new WaitForSeconds(1.0f);

    //}
}
