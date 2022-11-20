using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleButton : MonoBehaviour
{
    [Header("Canvas / UI")]
    [SerializeField] GameObject pnlMainMenu;
    [SerializeField] Button btnStart;
    [SerializeField] Text txtTitle;

    [field: Header("Camera Property")]
    [field: SerializeField] CinemachineVirtualCamera cam;
    [field: SerializeField] CinemachineTrackedDolly dollyComponent;
    [field: SerializeField] CinemachineSmoothPath dollyTrack;
    [field: SerializeField] float CamMoveSpeed { set; get; }

    [field: Header("Animation Time(Start)")]
    [field: SerializeField] float AnimationTxtTitleTime { set; get; } = 1.25f;
    [field: SerializeField] float AnimationBtnStartTime { set; get; } = 1.25f;
    [field: SerializeField] int DollyTrackStopStart { set; get; } = 3;

    [field: Header("Animation Time(To Play Scene)")]
    [field: SerializeField] float AnimationBtnStartTimeScale { set; get; } = 1.25f;
    [field: SerializeField] float AnimationTxtTitleTimeFade { set; get; } = 1.0f;
    [field: SerializeField] float AnimationTimeWaitAfterCameraMove { set; get; } = 0.75f;
    [field: SerializeField] float AnimationGyaaWaitBeforeMove { set; get; } = 0.5f;
    [field: SerializeField] float AnimationGyaaMoveTime { set; get; } = 1.5f;

    [field: Header("Character")]
    [field: SerializeField] GameObject PlayerGyaa { set; get; }
    [field: SerializeField] Animator PlayerGyaaAnim { set; get; }
    [field: SerializeField] GameObject PlayerGyaaMoveTarget { set; get; }
    [field: SerializeField] Vector3 PlayerGyaaRotationTarget { set; get; }


    private void Awake()
    {
        dollyComponent = cam.GetCinemachineComponent<CinemachineTrackedDolly>();
        PlayerGyaaAnim = PlayerGyaa.GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneTransitionAnimation.instance.ShowScene();
        StartCoroutine(GameStartAnimation());
    }

    IEnumerator GameStartAnimation()
    {
        //Set UI Non Active
        txtTitle.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(false);

        //Camera Animation
        float trackPath = 0.0f;
        do
        {
            trackPath += CamMoveSpeed;
            dollyComponent.m_PathPosition = trackPath;
            yield return null;
        }
        while (trackPath < DollyTrackStopStart);
        dollyComponent.m_PathPosition = DollyTrackStopStart;

        yield return new WaitForSeconds(1.0f);

        Vector3 sizeTarget = Vector3.one;
        Vector3 sizeStart = Vector3.zero;
        //show title
        {
            txtTitle.transform.localScale = sizeStart;
            txtTitle.gameObject.SetActive(true);
            txtTitle.transform.DOScale(sizeTarget, AnimationTxtTitleTime).SetEase(Ease.OutElastic);
            yield return new WaitForSeconds(AnimationTxtTitleTime);

        }

        //show start button
        {
            btnStart.interactable = false;
            btnStart.transform.localScale = sizeStart;
            btnStart.gameObject.SetActive(true);
            btnStart.transform.DOScale(sizeTarget, AnimationTxtTitleTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(AnimationTxtTitleTime);
            btnStart.interactable = true;
            var btnAnimationScript = btnStart.GetComponent<ButtonAnimation>();
            btnAnimationScript.IsCanAnimation = true;
        }
    }

    IEnumerator ChangeSceneToScenePlay()
    {
        ButtonAnimation btnStartAnimation = btnStart.GetComponent<ButtonAnimation>();
        btnStartAnimation.enabled = false;
        btnStart.interactable = false;

        //Btn Scale
        {
            btnStart.transform.DOScale(Vector3.zero, AnimationBtnStartTimeScale).SetEase(Ease.InBack);
            yield return new WaitForSeconds(AnimationBtnStartTimeScale);
            btnStart.gameObject.SetActive(false);
        }

        //Text Fade
        {
            txtTitle.DOFade(0, AnimationTxtTitleTimeFade);
            yield return new WaitForSeconds(AnimationTxtTitleTimeFade);
            txtTitle.gameObject.SetActive(false);
        }
        yield return null;


        //Move camera to house
        {
            //Camera Animation
            float trackPathMax = dollyTrack.m_Waypoints.Length - 1;
            float trackPath = dollyComponent.m_PathPosition;
            do
            {
                trackPath += CamMoveSpeed;
                dollyComponent.m_PathPosition = trackPath;
                yield return null;
            }
            while (trackPath < trackPathMax);
            dollyComponent.m_PathPosition = trackPathMax;
            yield return new WaitForSeconds(AnimationTimeWaitAfterCameraMove);
        }

        //Gyaa go inside house
        {
            PlayerGyaa.transform.eulerAngles = PlayerGyaaRotationTarget;
            yield return new WaitForSeconds(AnimationGyaaWaitBeforeMove);
            PlayerGyaaAnim.SetBool("isWalk", true);

            PlayerGyaa.transform.DOMove(PlayerGyaaMoveTarget.transform.position, AnimationGyaaMoveTime);
            yield return new WaitForSeconds(AnimationGyaaMoveTime);

            PlayerGyaaAnim.SetBool("isWalk", false);
            yield return null;
        }

        //Scene Transition
        {
            SceneTransitionAnimation.instance.ChangeScene("PlayScene");
        }
    }

    public void PushStart()
    {
        StartCoroutine(ChangeSceneToScenePlay());
    }
}
