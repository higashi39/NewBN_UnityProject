using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChargeGenerator : MonoBehaviour
{
    enum IndicatorLight
    {
        INDICATOR_GREEN_LIGHT,
        INDICATOR_YELLOW_LIGHT,
        INDICATOR_RED_LIGHT,
        INDICATOR_X_R_LIGHT,
        INDICATOR_X_L_LIGHT,
    }

    [field: Header("Properties")]
    [field: SerializeField] float GeneratorEnergyAmountMax { set; get; }
    [field: SerializeField] float GeneratorEnergyAmount { set; get; }
    [field: SerializeField] bool IsEnergyEmpty { set; get; }
    [field: SerializeField] bool IsInsideChargeZone { set; get; }
    [field: SerializeField] bool IsShowIndicatorCharge { set; get; }

    [field: Header("Energy Charge per Second")]
    [field: SerializeField] float EnergyChargePerSec { set; get; } = 2.0f;

    [field: Header("Script References")]
    [field: SerializeField] PlayerEnergy playerEnergy;
    [field: SerializeField] GameManager gameManager;

    [field: Header("Indicator Settings")]
    // 0-> Green / Top
    // 1-> Yellow / Middle
    // 2-> Red / Bottom
    [field: SerializeField] GeneratorLightIndicatorSet[] GeneratorLightIndicator;

    const float INDICATOR_GREEN_MIN_PERCENTAGE = 60.0f;
    const float INDICATOR_YELLOW_MIN_PERCENTAGE = 30.0f;
    const float INDICATOR_RED_MIN_PERCENTAGE = 0.0f;
    const float BLINKING_TIME_SHOW = 0.5f;
    float blinkTime;
    bool isBlink;

    private void Awake()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GeneratorEnergyAmount = GeneratorEnergyAmountMax;
        CheckOnOffBlinkIndicator();
        IsEnergyEmpty = false;
        blinkTime = 0.0f;
        isBlink = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsInsideChargeZone)
        {
            return;
        }
        else
        {
            PlayBlinkingAnimation();
        }

        if (IsEnergyEmpty)
        {
            return;
        }

        if (gameManager.GameStatus != GameManager.GAME_STATUS.GAME_NORMAL)
        {
            return;
        }

        if (playerEnergy.PlayerEnergyValue >= playerEnergy.PlayerEnergyMax)
        {
            return;
        }

        float chargeAmountRaw = EnergyChargePerSec * Time.deltaTime;
        float totalAfterCharge = chargeAmountRaw + playerEnergy.PlayerEnergyValue;
        float chargedVal = chargeAmountRaw;
        if (totalAfterCharge > playerEnergy.PlayerEnergyMax)
        {
            chargedVal = playerEnergy.PlayerEnergyMax - playerEnergy.PlayerEnergyValue;
        }

        GeneratorEnergyAmount -= chargedVal;
        playerEnergy.IsRecharging = true;
        if (GeneratorEnergyAmount <= 0)
        {
            IsEnergyEmpty = true;
            playerEnergy.IsRecharging = false;
        }
    }

    private void LateUpdate()
    {
        if (playerEnergy.IsRecharging)
        {
            if (playerEnergy.PlayerEnergyValue >= playerEnergy.PlayerEnergyMax)
            {
                playerEnergy.IsRecharging = false;
                ResetBlink();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnergy.EnergyChargePerSec = EnergyChargePerSec;
            IsInsideChargeZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsInsideChargeZone = false;
            playerEnergy.IsRecharging = false;
            ResetBlink();
        }
    }

    void PlayBlinkingAnimation()
    {
        if (!IsEnergyEmpty)
        {
            if (!playerEnergy.IsRecharging)
            {
                return;
            }
        }

        //プレゼント見つかる時アニメションポーズ
        if (gameManager.GameStatus != GameManager.GAME_STATUS.GAME_NORMAL)
        {
            return;
        }

        blinkTime -= Time.deltaTime;
        if (blinkTime <= 0.0f)
        {
            blinkTime = BLINKING_TIME_SHOW;
            isBlink = !isBlink;
            BlinkTheIndicator(isBlink);
            if (isBlink)
            {
                CheckOnOffBlinkIndicator();
            }
        }
    }

    void BlinkTheIndicator(bool isBlinking)
    {
        float EnergyPercentageLeft = (GeneratorEnergyAmount / GeneratorEnergyAmountMax) * 100.0f;
        if (EnergyPercentageLeft > INDICATOR_GREEN_MIN_PERCENTAGE)
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_GREEN_LIGHT].IndicatorFront.enabled = isBlinking;
        }
        else if (EnergyPercentageLeft > INDICATOR_YELLOW_MIN_PERCENTAGE)
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_YELLOW_LIGHT].IndicatorFront.enabled = isBlinking;
        }
        else if (EnergyPercentageLeft > INDICATOR_RED_MIN_PERCENTAGE)
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_RED_LIGHT].IndicatorFront.enabled = isBlinking;
        }
        else
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_X_R_LIGHT].IndicatorFront.enabled = isBlinking;
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_X_L_LIGHT].IndicatorFront.enabled = isBlinking;
        }
    }

    void CheckOnOffBlinkIndicator()
    {
        float EnergyPercentageLeft = (GeneratorEnergyAmount / GeneratorEnergyAmountMax) * 100.0f;
        if (EnergyPercentageLeft > INDICATOR_GREEN_MIN_PERCENTAGE)
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_X_R_LIGHT].SetIndicatorSetEnabled(false);
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_X_L_LIGHT].SetIndicatorSetEnabled(false);
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_GREEN_LIGHT].IndicatorFront.enabled = true;
        }
        else if (EnergyPercentageLeft > INDICATOR_YELLOW_MIN_PERCENTAGE)
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_GREEN_LIGHT].IndicatorFront.enabled = false;
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_YELLOW_LIGHT].IndicatorFront.enabled = true;
        }
        else if (EnergyPercentageLeft > INDICATOR_RED_MIN_PERCENTAGE)
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_YELLOW_LIGHT].IndicatorFront.enabled = false;
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_RED_LIGHT].IndicatorFront.enabled = true;
        }
        else
        {
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_GREEN_LIGHT].SetIndicatorSetEnabled(false);
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_YELLOW_LIGHT].SetIndicatorSetEnabled(false);
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_RED_LIGHT].SetIndicatorSetEnabled(false);

            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_X_R_LIGHT].SetIndicatorSetEnabled();
            GeneratorLightIndicator[(int)IndicatorLight.INDICATOR_X_L_LIGHT].SetIndicatorSetEnabled();
        }
    }

    void ResetBlink()
    {
        isBlink = true;
        blinkTime = 0.0f;
        CheckOnOffBlinkIndicator();
    }



    [System.Serializable]
    struct GeneratorLightIndicatorSet
    {
        public MeshRenderer IndicatorBackground;
        public MeshRenderer IndicatorFront;

        public void SetIndicatorSetEnabled(bool isEnabled = true)
        {
            IndicatorBackground.enabled = isEnabled;
            IndicatorFront.enabled = isEnabled;
        }
    }
}
