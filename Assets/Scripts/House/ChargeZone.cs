using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeZone : MonoBehaviour
{
    //ChargeZone
    //プレイヤーのEnergyを回復する
    //回復値はPlayerToolから設定（Player->Tool）
    //ChargeZoneはHouse/家の前に置く

    [field: Header("Script References")]
    [field: SerializeField] PlayerEnergy playerEnergy;

    private void Awake()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
