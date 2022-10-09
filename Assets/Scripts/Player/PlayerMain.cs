using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [Header("Player References")]
    PlayerMove playerMove;
    PlayerTool playerTool;
    PlayerSkillCrushAndRun playerSkill;
    PlayerEnergy playerEnergy;

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerSkill = GetComponent<PlayerSkillCrushAndRun>();
        playerTool = GetComponentInChildren<PlayerTool>();
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    //  プレイヤーの入力を設定する
    public void SetPlayerInputEnable(bool isCanInput = true)
    {
        SetPlayerMove(isCanInput);
        SetPlayerSkill(isCanInput);
        SetPlayerEnergy(isCanInput);
    }

    //  プレイヤーは移動かどうかを設定する
    public void SetPlayerMove(bool isCanMove = true)
    {
        playerMove.SetIsCanMove(isCanMove);
    }
    public void SetPlayerSkill(bool isCanUseSkill = true)
    {
        playerSkill.enabled = isCanUseSkill;
    }

    public void SetPlayerEnergy(bool isActive = true)
    {
        playerEnergy.enabled = isActive;
    }
}
