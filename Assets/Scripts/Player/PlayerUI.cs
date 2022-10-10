using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [field: Header("UI Energy")]
    [field: SerializeField] Image imgMaskEnergy;

    [field: Header("UI Skill")]
    [field: SerializeField] Image imgMaskSkill;

    [field: Header("UI Collected Present")]
    [field: SerializeField] Text txtCollectedPresent;

    [field: Header("Script References")]
    PlayerSkillCrushAndRun playerSkill;
    PlayerEnergy playerEnergy;
    GameManager gameManager;

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


    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkillCrushAndRun>();
        playerEnergy = GetComponent<PlayerEnergy>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCollectedPresentUI();
    }

    // Update is called once per frame
    void Update()
    {

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
        imgMaskEnergy.fillAmount = fillValue;
    }

    //Skillのゲージの値を更新する
    public void UpdateSkillBarUI()
    {
        float fillValue = GetImageFillValueNormalized(playerSkill.SkillValue, playerSkill.SkillValueMin, playerSkill.SkillValueMax);
        imgMaskSkill.fillAmount = fillValue;
    }


    //もらったプレゼント箱数を更新する
    public void UpdateCollectedPresentUI()
    {
        txtCollectedPresent.text = gameManager.PresentGetNum.ToString();
    }

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

    //void AnimateSkillBar()
    //{
    //    //Rect uvRect = imgSkillFill.uvRect;
    //    //uvRect.x += skillBarFillAnimationSpeed * Time.deltaTime;
    //    //imgSkillFill.uvRect = uvRect;

    //    //if (!isAnimateSkillUI) return;

    //    //timeSkillValueNow += skillFillSpeed * Time.deltaTime;
    //    //timeSkillValueInterpolate = skillFillSpeedCurve.Evaluate(timeSkillValueNow);

    //    //float nowAnimatedValue = Mathf.Lerp(skillFillLastValue, skillFillTargetValue, timeSkillValueInterpolate);

    //    //imgMask.fillAmount = nowAnimatedValue / playerSkill.GetSKillValueMax();

    //    //CheckAnimateSkillBar();
    //}

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
