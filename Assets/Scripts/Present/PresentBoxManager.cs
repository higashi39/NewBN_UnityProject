using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PresentBoxManager : MonoBehaviour
{
    [Header("Present")]
    List<PresentBox> presentList;
    int[] randomNumArr;

    [Header("References Script")]
    GameManager gameManager;

    [Header("Present Box Info")]
    public Text txtPresentInfo;

    [Header("in PresentBox")]
    public GameObject[] decorPrefabs;
    [SerializeField] List<GameObject> decorList = new List<GameObject>();

    //// ハズレ画像
    //[Header("in HazureImage")]
    //public GameObject hazureImg;
    //RectTransform hazureRtm;
    //[SerializeField] public Vector3 hazureImgPos;
    //[SerializeField] public bool isHazureImgFall;

    [HideInInspector] public GameObject randObj;
    private int rand;

    // デコレーションを指定した位置に飾るための変数
    [SerializeField] public string decorName;
    // 座標
    [SerializeField] public Vector3 newDecorPos = new Vector3();
    // 大きさ
    [SerializeField] public Vector3 newDecorScale = new Vector3();

    Rigidbody rb;

    void Start()
    {
        txtPresentInfo.gameObject.SetActive(false);
        // プレゼントの中身に入れる飾りのプレハブをリストに追加
        foreach (GameObject item in decorPrefabs)
        {
            decorList.Add(item);
        }

        //hazureRtm = hazureImg.GetComponent<RectTransform>();
        //hazureImgPos = hazureRtm.anchoredPosition;
        //isHazureImgFall = false;
    }


    //　PresentBoxManagerをInitする
    //  randomNumArrの準備とpresentListをnewする
    //  プレゼント数UIも更新する
    public void Init(int presentTotal)
    {
        presentList = new List<PresentBox>();

        randomNumArr = new int[presentTotal];
        for (int i = 0; i < presentTotal; ++i)
        {
            randomNumArr[i] = i;
        }

        gameManager = FindObjectOfType<GameManager>();
        gameManager.UpdateUIPresentLeft(presentTotal);
    }

    //　残りのプレゼント数
    public int GetAvailablePresent()
    {
        return presentList.Count;
    }

    //　プレイヤーのスキル発動
    public void PlayerUseSkill(int boxNum)
    {
        int availablePresentNum = GetAvailablePresent();

        //PresentBoxの数はmaxOpenBoxAtOnce以下場合
        //Random処理はいらない
        if (boxNum == availablePresentNum)
        {
            for (int i = 0; i < boxNum; ++i)
            {
                //Debug.Log("Open Box No : " + i);
                presentList[i].MakeBoxShiny();
            }
            return;
        }

        //Random Number
        for (int i = 0; i < availablePresentNum; ++i)
        {
            randomNumArr[i] = i;
        }

        //Swap number
        for (int i = 0; i < availablePresentNum; ++i)
        {
            int tmpNum = randomNumArr[i];
            int randomIndex = Random.Range(0, availablePresentNum);
            randomNumArr[i] = randomNumArr[randomIndex];
            randomNumArr[randomIndex] = tmpNum;
        }
        //Make box shiny
        for (int i = 0; i < boxNum; ++i)
        {
            presentList[randomNumArr[i]].MakeBoxShiny();
        }
    }

    //  開いたプレゼントをpresentListから削除
    public void RemoveOpenedPresent(PresentBox present)
    {
        presentList.Remove(present);
    }

    //　presentListにPresentBoxの参照を追加する
    public void AddPresentListRef(PresentBox present)
    {
        presentList.Add(present);
    }

    //  箱のアニメションが始めるとき
    public void OpenBoxStart()
    {
        //  ゲームステータスは GAME_OPEN_GIFT　にする（時間を止める）
        gameManager.ChangeGameStatus(GameManager.GAME_STATUS.GAME_PRESENT_APPEAR);
        gameManager.SetEnableAllUI(false);
    }

    //　箱のアニメション終了時
    public void OpenBoxEnd()
    {
        //  ゲームステータスは GAME_NORMAL　にする
        gameManager.ChangeGameStatus(GameManager.GAME_STATUS.GAME_NORMAL);
        gameManager.UpdateUIPresentLeft(GetAvailablePresent());
        gameManager.SetEnableAllUI();
    }

    public void ShowUIBoxInfo()
    {
        StartCoroutine(UIBoxInfoAnimation());
    }

    IEnumerator UIBoxInfoAnimation()
    {
        txtPresentInfo.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        txtPresentInfo.gameObject.SetActive(false);
    }

    // 出てくるデコレーションをランダムで選出
    public void DecorListRandom()
    {
        rand = Random.Range(0, decorList.Count);

        randObj = decorList[rand];
        rb = randObj.GetComponent<Rigidbody>();
        Debug.Log(randObj);
    }

    public void SetDecorTranslate()
    {
        decorName = randObj.name;


        if (decorName == "OrangePlant")
        {
            newDecorPos.x = 36.0f;
            newDecorPos.y = 3.0f;
            newDecorPos.z = 48.0f;

            newDecorScale.x = 120.0f;
            newDecorScale.y = 120.0f;
            newDecorScale.z = 120.0f;
        }
        else if (decorName == "BluePlant")
        {
            newDecorPos.x =40.0f;
            newDecorPos.y = 2.7f;
            newDecorPos.z = 48.0f;

            newDecorScale.x = 120.0f;
            newDecorScale.y = 120.0f;
            newDecorScale.z = 120.0f;
        }
        else if (decorName == "EggStool")
        {
            newDecorPos.x = 24.0f;
            newDecorPos.y = 0.0f;
            newDecorPos.z = 48.0f;

            newDecorScale.x = 8.0f;
            newDecorScale.y = 8.0f;
            newDecorScale.z = 8.0f;
        }
        else if (decorName == "EggTable")
        {
            newDecorPos.x = 20.0f;
            newDecorPos.y = 0.0f;
            newDecorPos.z = 44.0f;

            newDecorScale.x = 9.0f;
            newDecorScale.y = 9.0f;
            newDecorScale.z = 9.0f;
        }
        else if (decorName == "Vane")
        {
            newDecorPos.x = 25.0f;
            newDecorPos.y = 0.0f;
            newDecorPos.z = 42.0f;

            newDecorScale.x = 5.0f;
            newDecorScale.y = 5.0f;
            newDecorScale.z = 5.0f;
        }
        else if (decorName == "flower07")
        {
            newDecorPos.x = 30.0f;
            newDecorPos.y = 6.0f;
            newDecorPos.z = 48.0f;

            newDecorScale.x = 120.0f;
            newDecorScale.y = 120.0f;
            newDecorScale.z = 120.0f;
        }
        else if (decorName == "Flower1")
        {
            newDecorPos.x = 41.0f;
            newDecorPos.y = 0.0f;
            newDecorPos.z = 41.5f;

            newDecorScale.x = 2.0f;
            newDecorScale.y = 2.0f;
            newDecorScale.z = 2.0f;
        }
        else if (decorName == "Flower2")
        {
            newDecorPos.x = 35.0f;
            newDecorPos.y = 1.5f;
            newDecorPos.z = 41.5f;

            newDecorScale.x = 2.0f;
            newDecorScale.y = 2.0f;
            newDecorScale.z = 2.0f;
        }
    }

    public void ChangeUseGravity()
    {
        rb.useGravity = true;
    }

    public void RemoveRandObj()
    {
        decorList.RemoveAt(rand);
    }

    void Update()
    {
        //if (isHazureImgFall)
        //{
        //   HazureImageFall();
        //}
    }

    //public void HazureImageFall()
    //{
    //    hazureImgPos.y--;

    //    if (hazureImgPos.y < 0)
    //    {
    //        hazureImgPos.y = 0;
    //    }
    //    hazureRtm.anchoredPosition = hazureImgPos;
    //    Debug.Log(hazureImgPos);
    //}

}
