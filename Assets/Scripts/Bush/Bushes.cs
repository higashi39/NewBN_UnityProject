using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUSHES_LEVEL
{
    BUSHES_LEVEL_SMALL,
    BUSHES_LEVEL_MEDIUM,
    BUSHES_LEVEL_BIG,
    BUSHES_LEVEL_COUNT,
}

public class Bushes : MonoBehaviour
{
    [field: SerializeField] public float SkillGetValue { private set; get; }
    [field: SerializeField] public float EnergyConsumeValue { private set; get; }
    [field: SerializeField] public BUSHES_LEVEL BushesLevel { private set; get; }

    [field: SerializeField] public float AnimationTime { private set; get; } = 1.5f;

    [field: SerializeField] AnimationCurve sizeCurve;
    float tTime;

    GameObject[] objModelLevel;
    SphereCollider col;

    void Start()
    {

    }

    public void Init()
    {
        objModelLevel = new GameObject[(int)BUSHES_LEVEL.BUSHES_LEVEL_COUNT];
        objModelLevel[(int)BUSHES_LEVEL.BUSHES_LEVEL_SMALL] = transform.Find("Level_1").gameObject;
        objModelLevel[(int)BUSHES_LEVEL.BUSHES_LEVEL_MEDIUM] = transform.Find("Level_2").gameObject;
        objModelLevel[(int)BUSHES_LEVEL.BUSHES_LEVEL_BIG] = transform.Find("Level_3").gameObject;
        for (int i = 0; i < objModelLevel.Length; ++i)
        {
            objModelLevel[i].SetActive(false);
        }
        col = GetComponent<SphereCollider>();
    }

    public void SetBushedLevel(int level)
    {
        objModelLevel[level].SetActive(true);
        SetColliderValue(level);
    }

    void SetColliderValue(int targetLevel)
    {
        SphereCollider targetCol = objModelLevel[targetLevel].GetComponent<SphereCollider>();
        col.center = targetCol.center;
        col.radius = targetCol.radius;
    }

    public void CheckDestroyBush()
    {
        int nowLevel = (int)BushesLevel;
        int nextLevel = nowLevel - 1;
        //if (nextLevel < 0)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        StartCoroutine(AnimNextModel(nowLevel));

        if (nextLevel >= 0)
        {
            objModelLevel[nextLevel].SetActive(true);
            SetBushedLevel(nextLevel);
            BushesLevel = (BUSHES_LEVEL)nextLevel;
        }

    }

    IEnumerator AnimNextModel(int index)
    {
        tTime = 0.0f;
        Vector3 oriSize = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 targetSize = new Vector3(0.0f, 0.0f, 0.0f);

        while (tTime < AnimationTime)
        {
            // Vector3 nextSize = Vector3.Lerp(oriSize, targetSize, sizeCurve.Evaluate(tTime / AnimationTime));
            Vector3 nextSize = Vector3.one * sizeCurve.Evaluate(tTime);
            tTime += Time.deltaTime;
            objModelLevel[index].transform.localScale = nextSize;
            yield return null;
        }
        objModelLevel[index].SetActive(false);
        if (index == 0)
        {
            Destroy(gameObject);
        }
    }
}
