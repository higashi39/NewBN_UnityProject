using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerBody : MonoBehaviour
{
    [field: Header("Player Trigger Body Property")]
    //‘‚Ì’†‚É‚¢‚é‚©‚Ç‚¤‚©i‘¬“x‚ðŒ¸‚ç‚·‚½‚ßj
    //Player->PlayerTriggerBody‚É“\‚é
    [field: SerializeField] public bool IsInsideBush { set; get; } = false;

    [field: Header("SFX")]
    [field: SerializeField] SFXPlay SfxPlayPlayerHitBushes { set; get; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            IsInsideBush = true;
            SfxPlayPlayerHitBushes.PlaySFXSound();
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
