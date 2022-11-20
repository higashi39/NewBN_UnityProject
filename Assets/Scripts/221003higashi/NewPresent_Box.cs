using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//=========================================================
// 紐を解いて開けるプレゼント
//=========================================================
public class NewPresent_Box : MonoBehaviour
{
    [Header("References")]
    GameManager gameManager;
    NewPresentBoxManager newPresentBoxManager;
    //PresentAction presentAction;

    Vector2 mouseStart;     // マウスをクリックした場所
    Vector2 mouseStop;      // マウスを離した場所
    float mouseAngle;       // マウスをドラッグした角度
    float mouseLength;      // マウスをドラッグした距離

    [Header("if Open Present")]
    [SerializeField] public float minMouseLength = 150.0f;      // ドラッグしてほしい最短距離
    [SerializeField] public float minAngle = 90.0f;            // ドラッグしてほしい最低角度
    [SerializeField] public float maxAngle = 170.0f;            // ドラッグしてほしい最高角度



    private void Awake()
    {
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    { }
    //    // マウスのドラッグ距離を取得できたので、
    //    // それくらい引っ張ったかでプレゼント開けれるかどうかをする
    //    if (mouseLength >= 300.0f)
    //    {
    //        Debug.Log("プレゼントを開けることができたよ！");
    //        Debug.Log(mouseLength);
    //    }
    //    // 距離は200前後が妥当かな...
    //    else if (mouseLength < 200.0f && mouseLength > 0)
    //    {
    //        Debug.Log("開けることができなかった...");
    //        Debug.Log(mouseLength);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (newPresentBoxManager.presentTimeNow < 5.0f && newPresentBoxManager.presentTimeNow > 0.0f)
        {
            // マウスドラッグの距離を取得する
            if (Input.GetMouseButtonDown(0))
            {
                mouseStart = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseStop = Input.mousePosition;
                mouseLength = (mouseStop - mouseStart).magnitude;
                mouseAngle = GetAngle(mouseStart, mouseStop);
                Debug.Log(mouseAngle);
            }
        }

        // マウスのドラッグ距離とその角度からプレゼントを開けられるかどうか
        if (mouseLength >= minMouseLength)
        {
            mouseLength = 0.0f;
            //if ((mouseAngle > minAngle && mouseAngle <= maxAngle) ||
            //    (mouseAngle > -maxAngle && mouseAngle <= -minAngle))
            if (mouseAngle > minAngle && mouseAngle <= maxAngle)
            {
                // プレゼントが開く
                Debug.Log("プレゼントが開く");
                StartCoroutine(newPresentBoxManager.GetPresent());
            }

        }



    }


    // マウスをドラッグしたときの２点の角度を取得する
    float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        float rad = Mathf.Atan2(dir.x, dir.y);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

}