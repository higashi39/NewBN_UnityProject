using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [field: Header("Player References")]
    PlayerMove playerMove;
    PlayerSkillCrushAndRun playerSkill;
    PlayerEnergy playerEnergy;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerSkill = GetComponent<PlayerSkillCrushAndRun>();
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    //  ƒvƒŒƒCƒ„[‚Ì“ü—Í‚ğİ’è‚·‚é
    public void SetPlayerInputDisabled(bool isDisabled = true)
    {
        playerMove.IsInputDisabled = isDisabled;
        playerSkill.IsInputDisabled = isDisabled;
        playerEnergy.IsInputDisabled = isDisabled;
    }

}
