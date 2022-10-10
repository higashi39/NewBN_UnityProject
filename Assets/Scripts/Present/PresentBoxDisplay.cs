using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PresentBoxDisplay : MonoBehaviour
{
    enum PRESENT_ANIMATION
    {
        PRESENT_BOX_APPEAR,
        PRESENT_BOX_WAIT,
        PRESENT_BOX_DISSAPEAR
    }

    //Presentが上にいくと回転する
    [field: Header("Present Box Settings")]
    //回転速度
    [field: SerializeField] float RotationSpeed { set; get; } = 7.0f;
    //上へ行く速度（Y軸）
    [field: SerializeField] float UpSpeed { set; get; } = 1.0f;
    //サイズのカーブ（プレゼントの終わるアニメーションに使う「AnimationDisappear()」）
    [field: SerializeField] AnimationCurve SizeCurve { set; get; }


    [field: Header("Present Box Properties")]
    //プレゼントのアニメーション状態
    [field: SerializeField] PRESENT_ANIMATION PresentAnimation { set; get; } = PRESENT_ANIMATION.PRESENT_BOX_APPEAR;
    //最初の回転速度
    [field: SerializeField] float RotationSpeedOri { set; get; } = 7.0f;



    private void Start()
    {
        RotationSpeedOri = RotationSpeed;
        //最初の回転をランダム
        float randomRotY = Random.Range(0.0f, 360.0f);
        transform.Rotate(0.0f, randomRotY, 0.0f);
    }

    private void FixedUpdate()
    {
        if (PresentAnimation == PRESENT_ANIMATION.PRESENT_BOX_WAIT)
        {
            return;
        }

        //回転する（Y軸に回転する）と上へ行く（Y軸）
        transform.Rotate(Vector3.up, RotationSpeed * Time.fixedDeltaTime);
        Vector3 newPos = transform.position;
        newPos.y += UpSpeed * Time.fixedDeltaTime;
        transform.position = newPos;
    }

    //アニメーションを待つ
    //このアニメーションはプレゼントが出て、数秒ぐらいの後止まる
    //その数秒ぐらいについてはGameManager「[field: Header("GameA Animation Settings")]➝float AnimationPresentAppearWaitTime」
    //で設定する「Inspectorで設定ください」
    public void SetAnimationWait()
    {
        PresentAnimation = PRESENT_ANIMATION.PRESENT_BOX_WAIT;
        RotationSpeed = 0.0f;
        UpSpeed = 0.0f;
    }

    //プレゼントをもらう後、箱が消える
    //そのために消えるアニメションを呼ぶ
    //物体の回転を続けて、ただし、もう上へ行かない
    public void SetAnimationDisappear()
    {
        PresentAnimation = PRESENT_ANIMATION.PRESENT_BOX_DISSAPEAR;
        RotationSpeed = RotationSpeedOri;
        StartCoroutine(AnimationDisappear());
    }

    //箱の消えるアニメション
    //物体のサイズを大きく小さくする
    IEnumerator AnimationDisappear()
    {
        float tTime = 0.0f;
        float lastKeyFrameTime = SizeCurve[SizeCurve.length - 1].time;
        while (tTime < lastKeyFrameTime)
        {
            Vector3 nextSize = Vector3.one * SizeCurve.Evaluate(tTime);
            transform.localScale = nextSize;
            tTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
        Destroy(gameObject);
    }
}
