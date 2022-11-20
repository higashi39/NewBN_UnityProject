using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCrushAndRun : MonoBehaviour
{

    [field: Header("Skill Properties")]
    //���̃X�L���Q�[�W�l
    [field: SerializeField] public float SkillValue { private set; get; }
    //�Œ�X�L���Q�[�W�l
    [field: SerializeField] public float SkillValueMin { private set; get; } = 0.0f;
    //�ő�X�L���Q�[�W�l
    [field: SerializeField] public float SkillValueMax { private set; get; } = 100.0f;
    //���X�L���g�����ǂ���
    [field: SerializeField] public bool IsUseSkill { private set; get; }
    //IsInputDisabled��false�ɂ���O�ɂ��̎��̓X�L���g���Ă��邩�ǂ���
    [field: SerializeField] public bool IsUseSkillBeforeDisabled { private set; get; }
    //���͂ł��邩�ǂ���
    [field: SerializeField] bool isInputDisabled;

    //�����o��Ƃ��A�A�j���[�V������idle�ɂ��邽��
    //��U�A�����o����IsUseSkill��false�ɐݒ�
    //�����A���͏o������A�����A�O�̓X�L�����g���Ă���ꍇ
    //IsUseSkill��true�ɖ߂�
    public bool IsInputDisabled
    {
        set
        {
            isInputDisabled = value;
            if(isInputDisabled)
            {
                IsUseSkillBeforeDisabled = IsUseSkill;
                IsUseSkill = false;
            }
            else
            {
                if(IsUseSkillBeforeDisabled)
                {
                    IsUseSkill = true;
                }
            }
            
            
        }
        get { return isInputDisabled; }
    }

    [field: Header("Skill Settings")]
    //�X�L�����g�����̃Q�[�W�l�����炷�l�iSkillValuePerSec * Time.deltatime�j
    [field: SerializeField] float SkillValuePerSec { set; get; } = 2.0f;

    [field: Header("Script References")]
    PlayerUI playerUI;

    [field: Header("SFX")]
    [field: SerializeField] SFXPlay SfxPlayPUseSkill { set; get; }

    void Awake()
    {
        playerUI = GetComponent<PlayerUI>();
    }

    // Start is called before the first frame update
    void Start()
    {

        IsUseSkill = false;
    }

    // Update is called once per frame
    void Update()
    {
        //���͂ł��Ȃ�������
        if (IsInputDisabled)
        {
            return;
        }

        //�E�N���b�N������X�L�����g��
        if (Input.GetMouseButtonDown(1))
        {
            IsUseSkill = ActionUseSkill();
        }

        //�X�L�����g���Ă��鎞
        //�X�L���l�iSkillValue�j�����炷����
        if (IsUseSkill)
        {
            float skillReduceValue = SkillValuePerSec * Time.deltaTime;
            ReduceSkillValue(skillReduceValue);
        }

    }

    //SkillValue�𑝂₷
    //value�́iprefabs/GardenDecoration/Bushes/Bushes_Single�j����ݒ肷��
    public void AddSkillValue(float value)
    {
        if (IsUseSkill || IsUseSkillBeforeDisabled)
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

    //SkillValue�����炷
    //value��SkillValuePerSec����ݒ肷��i[field: Header("Skill Settings")]�̒��j
    void ReduceSkillValue(float value)
    {
        SkillValue -= value;
        if (SkillValue <= SkillValueMin)
        {
            SkillValue = SkillValueMin;
            IsUseSkill = false;
        }
        playerUI.UpdateSkillBarUI();
    }

    //�X�L�����g���邩�ǂ����̔��f
    //�X�L�����g������
    //�X�L���Q�[�W�l�iSkillValue�͖��^������
    bool ActionUseSkill()
    {
        if (SkillValue >= SkillValueMax || IsUseSkill)
        {
            SfxPlayPUseSkill.PlaySFXSound();
            return true;
        }
        return false;
    }

}
