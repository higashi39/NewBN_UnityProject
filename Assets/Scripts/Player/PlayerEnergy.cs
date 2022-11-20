using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [field: Header("Energy Settings")]
    //最低Energy
    [field: SerializeField] public float PlayerEnergyMin { private set; get; } = 0.0f;
    //最大Energy
    [field: SerializeField] public float PlayerEnergyMax { private set; get; } = 100.0f;
    //チャージしている時のもらうEnergy (Time.deltaTimeに掛ける)
    [field: SerializeField] public float EnergyChargePerSec { set; get; } = 5.5f;


    [field: Header("Player Energy Property")]
    //今のプレイヤーのEnergy
    [field: SerializeField] public float PlayerEnergyValue { private set; get; }
    //今Energyを使っているかどうか（草を刈っているかどうか）
    [field: SerializeField] public bool IsUseEnergy { set; get; }
    //今チャージしているかどうか
    [field: SerializeField] public bool IsRecharging { set; get; }
    //入力請けいるかどうか
    [field: SerializeField] public bool IsInputDisabled { set; get; }
    [field: SerializeField] public bool IsHaveEnergy { set; get; }

    [field: Header("Script References")]
    [field: SerializeField] PlayerTool playerTool;
    [field: SerializeField] PlayerUI playerUI;
    [field: SerializeField] SFXPlay sfxPlayEnergyRecharge;
    [field: SerializeField] public SFXPlay SfxPlayCuttingGrass { private set; get; }



    private void Awake()
    {
        playerTool = GetComponentInChildren<PlayerTool>();
        playerUI = GetComponent<PlayerUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Energyの初期化
        //最初のEnergyは満タンにする
        PlayerEnergyValue = PlayerEnergyMax;
        playerUI.UpdateEnergyBarUI();
        IsHaveEnergy = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInputDisabled)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            IsUseEnergy = true;
            if (playerTool.IsToolCuttingGrass && IsHaveEnergy)
            {
                SfxPlayCuttingGrass.PlaySFXSound();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsUseEnergy = false;
            SfxPlayCuttingGrass.StopSFXSound();
        }

        if (IsRecharging)
        {
            float chargeValue = EnergyChargePerSec * Time.deltaTime;
            AddPlayerEnergy(chargeValue);
            sfxPlayEnergyRecharge.PlaySFXSound();
        }
        else
        {
            sfxPlayEnergyRecharge.StopSFXSound();
        }
    }

    //プレイヤーのEnergyを増やす
    //増やす条件は家の前にいる（チャージエリアはprefabs/House/ChargingZone　設定する）
    //valueはEnergyChargePerSecから変更する（[field:Header("Energy Settings")]の中）
    public void AddPlayerEnergy(float value)
    {
        PlayerEnergyValue += value;
        IsHaveEnergy = true;
        if (PlayerEnergyValue > PlayerEnergyMax)
        {
            PlayerEnergyValue = PlayerEnergyMax;
        }
        playerUI.UpdateEnergyBarUI();
    }

    //プレイヤーのEnergyを減らす
    //減らす値はvalueによって
    void ReducePlayerEnergy(float value)
    {
        PlayerEnergyValue -= value;
        if (PlayerEnergyValue <= PlayerEnergyMin)
        {
            IsHaveEnergy = false;
            SfxPlayCuttingGrass.StopSFXSound();
            PlayerEnergyValue = PlayerEnergyMin;
        }
        playerUI.UpdateEnergyBarUI();
    }

    //草（bushes）を刈る
    //energyNeededは草を刈る時の必要のenergy
    //energyNeededはSingleBushスクリプトに設定する（prefabs/GardenDecoration/Bushes/Bushes_Single）
    public bool ActionCutGrass(float energyNeeded)
    {
        if (IsInputDisabled)
        {
            return false;
        }

        if (IsUseEnergy)
        {
            float energyNeededPerFrame = energyNeeded * Time.deltaTime;
            float energyReduced = energyNeededPerFrame;
            float energyLeft = PlayerEnergyValue - energyNeededPerFrame;
            if (energyLeft < 0.0f)
            {
                if (PlayerEnergyValue <= 0.0f)
                {
                    return false;
                }
                energyReduced = PlayerEnergyValue;
            }
            ReducePlayerEnergy(energyReduced);
            return true;
        }
        return false;
    }
}
