using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerBody : MonoBehaviour
{
    [field: Header("Player Trigger Body Property")]
    //���̒��ɂ��邩�ǂ����i���x�����炷���߁j
    //Player->PlayerTriggerBody�ɓ\��
    [field: SerializeField] public bool IsInsideBush { set; get; } = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            IsInsideBush = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            IsInsideBush = false;
        }
    }

}
