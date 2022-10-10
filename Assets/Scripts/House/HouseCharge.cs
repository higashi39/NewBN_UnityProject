using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCharge : MonoBehaviour
{
    //ChargeZone
    //�v���C���[��Energy���񕜂���
    //�񕜒l��PlayerTool����ݒ�iPlayer->Tool�j
    //ChargeZone��House/�Ƃ̑O�ɒu��

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
