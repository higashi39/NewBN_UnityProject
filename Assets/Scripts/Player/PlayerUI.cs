using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [field: Header("UI Energy")]
    [field: SerializeField] GameObject energyParent;
    [field: SerializeField] Image imgMaskEnergy;
    [field: SerializeField] RawImage imgEnergyFill;
    [field: SerializeField] bool isEnergyFull;

    [field: Header("UI Skill")]
    [field: SerializeField] GameObject skillParent;
    [field: SerializeField] Image imgMaskSkill;
    [field: SerializeField] RawImage imgSkillFill;
    [field: SerializeField] bool isSkillFull;

    [field: Header("UI Enery & Skill Animation")]
    [field: SerializeField] float skillBarFillAnimationSpeed = 0.15f;
    [field: SerializeField] float skillBarFillValueAnimationSpeed = 0.2f;
    [field: SerializeField] float skillBarFullAnimationTime = 0.2f;
    [field: SerializeField] float skillBarFullAnimationTimeDelay = 0.1f;
    const float parentTargetRotationZ = 3.0f;


    [field: Header("UI Collected Present")]
    [field: SerializeField] Text txtCollectedPresent;
    [field: SerializeField] float timeCollectedPresent = 3.0f;
    [field: SerializeField] float timeCollectedPresentDelay = 3.0f;

    [field: Header("Script References")]
    PlayerSkillCrushAndRun playerSkill;
    PlayerEnergy playerEnergy;
    GameManager gameManager;

    [field: Header("Coroutine")]
    Coroutine energyCoroutine;
    Coroutine skillCoroutine;

    //--------------------------------------------------------------------------------------
    //  今は使わない（スキルゲージのアニメーションのため）
    //  Energyやスキルが決めたらまだ使う
    //--------------------------------------------------------------------------------------
    //[Header("UI Skill")]
    //[SerializeField] Image[] sliderSkillSeparator;    //スキル値を表示するUI
    //public Image imgMask;
    //public RawImage imgSkillFill;
    //public GameObject UISeparatorParent;

    //const int UISeparatorNum = 5;
    //const float UISeparatorSize = 6.0f;

    //[SerializeField] float skillBarFillAnimationSpeed = 0.1f;

    //public AnimationCurve skillFillSpeedCurve;
    //[SerializeField] float skillFillSpeed = 0.05f;
    //public float skillFillLastValue = 0.0f;
    //public float skillFillTargetValue;
    //public bool isAnimateSkillUI;
    //public float timeSkillValueInterpolate;
    //float timeSkillValueNow;

    //[Header("Prefab")]
    //public GameObject prefUISkillSeparator;
    //--------------------------------------------------------------------------------------

    private void Start()
    {
        //isEnergyFull = true;
        //StartCoroutine(AnimationEnergyBarFull());
    }

    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkillCrushAndRun>();
        playerEnergy = GetComponent<PlayerEnergy>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        AnimateFillBar(imgEnergyFill);
        AnimateFillBar(imgSkillFill);
    }

    //正規化する値を戻る
    //UI Image->fillのため
    //Return 0.0f ~ 1.0f
    float GetImageFillValueNormalized(float value, float minValue, float maxValue)
    {
        float fillValueNormalize = value / (minValue + maxValue);
        return fillValueNormalize;
    }

    //ENergyのゲージの値を更新する
    public void UpdateEnergyBarUI()
    {
        float fillValue = GetImageFillValueNormalized(playerEnergy.PlayerEnergyValue, playerEnergy.PlayerEnergyMin, playerEnergy.PlayerEnergyMax);
        imgMaskEnergy.DOFillAmount(fillValue, skillBarFillValueAnimationSpeed).SetEase(Ease.OutCirc);
        isEnergyFull = false;
        if (fillValue >= 1.0f)
        {
            isEnergyFull = true;
            if (energyCoroutine == null)
            {
                energyCoroutine = StartCoroutine(AnimationEnergyBarFull());
            }
        }
    }

    //Skillのゲージの値を更新する
    public void UpdateSkillBarUI()
    {
        float fillValue = GetImageFillValueNormalized(playerSkill.SkillValue, playerSkill.SkillValueMin, playerSkill.SkillValueMax);
        imgMaskSkill.DOFillAmount(fillValue, skillBarFillValueAnimationSpeed).SetEase(Ease.OutCirc);
        isSkillFull = false;
        if (fillValue >= 1.0f)
        {
            isSkillFull = true;
            if (skillCoroutine == null)
            {
                skillCoroutine = StartCoroutine(AnimationSkillBarFull());
            }
        }
    }

    //もらったプレゼント箱数を更新する
    public void UpdateCollectedPresentUI()
    {
        StartCoroutine(AnimationPlayerCollectedPresent());
    }

    #region ANIMATION
    IEnumerator AnimationPlayerCollectedPresent()
    {
        yield return new WaitForSeconds(timeCollectedPresentDelay);
        Vector3 scaleStart = Vector3.zero;
        Vector3 scaleTarget = Vector3.one; //new Vector3(1.f, 1.4f, 1.4f);
        {
            txtCollectedPresent.transform.localScale = scaleStart;
        }
        txtCollectedPresent.transform.DOScale(scaleTarget, timeCollectedPresent).SetEase(Ease.OutBack);
        txtCollectedPresent.text = gameManager.PresentGetNum.ToString();
        yield return null;
    }

    IEnumerator AnimationEnergyBarFull()
    {
        while (isEnergyFull)
        {
            energyParent.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, 2.0f), skillBarFullAnimationTime, 4, 10.0f);
            yield return new WaitForSeconds(skillBarFullAnimationTime + skillBarFullAnimationTimeDelay);
        }
        energyCoroutine = null;
    }

    IEnumerator AnimationSkillBarFull()
    {
        while (isSkillFull)
        {
            skillParent.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, 2.0f), skillBarFullAnimationTime, 4, 10.0f);
            yield return new WaitForSeconds(skillBarFullAnimationTime + skillBarFullAnimationTimeDelay);
        }
        skillCoroutine = null;
    }


    #endregion


    #region UI SKILL
    //今は使わない
    //SKillのゲージのアニメーション


    //void InitUISkill()
    //{
    //    //CreateUISeparator();
    //    //imgMask.fillAmount = 0.0f;
    //    //skillFillLastValue = playerSkill.SkillValue;
    //    //skillFillTargetValue = playerSkill.GetSkillValueNow;
    //    //ResetAnimateSkillBarValue(true);
    //}

    //void CreateUISeparator()
    //{
    //    //RectTransform parentRectSize = UISeparatorParent.GetComponent<RectTransform>();
    //    //float firstSeperatePosBeforeMinusWidth = parentRectSize.rect.width / UISeparatorNum;
    //    //float firstSeperatePos = firstSeperatePosBeforeMinusWidth - UISeparatorSize * 0.5f;

    //    //int createObjNum = UISeparatorNum - 1;
    //    //for (int i = 0; i < createObjNum; ++i)
    //    //{
    //    //    GameObject obj = Instantiate(prefUISkillSeparator, UISeparatorParent.transform);

    //    //    RectTransform objRect = obj.GetComponent<RectTransform>();
    //    //    Vector2 newPos = Vector2.zero;
    //    //    newPos.x = firstSeperatePos + firstSeperatePosBeforeMinusWidth * i;
    //    //    newPos.y = objRect.anchoredPosition.y;
    //    //    objRect.anchoredPosition = newPos;

    //    //}
    //}

    void AnimateFillBar(RawImage imgFill)
    {
        Rect uvRect = imgFill.uvRect;
        uvRect.x += skillBarFillAnimationSpeed * Time.deltaTime;
        imgFill.uvRect = uvRect;

        //if (!isAnimateSkillUI) return;

        //timeSkillValueNow += skillFillSpeed * Time.deltaTime;
        //timeSkillValueInterpolate = skillFillSpeedCurve.Evaluate(timeSkillValueNow);

        //float nowAnimatedValue = Mathf.Lerp(skillFillLastValue, skillFillTargetValue, timeSkillValueInterpolate);

        //imgMask.fillAmount = nowAnimatedValue / playerSkill.GetSKillValueMax();

        //CheckAnimateSkillBar();
    }

    //public void GetNewSkillValue()
    //{
    //    //skillFillLastValue = imgMask.fillAmount * 100.0f;
    //    //skillFillTargetValue = playerSkill.GetSkillValueNow();
    //    //ResetAnimateSkillBarValue(true);
    //}

    //void CheckAnimateSkillBar()
    //{
    //    //if (timeSkillValueInterpolate >= 1.0f)
    //    //{
    //    //    ResetAnimateSkillBarValue();
    //    //}
    //}

    //void ResetAnimateSkillBarValue(bool isAnimate = false)
    //{
    //    //timeSkillValueInterpolate = 0.0f;
    //    //timeSkillValueNow = 0.0f;
    //    //isAnimateSkillUI = isAnimate;
    //}


    #endregion
}
