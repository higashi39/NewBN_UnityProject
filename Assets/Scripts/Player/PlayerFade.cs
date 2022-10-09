using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFade : MonoBehaviour
{
    public AnimationCurve fadingCurve;

    List<Material> listMaterial;

    [SerializeField] bool isFading;
    [SerializeField] bool isFadeProcessDone;

    [Header("Fading Setting")]
    [SerializeField] float fadeDuration = 2.0f;    //1回のフェード処理は何秒がかかる
    [SerializeField] float fadeDurationNow;

    // Start is called before the first frame update
    void Start()
    {
        listMaterial = new List<Material>();
        var allMat = GetComponentsInChildren<MeshRenderer>();
        int allMatNum = allMat.Length;
        listMaterial.Add(GetComponent<MeshRenderer>().material);
        for (int i = 0; i < allMatNum; ++i)
        {
            listMaterial.Add(allMat[i].material);
        }


        isFadeProcessDone = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFadeProcessDone) return;
        fadeDurationNow += isFading ? Time.deltaTime : -Time.deltaTime;
        fadeDurationNow = Mathf.Clamp(fadeDurationNow, 0.0f, fadeDuration);
        if (fadeDurationNow <= 0 || fadeDurationNow >= fadeDuration) isFadeProcessDone = true;
        for (int i = 0; i < listMaterial.Count; ++i)
        {
            Color fadeMatColor = listMaterial[i].color;
            fadeMatColor.a = fadingCurve.Evaluate(fadeDurationNow / fadeDuration);
            listMaterial[i].color = fadeMatColor;
        }
    }

    //  プレイヤーはフェードかどうかの設定関数
    public void SetPlayerFading(bool isFade = true)
    {
        isFading = isFade;
        isFadeProcessDone = false;
    }

}
