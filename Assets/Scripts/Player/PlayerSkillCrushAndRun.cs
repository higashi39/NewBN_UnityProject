using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCrushAndRun : MonoBehaviour
{
    PlayerUI playerUI;

    [field: SerializeField] public float SkillValue { private set; get; }
    [field: SerializeField] public float SkillValueMin { private set; get; } = 0.0f;
    [field: SerializeField] public float SkillValueMax { private set; get; } = 100.0f;
    [field: SerializeField] public bool IsUseSkill { private set; get; }
    [field: SerializeField] public float SkillMovementSpeed { private set; get; } = 5.0f;

    [SerializeField] float skillValueStart = 50.0f;
    [SerializeField] float skillValuePerSec = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        SkillValue = skillValueStart;
        IsUseSkill = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsUseSkill = ActionUseSkill();
        }

        if (IsUseSkill)
        {
            float skillReduceValue = skillValuePerSec * Time.deltaTime;
            ReduceSkillValue(skillReduceValue);
        }

    }

    public void AddSkillValue(float value)
    {
        if (IsUseSkill)
        {
            return;
        }

        SkillValue += value;
        if (SkillValue > SkillValueMax)
        {
            SkillValue = SkillValueMax;
        }
        playerUI.UpdateSkillBarUI();
    }

    public void ReduceSkillValue(float value)
    {
        SkillValue -= value;
        if (SkillValue <= SkillValueMin)
        {
            SkillValue = SkillValueMin;
            IsUseSkill = false;
        }
        playerUI.UpdateSkillBarUI();
    }

    bool ActionUseSkill()
    {
        if (SkillValue >= SkillValueMax || IsUseSkill)
        {
            return true;
        }
        return false;
    }

}
