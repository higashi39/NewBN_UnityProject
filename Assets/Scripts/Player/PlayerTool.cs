using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    [field: Header("Prefab Particle")]
    [field: SerializeField] GameObject prefParticleLeaf;

    [field: Header("Player Tool Properties")]
    //�����������Ă��邩�ǂ���
    [field: SerializeField] public bool IsToolCuttingGrass { private set; get; }
    //�����؂̒��̑��̎Q��
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
        //�����Ȃ���
        if (bushesTarget == null)
        {
            return;
        }

        //�l������
        IsToolCuttingGrass = false;
        bool isDestroyBush = false;

        //�X�L���g���Ă��邩�ǂ���
        //�g�����瑐������������
        //���Ȃ��ꍇ�v���C���[���~�܂��đ�������
        //�������� isDestroyBush�̒ltrue�ɂȂ�
        if (playerSkill.IsUseSkill)
        {
            isDestroyBush = bushesTarget.CutBushes(true);
        }
        else if (playerEnergy.ActionCutGrass(bushesTarget.EnergyConsumeValue))
        {
            IsToolCuttingGrass = true;
            isDestroyBush = bushesTarget.CutBushes();
        }

        //�����������j�󂵂���
        if (isDestroyBush)
        {
            //�v���[���g�𐶐�
            presentGenerator.GeneratePresent(bushesTarget.transform.position);
            //�X�L���l�����炤
            playerSkill.AddSkillValue(bushesTarget.SkillGetValue);
            //����������Ȃ�
            IsToolCuttingGrass = false;
            //Energy���g��Ȃ���SFX������
            playerEnergy.IsUseEnergy = false;
            playerEnergy.SfxPlayCuttingGrass.StopSFXSound();

            //���̒��ɂ��Ȃ�
            playerTriggerBody.IsInsideBush = false;

            //Particle�����
            //Particle��3�b��A�����I�Ɏ����Ŕj�󂷂�
            GameObject obj = Instantiate(prefParticleLeaf);
            Vector3 newPos = bushesTarget.transform.position;
            newPos.y += 1.0f;
            obj.transform.position = newPos;
            Destroy(obj, 3.0f);
        }

        //���̎Q�Ƃ����Z�b�g
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
