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

    // 制限時間関連
    [Header("Time Text")]
    [SerializeField] public Text txtPresentTime;           // プレゼントの制限時間
    [SerializeField] public float presentTimeMax;          // プレゼントの制限時間の最大
    [SerializeField] public float presentTimeNow;          // 今の時間
    public bool isTime;                             // 時間を進めるかどうか
    public float scaleChangeTime, changeSpeed;      // サイズが変わる時間 / そのスピード 
    public bool enlarge;                            // サイズを大きくするか、小さくするかのフラグ
    // 制限時間のUIの色の変数
    public float colorR = 1.0f;
    public float colorG = 1.0f;
    public float colorB = 1.0f;


    public bool isIndicatePresent;

    public bool presentSceneSetActive;


    // プレゼント関連
    [Header("Present")]
    //public float frameRotate;                       // プレゼントが回転する値
    //public float frameSize;                         // プレゼントのサイズを変更する値
    //bool isRotate = true;                           // プレゼントが回転するかどうか

    public bool isCalledTimeUpAnimation = false;           // 時間が終わったらプレゼントゲットしない時、

    // 紙吹雪用パーティクルシステム
    public ParticleSystem[] particles;

    // プレゼントリスト関連
    [Header("Present List")]
    [SerializeField] public List<GameObject> presentList;       // プレゼントリスト
    private GameObject presentRandomObj;                        // 選ばれたプレゼントがリストの何番目か      
    //private int presentChoiceNum;                             

    //------------------------------------------------------
    // 221113 プレゼント登場方法の仕方
    // エラーがでるかもなので、いったん囲っておく
    //Transformをキャッシュしておく。
    Transform tf;

    //【任意の値】モーションの長さ。
    [Header("モーションの長さ(持続時間)")]
    [SerializeField]
    float duration = 0.8f;

    //【任意の値】アクティブ化されてからモーション開始するまでの遅延。
    [Header("モーション開始までのディレイ")]
    [SerializeField]
    [Range(0, 10.0f)]
    float delay = 0;

    //【任意の値】最小の大きさ。
    [Header("最小Scale(モーション開始時のサイズ)")]
    [SerializeField]
    Vector3 scaleMin = new Vector3(0, 0, 0);
    //【任意の値】最大の大きさ。UIのデフォルトScale * 1.3ぐらいの大きさにすると収まりが良くなる。
    [Header("最大Scale(デフォルトScale * 1.3程度の値を推奨)")]
    [SerializeField]
    Vector3 scaleMax = new Vector3(1.3f, 1.3f, 1.3f);

    //デフォルトScaleを記憶。
    Vector3 defaultScale;

    //コルーチン管理用。
    Coroutine popout;

    //Sinの曲線を良き所まで使用。
    static readonly float Modifier = Mathf.PI * 0.725f;

    //経過時間。
    float elapsedTime;

    //WaitForSecondsでも毎フレームnewすると良くないらしいので、キャッシュしておく。
    WaitForSeconds delayWait;
    //ポーズ中(TimeScale = 0の時)に使用する場合は、WaitForSecondsRealtimeを使う。
    //    WaitForSecondsRealtime delayWait;
    //------------------------------------------------------

    // デコレーション関連
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
        // 初期化しないといけないので、先にプレゼントを指定しておく
        presentBox = GameObject.Find("Present_Pinata");
        tf = presentBox.transform;
        tf.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        defaultScale = tf.localScale;


        if (delay != 0)
        {
            delayWait = new WaitForSeconds(delay);

            //ポーズ中に使用する場合は、WaitForSecondsRealtimeを使う。
            //            delayWait = new WaitForSecondsRealtime(delay);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        presentSceneSetActive = false;

        // 各オブジェクトの表示の初期化
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

        // 制限時間の初期化
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
                // 元々持っていたデコレーションを手放す
                decorationUsedList.Add(decorationRandomObj);
                decorationChoiceNum = decorationList.IndexOf(decorationRandomObj);
                decorationList.RemoveAt(decorationChoiceNum);
            }

            // バグ防止用に、デコレーションリストの中が０になったら
            // 一度出たデコレーションをリセットしてリストに入れなおす
            if (decorationList.Count == 0)
            {
                int useCountPrev = decorationUsedList.Count;
                for (int i = 0; i < useCountPrev; ++i)
                {
                    decorationList.Add(decorationUsedList[0]);
                    decorationUsedList.RemoveAt(0);
                }
            }

            //// クリックカウントをリセット
            //presentAction.clickCount = 0;

            // どのプレゼントが出るか
            presentRandomObj = presentList[Random.Range(0, presentList.Count)];
            //presentChoiceNum = presentList.IndexOf(presentRandomObj);
            //presentList.RemoveAt(presentChoiceNum);
            presentBox = presentRandomObj;
            // プレゼントもリセット
            //presentBox.transform.localScale = Vector3.zero;
            tf = presentBox.transform;
            tf.localScale = new Vector3(0.0f, 0.0f, 0.0f);

            StartCoroutine(TryPresent());
            presentSceneSetActive = false;
            isCalledTimeUpAnimation = false;
        }

        // 制限時間の処理
        OpenPresentTimeText();

        // 制限時間を表示するフラグが立ったら
        if (isTime)
        {
            // 制限時間の処理を行う
            TimeStart();
        }

        if (isIndicatePresent)
        {
        }
    }

    // PresentTimeの書式
    void OpenPresentTimeText()
    {
        string timeformat = "Time:" + presentTimeNow.ToString("0.00");
        txtPresentTime.text = timeformat;
    }

    // 制限時間の処理
    void TimeStart()
    {
        // 制限時間を減らす
        presentTimeNow -= Time.deltaTime;
        // 一定数制限時間が迫ったら
        if (isIndicatePresent)
        {
            // 制限時間のUIを大小させたり、色を変えたりする
            if (presentTimeNow <= 3.0f)
            {

                // 変更のスパン
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
                // UIの色の表示
                txtPresentTime.color = new Color(colorR, colorG, colorB);

                // 制限時間が0秒になったら
                if (presentTimeNow <= 0.0f)
                {
                    // --------------------------
                    // 開封時間終了(ゲームに戻る)
                    // --------------------------
                    presentTimeNow = 0.0f;
                    if (!isCalledTimeUpAnimation)
                    {
                        isCalledTimeUpAnimation = true;
                        StartCoroutine(NotGetPresent());
                    }

                    // 色も大きさも戻す
                    txtPresentTime.transform.localScale = new Vector3(1, 1, 1);
                    txtPresentTime.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    // プレゼント開封の処理
    public IEnumerator TryPresent()
    {
        Debug.Log("TryPresent に入った");
        // プレゼント画面の表示
        pnlPresentScene.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        // 各UIの表示
        pnlPresentTime.SetActive(true);
        presentBox.SetActive(true);
        presentTimeNow = presentTimeMax;       // 制限時間をMAXにする

        //IndicatePresent();
        //多重起動防止。        
        if (popout != null)
        {
            StopCoroutine(popout);
        }

        popout = StartCoroutine(Popout());

        isIndicatePresent = true;                     // プレゼントを出現させる
        yield return new WaitForSeconds(2.0f);
        // 制限時間のスタート
        isTime = true;
    }

    //// プレゼント表示の処理
    //public void IndicatePresent()
    //{
    //    // プレゼントオブジェクトのサイズを変更する
    //    frameSize += 0.1f;
    //    // 回転もさせる(最初はtrue)
    //    presentBox.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
    //    if (!isRotate)
    //    {
    //        //*****************************************************
    //        //最後はカメラの向きに戻ってください(もしカメラの回転アニメーションがあれば)
    //        //いま、カメラはUIのLayerがScreenSpace-Cameraので注意
    //        //*****************************************************
    //        //presentBox.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {
    //        presentBox.transform.rotation *= Quaternion.AngleAxis(frameRotate, Vector3.back);
    //    }

    //    // サイズが最大まで行ったら
    //    if (frameSize > 5.3f)
    //    {
    //        frameSize = 5.3f;

    //        // 回転を止めるためにisRotateをfalseにする
    //        frameRotate -= 0.1f;
    //        if (frameRotate <= 0.0f)
    //        {
    //            frameRotate = 0.0f;
    //            isRotate = false;
    //        }
    //    }
    //}

    // デコレーションをゲットしたときの処理
    public IEnumerator GetPresent()
    {
        IsSuccessOpeningPresentBefore = true;
        // どのデコレーションが出るか
        decorationRandomObj = decorationList[Random.Range(0, decorationList.Count)];
        decoration = decorationRandomObj;
        imgPresentDecorTransform = decoration.GetComponent<RectTransform>();
        // プレゼントもリセット
        //presentBox.transform.localScale = Vector3.zero;

        SfxPlayPresentGet.PlaySFXSound();
        Debug.Log("GetPresentに入った");
        isTime = false;                 // 制限時間を止める
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
        //Debug.Log("入ってない...");
        //pnlPresentTime.SetActive(false);
        //pnlPresentScene.SetActive(false);
        yield return null;
    }

    public IEnumerator NotGetPresent()
    {
        IsSuccessOpeningPresentBefore = false;
        SfxPlayPresentMiss.PlaySFXSound();
        Debug.Log("NotGetPresentに入った");
        //RectTransform notGetPresent = newPresentBoxManager.presentBox.GetComponent<RectTransform>();
        //float oriPosY = imgPresent.transform.position.y;

        //Debug.Log("プレゼントを開くことができなかった！");
        ////imgPresent.transform.DOShakeScale(2.0f, 0.15f, 4, 20);
        ////yield return new WaitForSeconds(2.0f);
        ////notGetPresent.DOAnchorPosY(oriPosY, 1.25f).SetEase(Ease.InBack);
        //yield return new WaitForSeconds(1.25f);
        //Debug.Log("入りました！");

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
            //ポーズ中に使用する場合は、deltaTimeでなくunscaledDeltaTimeを使う。
            //            elapsedTime += Time.unscaledDeltaTime;


            tf.localScale = Vector3.Lerp(scaleMin, scaleMax, Mathf.Sin(elapsedTime / duration * Modifier));


            if (duration <= elapsedTime)
            {
                //通常のスケールに戻しておく。
                tf.localScale = new Vector3(5.3f, 5.3f, 5.3f);


                popout = null;
                yield break;
            }
            yield return null;
        }
    }
}
