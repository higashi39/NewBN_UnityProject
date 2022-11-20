using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionAnimation : MonoBehaviour
{
    [field: Header("My Self")]
    public static SceneTransitionAnimation instance;
    string SceneTargetName { set; get; }
    [field: SerializeField] public bool IsShowing { private set; get; }

    [field: Header("GameObject References")]
    [field: SerializeField] List<Image> imgBlackList;
    [field: SerializeField] Image imgBlackListParent;

    [field: Header("Animation Settings(Start)")]
    [field: SerializeField] float AnimationTimeStartShowImgBlack { set; get; } = 1.0f;
    [field: SerializeField] float AnimationTimeImgBlackCoverScreen { set; get; } = 0.01f;
    [field: SerializeField] Vector3 RotateTarget { set; get; }

    [field: Header("Animation Settings(End)")]
    [field: SerializeField] float AnimationTimeEndFade { set; get; } = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        imgBlackListParent.enabled = false;
        imgBlackList = new List<Image>();
        for (int i = 0; i < imgBlackListParent.transform.childCount; ++i)
        {
            Image img = imgBlackListParent.transform.GetChild(i).GetComponent<Image>();
            img.enabled = false;
            imgBlackList.Add(img);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeScene(string sceneName)
    {
        SceneTargetName = sceneName;
        IsShowing = true;
        StartCoroutine(StartChangeScene());
    }

    public void ChangeSceneSame()
    {
        StartCoroutine(StartChangeScene());
    }

    public void ShowScene()
    {
        if (IsShowing)
        {
            StartCoroutine(EndChangeScene());
        }
    }

    IEnumerator StartChangeScene()
    {
        //Init
        {
            imgBlackListParent.enabled = true;
            foreach (var img in imgBlackList)
            {
                img.transform.localScale = Vector3.zero;
                img.transform.eulerAngles = Vector3.zero;
                var nowColor = img.color;
                nowColor.a = 0;
                img.color = nowColor;
                img.enabled = true;
            }
        }

        //Show img black
        {
            foreach (var img in imgBlackList)
            {
                img.transform.DORotate(RotateTarget, AnimationTimeStartShowImgBlack).SetEase(Ease.OutCirc);
                img.transform.DOScale(Vector3.one, AnimationTimeStartShowImgBlack).SetEase(Ease.OutBack);
                img.DOFade(1, AnimationTimeStartShowImgBlack);
                yield return new WaitForSeconds(AnimationTimeImgBlackCoverScreen);
            }
            yield return new WaitForSeconds(AnimationTimeStartShowImgBlack);
        }
        yield return null;

        SceneManager.LoadScene(SceneTargetName);
    }

    IEnumerator EndChangeScene()
    {
        for (int i = 0; i < imgBlackList.Count; ++i)
        {
            int index = imgBlackList.Count - 1 - i;
            imgBlackList[index].transform.DORotate(Vector3.zero, AnimationTimeStartShowImgBlack).SetEase(Ease.OutCirc);
            imgBlackList[index].transform.DOScale(Vector3.zero, AnimationTimeStartShowImgBlack).SetEase(Ease.InBack);
            imgBlackList[index].DOFade(0, AnimationTimeStartShowImgBlack);
            yield return new WaitForSeconds(AnimationTimeImgBlackCoverScreen);
        }
        yield return new WaitForSeconds(AnimationTimeEndFade);

        yield return null;
        imgBlackListParent.enabled = false;
    }

}
