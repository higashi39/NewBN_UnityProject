using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCrushAndRun : MonoBehaviour
{

    [field: Header("Skill Properties")]
    //今のスキルゲージ値
    [field: SerializeField] public float SkillValue { private set; get; }
    //最低スキルゲージ値
    [field: SerializeField] public float SkillValueMin { private set; get; } = 0.0f;
    //最大スキルゲージ値
    [field: SerializeField] public float SkillValueMax { private set; get; } = 100.0f;
    //今スキル使うかどうか
    [field: SerializeField] public bool IsUseSkill { private set; get; }
    //IsInputDisabledをfalseにする前にその時はスキル使っているかどうか
    [field: SerializeField] public bool IsUseSkillBeforeDisabled { private set; get; }
    //入力できるかどうか
    [field: SerializeField] bool isInputDisabled;

    //箱が出るとき、アニメーションはidleにするため
    //一旦、箱が出たらIsUseSkillはfalseに設定
    //もし、入力出来たら、もし、前はスキルを使っている場合
    //IsUseSkillはtrueに戻る
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
    //スキルを使う時のゲージ値を減らす値（SkillValuePerSec * Time.deltatime）
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
        //入力できなかったら
        if (IsInputDisabled)
        {
            return;
        }

        //右クリックしたらスキルを使う
        if (Input.GetMouseButtonDown(1))
        {
            IsUseSkill = ActionUseSkill();
        }

        //スキルを使っている時
        //スキル値（SkillValue）を減らすため
        if (IsUseSkill)
        {
            float skillReduceValue = SkillValuePerSec * Time.deltaTime;
            ReduceSkillValue(skillReduceValue);
        }

    }

    //SkillValueを増やす
    //valueは（prefabs/GardenDecoration/Bushes/Bushes_Single）から設定する
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

    //SkillValueを減らす
    //valueはSkillValuePerSecから設定する（[field: Header("Skill Settings")]の中）
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

    //スキルを使得るかどうかの判断
    //スキルを使う条件
    //スキルゲージ値（SkillValueは満タンだけ
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
