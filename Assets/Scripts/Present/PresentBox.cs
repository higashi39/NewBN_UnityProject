using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class PresentBox : MonoBehaviour
{
    [Header("Box Status")]
    [SerializeField] bool isClickable = false;
    [SerializeField] bool isOnSkill = false;
    public PRESENT_TYPE presentType;

    [Header("Box Shiny Settings")]
    const float shineTimeMax = 6.0f;
    [SerializeField] float shineTimeNow = 0.0f;

    [Header("Shiny Light")]
    public AnimationCurve lightIntensity;
    Light shinyLight;

    [Header("References")]
    PlayerMain playerMain;
    PresentBoxManager presentBoxManager;

    [Header("Box Score")]
    public int boxScore = 100;

    public enum PRESENT_TYPE
    {
        PRESENT,
        HAZURE,
    }

    // �N���b�N�����I�u�W�F�N�g�� PlayerClickBox.cs �̃N���b�N�����Ŏ擾
    [HideInInspector] public GameObject clickedObj;
    // �擾�����I�u�W�F�N�g�� CinemachineVirtualCamera ������ϐ�
    CinemachineVirtualCamera vCam;

    // �f�R���[�V�������N���b�N���ꂽ�v���[���g�I�u�W�F�N�g�̎q�ɂ���
    GameObject decorObject;

    // �v���[���g�I�u�W�F�N�g�̎q(BoxandLid)�Ƒ�(PresentLid)������p�ϐ�
    public GameObject childPresentlid;
    public GameObject childBoxandlid;

    // ����h�炷�p�ϐ�
    public float shakeDuration;
    public float strength;
    public int vibrato;
    public float randomness;
    public bool snapping;
    public bool fadeOut;

    // �v���[���g�I�u�W�F�N�g�̒��̃{�[���i�ӂ����J�����u�Ԑ^��ɔ�ԁj�p
    [SerializeField] private GameObject impulseSphere;
    private Rigidbody impulseSphereRb;       // Rigidbody�̎擾

    // ���̂ӂ����΂��p�ϐ�
    [HideInInspector] public GameObject child;
    public float jumpPower;
    public int jumpNum;
    public float jumpDuration;

    // ������p�p�[�e�B�N���V�X�e��
    public ParticleSystem[] particles;

    // Start is called before the first frame update
    void Start()
    {
        shinyLight = GetComponentInChildren<Light>();
        playerMain = FindObjectOfType<PlayerMain>();
        presentBoxManager = FindObjectOfType<PresentBoxManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsOnSkill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isClickable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isClickable = false;
    }

    public void OpenBox()
    {
        if (!isClickable)
        {
            Debug.Log("Too far from the box�i�v���[���g���牓������j");
            Collider[] cols = Physics.OverlapSphere(transform.position, 1.0f);
            bool isAnyBushes = false;
            for (int i = 0; i < cols.Length; ++i)
            {
                if (cols[i].CompareTag("Bush"))
                {
                    isAnyBushes = true;
                    break;
                }
            }
            if (!isAnyBushes) presentBoxManager.ShowUIBoxInfo();
            return;
        }
        Debug.Log("Open the box(�v���[���g���J��)");

        //  �X�L�����܂��Đ�������Off�ɂ���
        TurnOffLight();

        //�v���C���[���t�F�[�h����
        //playerMain.SetPlayerFade();
        //  �v���C���[�͓��͏o���ł��Ȃ�
        playerMain.SetPlayerInputEnable(false);
        //playerMain.AddPlayerScore(boxScore);


        //  presentBoxManager�ɕ񍐂���
        presentBoxManager.OpenBoxStart();

        // �v���[���g���J����邱�ƂɂȂ�����A clickedObj �̎q�̃I�u�W�F�N�g�i Cinemachine �����j���擾
        // ���̎q�I�u�W�F�N�g�̃R���|�[�l���g CinemachineVirtualCamera ��ϐ��ɓ����
        vCam = clickedObj.GetComponentInChildren<CinemachineVirtualCamera>();
        // �N���b�N���ꂽ�v���[���g�� Priority(�J�����D�揇�ʂ����߂����) ���v���C���[�����傫���l�ɂ���
        vCam.Priority = 20;

        // �N���b�N���ꂽ�q�I�u�W�F�N�g�̎擾)
        // �uBoxandLid�v(�v���[���g�̔��Ƃӂ�)���擾�i�h�炷���߁j
        childBoxandlid = transform.Find("BoxandLid").gameObject;
        // �uPresentLid�v(�v���[���g�̂ӂ�)���擾�i��΂����߁j
        childPresentlid = childBoxandlid.transform.Find("PresentLid").gameObject;

        // �v���[���g�I�u�W�F�N�g�ɓ����Ă���ImpulseSphere���擾
        impulseSphere = transform.Find("ImpulseSphere").gameObject;
        impulseSphereRb = impulseSphere.GetComponent<Rigidbody>();

        if (presentType == PRESENT_TYPE.PRESENT)
        {
            // �v���[���g�̒��g�����߂郉���_������
            presentBoxManager.DecorListRandom();

            CreateRandObj();
            //Debug.Log(presentBoxManager.randObj);

            // �v���[���g���J���鎞�̏���
            StartCoroutine(AtariboxAction());
        }
        else if (presentType == PRESENT_TYPE.HAZURE)
        {
            // �N���b�N�����I�u�W�F�N�g��image���擾����
            //hazureImageManager.LoadThisImage(this);

            // �N���b�N�����I�u�W�F�N�g��Canvas �� image �ɕt���Ă���X�N���v�g���擾
            //hazureCanvas = hazureImageManager.clickedCanvas;
            //hazureImage = hazureImageManager.clickedHazereImage;

            //changeSize = hazureImageManager.clickedHazere.GetComponent<ChangeSize>();
            // �n�Y���v���[���g�̃A�j���[�V����
            StartCoroutine(HazureboxAction());
        }


        //// �v���[���g���J���鎞�̏���
        //StartCoroutine(giftboxAction());
    }

    //�v���[���g�����L���L��������֐�
    public void MakeBoxShiny()
    {
        TurnOnLight();
    }

    //�X�V�����֐���isOnSkill���`�F�b�N����֐�
    //isOnSkill��true�ꍇ�� false�܂ŏ���������ilight�E����on��off��on��...�j
    void CheckIsOnSkill()
    {
        if (!isOnSkill) return;

        shineTimeNow += Time.deltaTime;
        if (shineTimeNow >= shineTimeMax)
        {
            TurnOffLight();
            return;
        }

        shinyLight.intensity = lightIntensity.Evaluate(shineTimeNow);

    }

    //Light�E����On�ɂ���֐�
    void TurnOnLight()
    {
        shineTimeNow = 0.0f;
        shinyLight.enabled = true;
        isOnSkill = true;
    }

    //Light�E����Off�ɂ���֐�
    void TurnOffLight()
    {
        shinyLight.enabled = false;
        isOnSkill = false;
    }

    // �v���[���g�̒��g�����߂鏈���i���݁A�d���Ȃ��j
    void CreateRandObj()
    {

        Debug.Log(presentBoxManager.randObj.name);

        decorObject = (GameObject)Instantiate(presentBoxManager.randObj, new Vector3(clickedObj.gameObject.transform.position.x
                                       , clickedObj.gameObject.transform.position.y + 10.0f
                                       , clickedObj.gameObject.transform.position.z - 1.5f)
                                       , Quaternion.identity);

        //decorObject.transform.parent = clickedObj.transform;

        //decorRb = presentBoxManager.randObj.GetComponent<Rigidbody>();
    }

    // ������̃v���[���g���J�����̏���
    IEnumerator AtariboxAction()
    {
        yield return new WaitForSeconds(0.7f);
        ShakeBox();
        yield return new WaitForSeconds(1.0f);
        FlyBoxLid();
        yield return new WaitForSeconds(0.5f);
        ImpulseSphere();
        DecorFall();
        yield return new WaitForSeconds(1.3f);
        ConfettiAction();
        //if (impulseSphere.transform.position.y > 20.0f)
        //{
        //    Destroy(impulseSphere);
        //}
        yield return new WaitForSeconds(5.0f);
        presentBoxManager.SetDecorTranslate();
        decorObject.transform.position = presentBoxManager.newDecorPos;
        decorObject.transform.localScale = presentBoxManager.newDecorScale;
        StartCoroutine(DeleteBox());
    }

    IEnumerator HazureboxAction()
    {
        yield return new WaitForSeconds(0.7f);
        ShakeBox();
        yield return new WaitForSeconds(1.0f);
        FlyBoxLid();
        //ConfettiAction();
        yield return new WaitForSeconds(0.7f);
        ImpulseSphere();
        yield return new WaitForSeconds(1.0f);

        GameObject imgHazure = GameObject.Find("NewHazureImage");
        RectTransform hazureRectTransform = imgHazure.GetComponent<RectTransform>();
        float oriPosY = imgHazure.transform.position.y;
        float targetPosY = 0.0f;

        yield return new WaitForSeconds(0.25f);
        hazureRectTransform.DOAnchorPosY(targetPosY, 1.25f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.25f);
        imgHazure.transform.DOShakeScale(2.0f, 0.15f, 4, 20);
        yield return new WaitForSeconds(2.0f);
        hazureRectTransform.DOAnchorPosY(oriPosY, 1.25f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(1.25f);
        StartCoroutine(DeleteBox());
    }

    // ����h�炷����
    void ShakeBox()
    {
        // duration   : ����
        // strength   : �h��̋���
        // vibrato    : �ǂ̂��炢�U�����邩
        // randomress : �����_���x��(0 ~ 180)
        // fadeOut    : �t�F�[�h�A�E�g���邩 
        childBoxandlid.transform.DOShakePosition(shakeDuration, strength, vibrato, randomness, snapping, fadeOut);
    }

    // ���̂ӂ���΂�����
    void FlyBoxLid()
    {
        // jumpPower    : �W�����v����͗�
        // jumpNum      : ����W�����v���邩
        // jumpDuration : �W�����v���鎞�� 
        //childPresentlid.transform.DOJump(new Vector3(2, 0, 2), jumpPower, jumpNum, jumpDuration);

        Vector3 newPos = childPresentlid.transform.position;
        const float flyDistance = 2.0f;
        float randomRadian = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        newPos.x += flyDistance * Mathf.Sin(randomRadian);
        newPos.z += flyDistance * Mathf.Cos(randomRadian);
        newPos.y = -0.5f;

        childPresentlid.transform.DOJump(newPos, jumpPower, jumpNum, jumpDuration);
    }

    // �v���[���g�{�b�N�X�̒���Sphere���΂�����
    void ImpulseSphere()
    {
        Flip(new Vector3(0.0f, 10.0f, 0.0f));
    }

    void DecorFall()
    {
        Rigidbody rb = decorObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void Flip(Vector3 force)
    {
        impulseSphereRb.AddForce(force, ForceMode.Impulse);
    }


    // ��������o������
    void ConfettiAction()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }

    //void HazureImageFall()
    //{
    //    hazureImgPos.y--;
    //    if(hazureImgPos.y < 0)
    //    {
    //        hazureImgPos.y = 0;
    //    }
    //}

    // �J������̃I�u�W�F�N�g���폜���邽�߂̏���(�J�������v���C���[�ɖ߂�->���̊Ԃɍ폜)
    IEnumerator DeleteBox()
    {
        vCam.Priority = 0;
        //  �v���C���[�̓t�F�[�h�i�`�悷��j
        //playerMain.SetPlayerFade(false);

        yield return new WaitForSeconds(2.0f);

        if (presentType == PRESENT_TYPE.PRESENT)
        {
            //�@PresentBoxManager�̃��X�g����폜
            presentBoxManager.RemoveOpenedPresent(this);
            presentBoxManager.RemoveRandObj();
        }
        else if (presentType == PRESENT_TYPE.HAZURE)
        {
            presentBoxManager.RemoveOpenedPresent(this);
        }

        Destroy(clickedObj);

        //  �v���C���[�͓��͏o����
        playerMain.SetPlayerInputEnable();

        //  presentBoxManager�ɕ񍐂���
        presentBoxManager.OpenBoxEnd();
    }
}
