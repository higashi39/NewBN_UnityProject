using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBush : MonoBehaviour
{
    const float CutBushTimeMin = 0.45f;
    const float CutBushTimeMax = 0.80f;
    const float ShakePower = 0.085f;

    [field: SerializeField] float skillGetValue;
    public float SkillGetValue
    {
        private set { skillGetValue = value; }
        get { return skillGetValue * CutGrassTimeOri; }
    }

    //Per Second
    [field: SerializeField] public float EnergyConsumeValue { private set; get; }
    [field: SerializeField] public bool IsCuttedNow { private set; get; }
    [field: SerializeField] bool IsBushAlive { set; get; } = true;
    [field: SerializeField] Vector3 OriPos { set; get; }

    [field: SerializeField] SphereCollider col;

    [field: SerializeField] AnimationCurve sizeCurve;
    float tTime = 0.0f;
    [field: SerializeField] public float AnimationTime { private set; get; } = 0.55f;

    [field: SerializeField] float CutGrassTime { set; get; } = 0.0f;
    [field: SerializeField] float CutGrassTimeOri { set; get; } = 0.0f;

    [field: SerializeField] GameObject objModel;

    // Start is called before the first frame update
    void Start()
    {
        CutGrassTime = CutGrassTimeOri = Random.Range(CutBushTimeMin, CutBushTimeMax);

        objModel = transform.Find("Model").gameObject;
        SphereCollider targetCol = objModel.GetComponent<SphereCollider>();
        col = GetComponent<SphereCollider>();
        col.center = targetCol.center;
        col.radius = targetCol.radius;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        IsCuttedNow = false;
    }

    void DestroyBushes()
    {
        col.enabled = false;
        IsBushAlive = false;
        StartCoroutine(AnimDestroyBushes());
    }

    IEnumerator AnimDestroyBushes()
    {
        tTime = 0.0f;
        Vector3 oriSize = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 targetSize = new Vector3(0.0f, 0.0f, 0.0f);
        while (tTime < AnimationTime)
        {
            Vector3 nextSize = Vector3.one * sizeCurve.Evaluate(tTime);
            objModel.transform.localScale = nextSize;
            tTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

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