using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    [field: Header("Prefab Particle")]
    [field: SerializeField] GameObject prefParticleLeaf;

    [field: Header("Player Tool Properties")]
    //今草を刈っているかどうか
    [field: SerializeField] public bool IsToolCuttingGrass { private set; get; }
    //今草切の中の草の参照
    [field: SerializeField] SingleBush bushesTarget;

    [field: Header("Script References")]
    [field: SerializeField] PlayerEnergy playerEnergy;
    [field: SerializeField] PlayerSkillCrushAndRun playerSkill;
    [field: SerializeField] PlayerTriggerBody playerTriggerBody;
    [field: SerializeField] PresentGenerator presentGenerator;

    void Awake()
    {
        playerSkill = GetComponentInParent<PlayerSkillCrushAndRun>();
        playerEnergy = GetComponentInParent<PlayerEnergy>();
        playerTriggerBody = transform.parent.GetComponentInChildren<PlayerTriggerBody>();
        presentGenerator = FindObjectOfType<PresentGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        //草がない時
        if (bushesTarget == null)
        {
            return;
        }

        //値初期化
        IsToolCuttingGrass = false;
        bool isDestroyBush = false;

        //スキル使っているかどうか
        //使ったら草がすぐ刈った
        //しない場合プレイヤーが止まって草を刈る
        //刈ったら isDestroyBushの値trueになる
        if (playerSkill.IsUseSkill)
        {
            isDestroyBush = bushesTarget.CutBushes(true);
        }
        else if (playerEnergy.ActionCutGrass(bushesTarget.EnergyConsumeValue))
        {
            IsToolCuttingGrass = true;
            isDestroyBush = bushesTarget.CutBushes();
        }

        //刈った草が破壊したら
        if (isDestroyBush)
        {
            //プレゼントを生成
            presentGenerator.GeneratePresent(bushesTarget.transform.position);
            //スキル値をもらう
            playerSkill.AddSkillValue(bushesTarget.SkillGetValue);
            //草刈りをしない
            IsToolCuttingGrass = false;
            //Energyも使わないとSFXも消す
            playerEnergy.IsUseEnergy = false;
            playerEnergy.SfxPlayCuttingGrass.StopSFXSound();

            //草の中にいない
            playerTriggerBody.IsInsideBush = false;

            //Particleを作る
            //Particleは3秒後、自動的に自分で破壊する
            GameObject obj = Instantiate(prefParticleLeaf);
            Vector3 newPos = bushesTarget.transform.position;
            newPos.y += 1.0f;
            obj.transform.position = newPos;
            Destroy(obj, 3.0f);
        }

        //草の参照をリセット
        bushesTarget = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            bushesTarget = other.GetComponentInParent<SingleBush>();
        }
    }

}
