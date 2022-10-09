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

    // 制限時間関連
    [Header("Time Text")]
    [SerializeField] public Text txtPresentTime;           // プレゼントの制限時間
    [SerializeField] public float presentTimeMax;          // プレゼントの制限時間の最大
    [SerializeField] public float presentTimeNow;          // 今の時間
    public bool isTime;                             // 時間を進めるかどうか
    public float scaleChangeTime, changeSpeed;      // サイズが変わる時間 / そのスピード 
    public bool enlarge;                            // サイズを大きくするか、小さくするかのフラグ

    public bool isIndicatePresent;

    PresentGenerator presentGenerator;
    public bool presentSceneSetActive;

    // プレゼント関連
    [Header("Present")]
    public float frameRotate;                       // プレゼントが回転する値
    public float frameSize;                         // プレゼントのサイズを変更する値
    bool isRotate = true;                           // プレゼントが回転するかどうか


    //[Header("Present Box Info")]
    //public Text txtPresentInfo;

    // Start is called before the first frame update
    void Start()
    {
        presentSceneSetActive = false;

        // 各オブジェクトの表示の初期化
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

        // 制限時間の初期化
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

            // プレゼントもリセット
            presentBox.transform.localScale = Vector3.zero;

            //// クリックカウントをリセット
            //presentAction.clickCount = 0;

            StartCoroutine(TryPresent());
            presentSceneSetActive = false;
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
            IndicatePresent();
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
        // 制限時間のUIを大小させたり、色を変えたりする
        if (presentTimeNow <= 3.0f)
        {
            // UIの色の変数
            float colorR = 1.0f;
            float colorG = 1.0f;
            float colorB = 1.0f;

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
                //StartCoroutine(NotGetPresent());

                // 色も大きさも戻す
                txtPresentTime.transform.localScale = new Vector3(1, 1, 1);
                txtPresentTime.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    // プレゼント開封の処理
    public IEnumerator TryPresent()
    {
        // プレゼント画面の表示
        pnlPresentScene.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        // 各UIの表示
        pnlPresentTime.SetActive(true);
        presentBox.SetActive(true);
        presentTimeNow = presentTimeMax;       // 制限時間をMAXにする

        isIndicatePresent = true;                     // プレゼントを出現させる
        yield return new WaitForSeconds(2.0f);
        // 制限時間のスタート
        isTime = true;
    }

    // プレゼント表示の処理
    public void IndicatePresent()
    {
        // プレゼントオブジェクトのサイズを変更する
        frameSize += 0.1f;
        // 回転もさせる(最初はtrue)
        presentBox.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
        if (!isRotate)
        {
            presentBox.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            presentBox.transform.rotation *= Quaternion.AngleAxis(frameRotate, Vector3.back);
        }

        // サイズが最大まで行ったら
        if (frameSize > 8.0f)
        {
            frameSize = 8.0f;

            // 回転を止めるためにisRotateをfalseにする
            frameRotate -= 0.1f;
            if (frameRotate <= 0.0f)
            {
                frameRotate = 0.0f;
                isRotate = false;
            }
        }
    }

    // デコレーションをゲットしたときの処理
    public IEnumerator GetPresent()
    {
        isTime = false;                 // 制限時間を止める
        //newPresentBoxManager.DecorListRandom();
        presentBox.SetActive(false);
        decoration.SetActive(true);
        imgPresentDecorTransform.DOAnchorPosY(0.0f, 1.0f).SetEase(Ease.OutBounce);
        StartCoroutine(gameManager.BackGamePlay());
        //StartCoroutine(FinishOpenPresent());
        //yield return new WaitForSeconds(5.0f);
        //Debug.Log("入ってない...");
        //pnlPresentTime.SetActive(false);
        //pnlPresentScene.SetActive(false);
        yield return null;
    }
}
