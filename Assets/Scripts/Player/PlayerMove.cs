using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [field: Header("Movement Speed Settings")]
    // �X�L�����g���Ƃ����x
    [field: SerializeField] float SkillMovementSpeed { set; get; } = 5.0f;
    //�ő�ړ����x
    [field: SerializeField] float NormalMovementSpeed { set; get; } = 3.0f;
    //���ɓ��鑬�x
    [field: SerializeField] float BushMovementSpeed { set; get; } = 1.25f;
    //��]���x
    //[field: SerializeField] float RotationSpeed { set; get; } = 3.0f;


    [field: Header("Player Move Settings")]
    //�v���C���[���ړ����邽�߂Ƀ}�E�X�ƍŒ዗���i��ʍ��W�j
    [field: SerializeField] float MinDistanceToMove { set; get; } = 10.0f;

    [field: Header("Player Move Direction")]
    //�ړ�����
    [field: SerializeField] Vector3 moveDir;
    //��]
    [field: SerializeField] Quaternion lookRot;

    [field: Header("Player Properties")]
    //���͐������邩�ǂ���
    [field: SerializeField] bool isInputDisabled;

    //�ő�ړ����x
    [field: SerializeField] public float MovementSpeed { private set; get; } = 3.0f;
    //�v���C���[�������ǂ����iMinDistanceToMove�ɂ���āj
    [field: SerializeField] bool IsPlayerMoving { set; get; }
    public bool IsInputDisabled
    {
        set
        {
            isInputDisabled = value;
            MovementSpeed = 0.0f;
            PlayerWalkRunSoundHandler(false);
        }
        get { return isInputDisabled; }
    }

    [field: Header("Script References")]
    PlayerSkillCrushAndRun playerSkill;
    PlayerTriggerBody playerTriggerBody;
    PlayerEnergy playerEnergy;
    PlayerTool playerTool;
    [field: SerializeField] SFXPlay sfxPlayWalk;
    [field: SerializeField] SFXPlay sfxPlayRun;

    [field: Header("Component References")]
    Rigidbody rb;

    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkillCrushAndRun>();
        playerTriggerBody = GetComponentInChildren<PlayerTriggerBody>();
        playerEnergy = GetComponent<PlayerEnergy>();
        playerTool = GetComponentInChildren<PlayerTool>();

        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MovementSpeed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInputDisabled)
        {
            return;
        }
        PlayerWalkRunSoundHandler();
        MovementSpeedUpdate();

        if (playerTool.IsToolCuttingGrass)
        {
            return;
        }

        //  �v���C���[�ƃ}�E�X�̉�ʍ��W���擾
        Vector2 playerPosScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mousePos = Input.mousePosition;

        //  �v���C���[�ƃ}�E�X�̋����i��ʍ��W�A�����h���W�ł͂Ȃ��j
        float mousePlayerDistance = Vector2.Distance(mousePos, playerPosScreen);

        //--------------------------------------------------
        //Move 1    �ړ����@�P�i�����ƌ������x����j
        //--------------------------------------------------
        //playerSharedData.MovementSpeed = speedCurve.Evaluate((mousePlayerDistance - minDistanceToMove) * scalerDistanceToMove) * maxMovementSpeed;
        //moveDir.x = mousePos.x - playerPosScreen.x;
        //moveDir.y = 0.0f;
        //moveDir.z = mousePos.y - playerPosScreen.y;
        //moveDir.Normalize();

        //--------------------------------------------------
        //Move 2    �ړ����@�Q�i���x�̓��j�A�u���`�v�j
        //--------------------------------------------------
        //movementSpeed = 0.0f;
        //if (mousePlayerDistance > minDistanceToMove)
        //{
        //    movementSpeed = maxMovementSpeed;
        //    moveDir.x = mousePos.x - playerPosScreen.x;
        //    moveDir.y = 0.0f;
        //    moveDir.z = mousePos.y - playerPosScreen.y;
        //    moveDir.Normalize();
        //}

        //--------------------------------------------------
        //Move 3    �ړ����@3�i���x�̓��j�A�u���`�vNon Stop(�����Ɠ���)�j
        //--------------------------------------------------
        IsPlayerMoving = mousePlayerDistance > MinDistanceToMove;
        if (IsPlayerMoving)
        {
            moveDir.x = mousePos.x - playerPosScreen.x;
            moveDir.y = 0.0f;
            moveDir.z = mousePos.y - playerPosScreen.y;
            moveDir.Normalize();
        }
        //�}�E�X�̏��ɉ�]����
        lookRot = Quaternion.LookRotation(moveDir);
    }

    void FixedUpdate()
    {
        //Tools�i���؋@�j��Collider�����邩��
        //���炭�ǂȂǂɓ��������Ƃ�Rigidbody�̑��x���e������̂�
        //������ velocity �� angularVelocity�@�����Z�b�g����
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (IsInputDisabled)
        {
            return;
        }
        //---------------------------------------------------------------
        //�@�ړ����@�@�P�i���ڃ}�E�X�����ֈړ��i��]�͌����ڂ����j�j
        //---------------------------------------------------------------
        transform.position += moveDir * MovementSpeed * Time.fixedDeltaTime;
        transform.rotation = lookRot;

        //---------------------------------------------------------------
        //�@�ړ����@�@�Q
        //�@�v���C���[�̌����Ă�������ֈړ�
        //  ��]�͏�������
        //---------------------------------------------------------------
        //transform.position += transform.forward * movementSpeed * Time.fixedDeltaTime;
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * rotationSpeed);
    }


    //�ǂ����̈ړ����x���g���̂͂��̊֐��̒��Ō��߂�
    void MovementSpeedUpdate()
    {
        if (!IsPlayerMoving)
        {
            MovementSpeed = 0.0f;
            return;
        }

        MovementSpeed = NormalMovementSpeed;
        if (playerSkill.IsUseSkill)
        {
            MovementSpeed = SkillMovementSpeed;
            return;
        }

        if (playerEnergy.IsUseEnergy && playerTool.IsToolCuttingGrass)
        {
            MovementSpeed = 0.0f;
            return;
        }

        if (playerTriggerBody.IsInsideBush)
        {
            MovementSpeed = BushMovementSpeed;
            return;
        }
    }

    //�v���C���[��SFX���Đ�����̔��f
    void PlayerWalkRunSoundHandler(bool isPlay = true)
    {
        if (isPlay)
        {
            if (playerTool.IsToolCuttingGrass)
            {
                sfxPlayWalk.StopSFXSound();
                sfxPlayRun.StopSFXSound();
            }
            else if (playerSkill.IsUseSkill)
            {
                sfxPlayWalk.StopSFXSound();
                sfxPlayRun.PlaySFXSound();
            }
            else
            {
                sfxPlayRun.StopSFXSound();
                sfxPlayWalk.PlaySFXSound();
            }
        }
        else
        {
            sfxPlayWalk.StopSFXSound();
            sfxPlayRun.StopSFXSound();
        }
    }
}
