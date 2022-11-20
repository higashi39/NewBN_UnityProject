using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCharge : MonoBehaviour
{
    [field: Header("Energy Charge per Second")]
    [field: SerializeField] float EnergyChargePerSec { set; get; } = 5.5f;

    [field: Header("Script References")]
    [field: SerializeField] PlayerEnergy playerEnergy;

    private void Awake()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
    }

    //ChargeZone
    //プレイヤーのEnergyを回復する
    //回復値はPlayerToolから設定（Player->Tool）
    //ChargeZoneはHouse/家の前に置く

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnergy.EnergyChargePerSec = EnergyChargePerSec;
            playerEnergy.IsRecharging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnergy.IsRecharging = false;
        }
    }

}
