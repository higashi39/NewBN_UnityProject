using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //References
    PlayerSkillCrushAndRun playerSkill;
    PlayerTriggerBody playerTriggerBody;
    PlayerEnergy playerEnergy;
    PlayerTool playerTool;

    Rigidbody rb;

    [field: SerializeField] public float NormalMovementSpeed { private set; get; } = 3.0f;   //�ő�ړ����x
    [field: SerializeField] public float BushMovementSpeed { private set; get; } = 1.25f;
    [field: SerializeField] public float MovementSpeed { private set; get; } = 3.0f;   //�ő�ړ����x

    [Header("Player MinMax DistanceToMove")]
    const float minDistanceToMove = 10.0f;      //�v���C���[���ړ����邽�߂Ƀ}�E�X�ƍŒ዗��

    [Header("Player Move Direction")]
    Vector3 moveDir;        //�ړ�����
    Quaternion lookRot;     //��]
    const float rotationSpeed = 3.0f;   //��]���x

    [Header("Player Status")]
    public bool isCanMove;

    bool isPlayerMoving;

    // Start is called before the first frame update
    void Start()
    {
        MovementSpeed = 0.0f;
        playerSkill = GetComponent<PlayerSkillCrushAndRun>();
        playerTriggerBody = GetComponentInChildren<PlayerTriggerBody>();
        playerEnergy = GetComponent<PlayerEnergy>();
        playerTool = GetComponentInChildren<PlayerTool>();

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isCanMove)
        {
            return;
        }

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
        isPlayerMoving = mousePlayerDistance > minDistanceToMove;
        if (isPlayerMoving)
        {
            moveDir.x = mousePos.x - playerPosScreen.x;
            moveDir.y = 0.0f;
            moveDir.z = mousePos.y - playerPosScreen.y;
            moveDir.Normalize();
        }
        //�}�E�X�̏��ɉ�]����
        lookRot = Quaternion.LookRotation(moveDir);
    }

    private void FixedUpdate()
    {
        //Tools�i���؋@�j��Collider�����邩��
        //���炭�ǂȂǂɓ��������Ƃ�Rigidbody�̑��x���e������̂�
        //������ velocity �� angularVelocity�@�����Z�b�g����
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (!isCanMove)
        {
            return;
        }
        //---------------------------------------------------------------
        //�@�ړ����@�@�P�i���ڃ}�E�X�����ֈړ��i��]�͌����ڂ����j�j
        //---------------------------------------------------------------
        //transform.position += moveDir * playerSharedData.MovementSpeed * Time.fixedDeltaTime;
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

    public void SetIsCanMove(bool canMove = true)
    {
        isCanMove = canMove;
    }

    void MovementSpeedUpdate()
    {
        if (!isPlayerMoving)
        {
            MovementSpeed = 0.0f;
            return;
        }

        MovementSpeed = NormalMovementSpeed;
        if (playerSkill.IsUseSkill)
        {
            MovementSpeed = playerSkill.SkillMovementSpeed;
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
}
