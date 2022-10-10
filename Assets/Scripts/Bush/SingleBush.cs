using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBush : MonoBehaviour
{
    [field: Header("Bush Settings")]
    //草を刈る時間のランダム（最低）
    [field: SerializeField] float CutBushTimeMin { set; get; } = 0.65f;
    //草を刈る時間のランダム（最大）
    [field: SerializeField] float CutBushTimeMax { set; get; } = 0.95f;
    //草を刈っている震えるの力（座標ずらす）
    [field: SerializeField] float ShakePower { set; get; } = 0.075f;
    //草を刈るためのEnergy（この値は Time.deltaTIme「PlayerTool(Update関数)->PlayerEnergy（ActionCutGrass(float energyNeeded)）」　）
    [field: SerializeField] public float EnergyConsumeValue { private set; get; }
    //草を刈った後、もらったスキルゲージ値
    [field: SerializeField] float skillGetValue;
    public float SkillGetValue
    {
        private set { skillGetValue = value; }
        get { return skillGetValue * CutGrassTimeOri; }
    }

    [field: Header("Bushes Animation")]
    //刈った後アニメションのサイズ（大きく小さく）
    [field: SerializeField] AnimationCurve sizeCurve;

    [field: Header("Bushes Property")]
    //今刈っているかどうか（草を震えるのため）
    [field: SerializeField] public bool IsCuttedNow { private set; get; }
    //草まだ生きているかどうか
    [field: SerializeField] bool IsBushAlive { set; get; } = true;
    //草の最初の座標（置いている座標）
    [field: SerializeField] Vector3 OriPos { set; get; }
    //今・残り草を刈り時間
    [field: SerializeField] float CutGrassTime { set; get; } = 0.0f;
    //最初のランダムした時間
    [field: SerializeField] float CutGrassTimeOri { set; get; } = 0.0f;


    [field: Header("Model References")]
    [field: SerializeField] GameObject objModel;

    [field: Header("Component References")]
    [field: SerializeField] SphereCollider col;

    // Start is called before the first frame update
    void Start()
    {
        //草を刈る時間をランダム
        CutGrassTime = CutGrassTimeOri = Random.Range(CutBushTimeMin, CutBushTimeMax);

        //草のモデルによってColliderを設定する
        objModel = transform.Find("Model").gameObject;
        SphereCollider targetCol = objModel.GetComponent<SphereCollider>();
        col = GetComponent<SphereCollider>();
        col.center = targetCol.center;
        col.radius = targetCol.radius;
    }

    private void LateUpdate()
    {
        IsCuttedNow = false;
    }

    //草を破壊する
    //草のColliderとIsBushAliveはfalseに設定
    //草の破壊したアニメションを再生「AnimDestroyBushes()」
    void DestroyBushes()
    {
        col.enabled = false;
        IsBushAlive = false;
        StartCoroutine(AnimDestroyBushes());
    }

    //草を破壊したアニメション
    //草は大きく小さくする
    //アニメションが終わったら物体を破壊する
    IEnumerator AnimDestroyBushes()
    {
        float tTime = 0.0f;
        float lastKeyFrameTime = sizeCurve[sizeCurve.length - 1].time;
        while (tTime < lastKeyFrameTime)
        {
            Vector3 nextSize = Vector3.one * sizeCurve.Evaluate(tTime);
            objModel.transform.localScale = nextSize;
            tTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
        Destroy(gameObject);
    }

    //草を揺れる関数
    //揺れる座標XとZ座標
    //揺れる力は ShakePower　から設定する（[field: Header("Bush Settings")]の中）
    void ShakeBushes()
    {
        Vector3 extraPos;
        extraPos.x = Random.Range(-ShakePower, ShakePower);
        extraPos.y = 0.0f;
        extraPos.z = Random.Range(-ShakePower, ShakePower);

        Vector3 myPos = transform.position;

        Vector3 targetPos = myPos + extraPos;

        objModel.transform.position = targetPos;
    }

    //草を刈る関数
    //isInstanはプレイヤーが今スキルを使っているかどうか
    //スキルが使っている場合isInstanはtrue➝草のはすぐ破壊する「DestroyBushes()を呼ぶ」
    //isInstanはfalse➝草の　CutGrassTime　を減らす、その時も草を揺れる
    public bool CutBushes(bool isInstan = false)
    {
        if (!isInstan)
        {
            IsCuttedNow = true;
            CutGrassTime -= Time.deltaTime;
            ShakeBushes();
        }
        else
        {
            CutGrassTime = 0.0f;
        }

        if (CutGrassTime <= 0.0f)
        {
            DestroyBushes();
            return true;
        }
        return false;
    }

}