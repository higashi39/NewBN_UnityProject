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

    // クリックした時に箱を揺らす用変数
    public float shakeDuration;
    public float strength;
    public int vibrato;
    public float randomness;
    public bool snapping;
    public bool fadeOut;

    // 紙吹雪用パーティクルシステム
    public ParticleSystem[] particles;

    GameManager gameManager;

    //// プレゼント関連
    //[Header("Present")]
    //public float frameRotate;                       // プレゼントが回転する値
    //public float frameSize;                         // プレゼントのサイズを変更する値
    //bool isRotate = true;                           // プレゼントが回転するかどうか

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

        //// 各オブジェクトの表示の初期化
        //pnlPresentScene.SetActive(false);
        //imgPresent.SetActive(false);
        //pnlPresentTime.SetActive(false);

        //// 制限時間の初期化
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
        //        // デコレーションゲット
        //        // ----------------
        //        isClicked = true;
        //    }
        //    else
        //    {
        //        // ----------------------------
        //        // デコレーションをゲットできなかった
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

    //// プレゼント開封の処理
    //public IEnumerator TryPresent()
    //{
    //    // プレゼント画面の表示
    //    newPresentBoxManager.pnlPresentScene.SetActive(true);
    //    yield return new WaitForSeconds(1.0f);
    //    // 各UIの表示
    //    newPresentBoxManager.pnlPresentTime.SetActive(true);
    //    newPresentBoxManager.presentBox.SetActive(true);
    //    newPresentBoxManager.presentTimeNow = newPresentBoxManager.presentTimeMax;       // 制限時間をMAXにする

    //    isIndicatePresent = true;                     // プレゼントを出現させる
    //    yield return new WaitForSeconds(2.0f);
    //    // 制限時間のスタート
    //    newPresentBoxManager.isTime = true;
    //}

    //// プレゼント表示の処理
    //public void IndicatePresent()
    //{
    //    // プレゼントオブジェクトのサイズを変更する
    //    frameSize += 0.01f;
    //    // 回転もさせる(最初はtrue)
    //    newPresentBoxManager.presentBox.transform.localScale = new Vector3(frameSize, frameSize, frameSize);
    //    if (!isRotate)
    //    {
    //        newPresentBoxManager.presentBox.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {
    //        newPresentBoxManager.presentBox.transform.rotation *= Quaternion.AngleAxis(frameRotate, Vector3.back);
    //    }

    //    // サイズが最大まで行ったら
    //    if (frameSize > 1.0f)
    //    {
    //        frameSize = 1.0f;

    //        // 回転を止めるためにisRotateをfalseにする
    //        frameRotate -= 0.1f;
    //        if (frameRotate <= 0.0f)
    //        {
    //            frameRotate = 0.0f;
    //            isRotate = false;
    //        }
    //    }
    //}


    //// デコレーションをゲットしたときの処理
    //public IEnumerator GetPresent()
    //{
    //    newPresentBoxManager.isTime = false;                 // 制限時間を止める
    //    yield return new WaitForSeconds(0.1f);
    //    //newPresentBoxManager.DecorListRandom();
    //    RectTransform imgPresentDecorTransform = newPresentBoxManager.decoration.GetComponent<RectTransform>();
    //    imgPresentDecorTransform.DOAnchorPosY(0.0f, 1.0f).SetEase(Ease.OutBounce);
    //    StartCoroutine(newPresentBoxManager.FinishOpenPresent());
    //}

    //// デコレーションをゲットできなかったときの処理
    //IEnumerator NotGetPresent()
    //{
    //    //RectTransform notGetPresent = newPresentBoxManager.presentBox.GetComponent<RectTransform>();
    //    //float oriPosY = imgPresent.transform.position.y;

    //    Debug.Log("プレゼントを開くことができなかった！");
    //    //imgPresent.transform.DOShakeScale(2.0f, 0.15f, 4, 20);
    //    //yield return new WaitForSeconds(2.0f);
    //    //notGetPresent.DOAnchorPosY(oriPosY, 1.25f).SetEase(Ease.InBack);
    //    yield return new WaitForSeconds(1.25f);
    //    Debug.Log("入りました！");

    //    newPresentBoxManager.pnlPresentScene.SetActive(false);
    //    newPresentBoxManager.pnlPresentTime.SetActive(false);
    //    StartCoroutine(gameManager.BackGamePlay());
    //    yield return new WaitForSeconds(1.0f);

    //}
}
