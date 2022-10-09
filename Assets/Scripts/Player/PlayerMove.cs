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

    [field: SerializeField] public float NormalMovementSpeed { private set; get; } = 3.0f;   //最大移動速度
    [field: SerializeField] public float BushMovementSpeed { private set; get; } = 1.25f;
    [field: SerializeField] public float MovementSpeed { private set; get; } = 3.0f;   //最大移動速度

    [Header("Player MinMax DistanceToMove")]
    const float minDistanceToMove = 10.0f;      //プレイヤーが移動するためにマウスと最低距離

    [Header("Player Move Direction")]
    Vector3 moveDir;        //移動方向
    Quaternion lookRot;     //回転
    const float rotationSpeed = 3.0f;   //回転速度

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

        //  プレイヤーとマウスの画面座標を取得
        Vector2 playerPosScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mousePos = Input.mousePosition;

        //  プレイヤーとマウスの距離（画面座標、ワルド座標ではない）
        float mousePlayerDistance = Vector2.Distance(mousePos, playerPosScreen);

        //--------------------------------------------------
        //Move 1    移動方法１（加速と減速速度あり）
        //--------------------------------------------------
        //playerSharedData.MovementSpeed = speedCurve.Evaluate((mousePlayerDistance - minDistanceToMove) * scalerDistanceToMove) * maxMovementSpeed;
        //moveDir.x = mousePos.x - playerPosScreen.x;
        //moveDir.y = 0.0f;
        //moveDir.z = mousePos.y - playerPosScreen.y;
        //moveDir.Normalize();

        //--------------------------------------------------
        //Move 2    移動方法２（速度はリニア「線形」）
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
        //Move 3    移動方法3（速度はリニア「線形」Non Stop(ずっと動く)）
        //--------------------------------------------------
        isPlayerMoving = mousePlayerDistance > minDistanceToMove;
        if (isPlayerMoving)
        {
            moveDir.x = mousePos.x - playerPosScreen.x;
            moveDir.y = 0.0f;
            moveDir.z = mousePos.y - playerPosScreen.y;
            moveDir.Normalize();
        }
        //マウスの所に回転する
        lookRot = Quaternion.LookRotation(moveDir);
    }

    private void FixedUpdate()
    {
        //Tools（草切機）がColliderがあるから
        //恐らく壁などに当たったときRigidbodyの速度が影響するので
        //だから velocity と angularVelocity　をリセットする
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (!isCanMove)
        {
            return;
        }
        //---------------------------------------------------------------
        //　移動方法　１（直接マウス方向へ移動（回転は見た目だけ））
        //---------------------------------------------------------------
        //transform.position += moveDir * playerSharedData.MovementSpeed * Time.fixedDeltaTime;
        transform.position += moveDir * MovementSpeed * Time.fixedDeltaTime;
        transform.rotation = lookRot;

        //---------------------------------------------------------------
        //　移動方法　２
        //　プレイヤーの向いている方向へ移動
        //  回転は少しずつ
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
