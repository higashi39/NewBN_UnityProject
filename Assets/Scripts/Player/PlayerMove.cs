using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [field: Header("Movement Speed Settings")]
    // スキルを使うとき速度
    [field: SerializeField] float SkillMovementSpeed { set; get; } = 5.0f;
    //最大移動速度
    [field: SerializeField] float NormalMovementSpeed { set; get; } = 3.0f;
    //草に入る速度
    [field: SerializeField] float BushMovementSpeed { set; get; } = 1.25f;
    //回転速度
    //[field: SerializeField] float RotationSpeed { set; get; } = 3.0f;


    [field: Header("Player Move Settings")]
    //プレイヤーが移動するためにマウスと最低距離（画面座標）
    [field: SerializeField] float MinDistanceToMove { set; get; } = 10.0f;

    [field: Header("Player Move Direction")]
    //移動方向
    [field: SerializeField] Vector3 moveDir;
    //回転
    [field: SerializeField] Quaternion lookRot;

    [field: Header("Player Properties")]
    //入力請けいるかどうか
    [field: SerializeField] bool isInputDisabled;

    //最大移動速度
    [field: SerializeField] public float MovementSpeed { private set; get; } = 3.0f;
    //プレイヤー動くかどうか（MinDistanceToMoveによって）
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
        IsPlayerMoving = mousePlayerDistance > MinDistanceToMove;
        if (IsPlayerMoving)
        {
            moveDir.x = mousePos.x - playerPosScreen.x;
            moveDir.y = 0.0f;
            moveDir.z = mousePos.y - playerPosScreen.y;
            moveDir.Normalize();
        }
        //マウスの所に回転する
        lookRot = Quaternion.LookRotation(moveDir);
    }

    void FixedUpdate()
    {
        //Tools（草切機）がColliderがあるから
        //恐らく壁などに当たったときRigidbodyの速度が影響するので
        //だから velocity と angularVelocity　をリセットする
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (IsInputDisabled)
        {
            return;
        }
        //---------------------------------------------------------------
        //　移動方法　１（直接マウス方向へ移動（回転は見た目だけ））
        //---------------------------------------------------------------
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


    //どっちの移動速度を使うのはこの関数の中で決める
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

    //プレイヤーのSFXを再生するの判断
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
