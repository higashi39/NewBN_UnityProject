using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    PlayerUI playerUI;

    [field: SerializeField] public float PlayerEnergyValue { private set; get; }
    [field: SerializeField] public float PlayerEnergyMin { private set; get; } = 0.0f;
    [field: SerializeField] public float PlayerEnergyMax { private set; get; } = 100.0f;
    [field: SerializeField] public float EnergyChargePerSec { private set; get; } = 5.5f;
    [field: SerializeField] public bool IsUseEnergy { private set; get; }
    [field: SerializeField] public bool IsCanCutGrass { private set; get; }
    [field: SerializeField] public bool IsRecharging { set; get; }

    [field: SerializeField] SFXPlay sfxPlay;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        PlayerEnergyValue = PlayerEnergyMax;
        playerUI.UpdateEnergyBarUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsUseEnergy = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsUseEnergy = false;
        }

        if (IsRecharging)
        {
            float chargeValue = EnergyChargePerSec * Time.deltaTime;
            AddPlayerEnergy(chargeValue);
            sfxPlay.enabled = true;
        }
        else
        {
            sfxPlay.enabled = false;
        }
    }


    public void AddPlayerEnergy(float value)
    {
        PlayerEnergyValue += value;
        if (PlayerEnergyValue > PlayerEnergyMax)
        {
            PlayerEnergyValue = PlayerEnergyMax;
        }
        playerUI.UpdateEnergyBarUI();
    }

    public void ReducePlayerEnergy(float value)
    {
        PlayerEnergyValue -= value;
        if (PlayerEnergyValue < PlayerEnergyMin)
        {
            PlayerEnergyValue = PlayerEnergyMin;
        }
        playerUI.UpdateEnergyBarUI();
    }

    public bool ActionCutGrass(float energyNeeded)
    {
        if (IsUseEnergy)
        {
            float energyNeededPerFrame = energyNeeded * Time.deltaTime;
            float energyLeft = PlayerEnergyValue - energyNeededPerFrame;
            if (energyLeft < 0)
            {
                return false;
            }
            ReducePlayerEnergy(energyNeededPerFrame);
            return true;
        }
        return false;
    }
}
