using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCharge : MonoBehaviour
{
    //ChargeZone
    //プレイヤーのEnergyを回復する
    //回復値はPlayerToolから設定（Player->Tool）
    //ChargeZoneはHouse/家の前に置く

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerEnergy = other.GetComponent<PlayerEnergy>();
            playerEnergy.IsRecharging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerEnergy = other.GetComponent<PlayerEnergy>();
            playerEnergy.IsRecharging = false;
        }
    }

}
