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

    // クリックしたオブジェクトを PlayerClickBox.cs のクリック処理で取得
    [HideInInspector] public GameObject clickedObj;
    // 取得したオブジェクトの CinemachineVirtualCamera を入れる変数
    CinemachineVirtualCamera vCam;

    // デコレーションをクリックされたプレゼントオブジェクトの子にする
    GameObject decorObject;

    // プレゼントオブジェクトの子(BoxandLid)と孫(PresentLid)を入れる用変数
    public GameObject childPresentlid;
    public GameObject childBoxandlid;

    // 箱を揺らす用変数
    public float shakeDuration;
    public float strength;
    public int vibrato;
    public float randomness;
    public bool snapping;
    public bool fadeOut;

    // プレゼントオブジェクトの中のボール（ふたが開いた瞬間真上に飛ぶ）用
    [SerializeField] private GameObject impulseSphere;
    private Rigidbody impulseSphereRb;       // Rigidbodyの取得

    // 箱のふたを飛ばす用変数
    [HideInInspector] public GameObject child;
    public float jumpPower;
    public int jumpNum;
    public float jumpDuration;

    // 紙吹雪用パーティクルシステム
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
            Debug.Log("Too far from the box（プレゼントから遠すぎる）");
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
        Debug.Log("Open the box(プレゼントを開く)");

        //  スキルがまだ再生したらOffにする
        TurnOffLight();

        //プレイヤーをフェードする
        //playerMain.SetPlayerFade();
        //  プレイヤーは入力出来できない
        playerMain.SetPlayerInputEnable(false);
        //playerMain.AddPlayerScore(boxScore);


        //  presentBoxManagerに報告する
        presentBoxManager.OpenBoxStart();

        // プレゼントが開かれることになったら、 clickedObj の子のオブジェクト（ Cinemachine を持つ）を取得
        // その子オブジェクトのコンポーネント CinemachineVirtualCamera を変数に入れる
        vCam = clickedObj.GetComponentInChildren<CinemachineVirtualCamera>();
        // クリックされたプレゼントの Priority(カメラ優先順位を決めるもの) をプレイヤーよりも大きい値にする
        vCam.Priority = 20;

        // クリックされた子オブジェクトの取得)
        // 「BoxandLid」(プレゼントの箱とふた)を取得（揺らすため）
        childBoxandlid = transform.Find("BoxandLid").gameObject;
        // 「PresentLid」(プレゼントのふた)を取得（飛ばすため）
        childPresentlid = childBoxandlid.transform.Find("PresentLid").gameObject;

        // プレゼントオブジェクトに入っているImpulseSphereを取得
        impulseSphere = transform.Find("ImpulseSphere").gameObject;
        impulseSphereRb = impulseSphere.GetComponent<Rigidbody>();

        if (presentType == PRESENT_TYPE.PRESENT)
        {
            // プレゼントの中身を決めるランダム処理
            presentBoxManager.DecorListRandom();

            CreateRandObj();
            //Debug.Log(presentBoxManager.randObj);

            // プレゼントを開ける時の処理
            StartCoroutine(AtariboxAction());
        }
        else if (presentType == PRESENT_TYPE.HAZURE)
        {
            // クリックしたオブジェクトのimageを取得する
            //hazureImageManager.LoadThisImage(this);

            // クリックしたオブジェクトのCanvas と image に付いているスクリプトを取得
            //hazureCanvas = hazureImageManager.clickedCanvas;
            //hazureImage = hazureImageManager.clickedHazereImage;

            //changeSize = hazureImageManager.clickedHazere.GetComponent<ChangeSize>();
            // ハズレプレゼントのアニメーション
            StartCoroutine(HazureboxAction());
        }


        //// プレゼントを開ける時の処理
        //StartCoroutine(giftboxAction());
    }

    //プレゼント箱をキラキラさせる関数
    public void MakeBoxShiny()
    {
        TurnOnLight();
    }

    //更新処理関数にisOnSkillをチェックする関数
    //isOnSkillがtrue場合は falseまで処理をする（light・光をon→off→on→...）
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

    //Light・光をOnにする関数
    void TurnOnLight()
    {
        shineTimeNow = 0.0f;
        shinyLight.enabled = true;
        isOnSkill = true;
    }

    //Light・光をOffにする関数
    void TurnOffLight()
    {
        shinyLight.enabled = false;
        isOnSkill = false;
    }

    // プレゼントの中身を決める処理（現在、重複なし）
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

    // 当たりのプレゼントを開く時の処理
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

    // 箱を揺らす処理
    void ShakeBox()
    {
        // duration   : 時間
        // strength   : 揺れの強さ
        // vibrato    : どのくらい振動するか
        // randomress : ランダム度合(0 ~ 180)
        // fadeOut    : フェードアウトするか 
        childBoxandlid.transform.DOShakePosition(shakeDuration, strength, vibrato, randomness, snapping, fadeOut);
    }

    // 箱のふた飛ばす処理
    void FlyBoxLid()
    {
        // jumpPower    : ジャンプする力量
        // jumpNum      : 何回ジャンプするか
        // jumpDuration : ジャンプする時間 
        //childPresentlid.transform.DOJump(new Vector3(2, 0, 2), jumpPower, jumpNum, jumpDuration);

        Vector3 newPos = childPresentlid.transform.position;
        const float flyDistance = 2.0f;
        float randomRadian = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        newPos.x += flyDistance * Mathf.Sin(randomRadian);
        newPos.z += flyDistance * Mathf.Cos(randomRadian);
        newPos.y = -0.5f;

        childPresentlid.transform.DOJump(newPos, jumpPower, jumpNum, jumpDuration);
    }

    // プレゼントボックスの中のSphereを飛ばす処理
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


    // 紙吹雪を出す処理
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

    // 開けた後のオブジェクトを削除するための処理(カメラをプレイヤーに戻す->その間に削除)
    IEnumerator DeleteBox()
    {
        vCam.Priority = 0;
        //  プレイヤーはフェード（描画する）
        //playerMain.SetPlayerFade(false);

        yield return new WaitForSeconds(2.0f);

        if (presentType == PRESENT_TYPE.PRESENT)
        {
            //　PresentBoxManagerのリストから削除
            presentBoxManager.RemoveOpenedPresent(this);
            presentBoxManager.RemoveRandObj();
        }
        else if (presentType == PRESENT_TYPE.HAZURE)
        {
            presentBoxManager.RemoveOpenedPresent(this);
        }

        Destroy(clickedObj);

        //  プレイヤーは入力出来る
        playerMain.SetPlayerInputEnable();

        //  presentBoxManagerに報告する
        presentBoxManager.OpenBoxEnd();
    }
}
