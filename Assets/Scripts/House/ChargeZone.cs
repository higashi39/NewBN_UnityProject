using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeZone : MonoBehaviour
{
    //ChargeZone
    //�v���C���[��Energy���񕜂���
    //�񕜒l��PlayerTool����ݒ�iPlayer->Tool�j
    //ChargeZone��House/�Ƃ̑O�ɒu��

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
