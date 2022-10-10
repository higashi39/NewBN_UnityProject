using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [field: Header("Script References")]
    [field: SerializeField] PlayerMove playerMove;
    [field: SerializeField] PlayerSkillCrushAndRun playerSkill;
    [field: SerializeField] PlayerEnergy playerEnergy;
    [field: SerializeField] PlayerTool playerTool;

    [field: Header("Component References")]
    [field: SerializeField] Animator anim;

    void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        playerSkill = GetComponentInParent<PlayerSkillCrushAndRun>();
        playerEnergy = GetComponentInParent<PlayerEnergy>();
        playerTool = FindObjectOfType<PlayerTool>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("movementSpeed", playerMove.MovementSpeed);
        anim.SetBool("isUseSkill", playerSkill.IsUseSkill);
        anim.SetBool("isToolCuttingGrass", playerTool.IsToolCuttingGrass);
    }
}
