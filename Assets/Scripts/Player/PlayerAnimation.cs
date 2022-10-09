using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [field: SerializeField] PlayerMove playerMove;
    [field: SerializeField] PlayerSkillCrushAndRun playerSkill;

    [field: SerializeField] Animator anim;

    void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        playerSkill = GetComponentInParent<PlayerSkillCrushAndRun>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("movementSpeed", playerMove.MovementSpeed);
        anim.SetBool("isUseSkill", playerSkill.IsUseSkill);
    }

    void SetPlayerAnimationSpeed(float speed = 1.0f)
    {

    }
}
