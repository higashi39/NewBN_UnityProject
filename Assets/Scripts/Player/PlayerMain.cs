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

    //  �v���C���[�̓��͂�ݒ肷��
    public void SetPlayerInputEnable(bool isCanInput = true)
    {
        SetPlayerMove(isCanInput);
        SetPlayerSkill(isCanInput);
        SetPlayerEnergy(isCanInput);
    }

    //  �v���C���[�͈ړ����ǂ�����ݒ肷��
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
