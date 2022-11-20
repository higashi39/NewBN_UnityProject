using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [field: Header("Energy Settings")]
    //�Œ�Energy
    [field: SerializeField] public float PlayerEnergyMin { private set; get; } = 0.0f;
    //�ő�Energy
    [field: SerializeField] public float PlayerEnergyMax { private set; get; } = 100.0f;
    //�`���[�W���Ă��鎞�̂��炤Energy (Time.deltaTime�Ɋ|����)
    [field: SerializeField] public float EnergyChargePerSec { set; get; } = 5.5f;


    [field: Header("Player Energy Property")]
    //���̃v���C���[��Energy
    [field: SerializeField] public float PlayerEnergyValue { private set; get; }
    //��Energy���g���Ă��邩�ǂ����i���������Ă��邩�ǂ����j
    [field: SerializeField] public bool IsUseEnergy { set; get; }
    //���`���[�W���Ă��邩�ǂ���
    [field: SerializeField] public bool IsRecharging { set; get; }
    //���͐������邩�ǂ���
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
        //Energy�̏�����
        //�ŏ���Energy�͖��^���ɂ���
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

    //�v���C���[��Energy�𑝂₷
    //���₷�����͉Ƃ̑O�ɂ���i�`���[�W�G���A��prefabs/House/ChargingZone�@�ݒ肷��j
    //value��EnergyChargePerSec����ύX����i[field:Header("Energy Settings")]�̒��j
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

    //�v���C���[��Energy�����炷
    //���炷�l��value�ɂ����
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

    //���ibushes�j������
    //energyNeeded�͑������鎞�̕K�v��energy
    //energyNeeded��SingleBush�X�N���v�g�ɐݒ肷��iprefabs/GardenDecoration/Bushes/Bushes_Single�j
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
