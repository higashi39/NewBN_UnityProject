using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentGenerator : MonoBehaviour
{
    [field: Header("Present Generator Settings")]
    //プレゼントを出る可能性「％」
    //最低　5％　～　100％
    //12草は100％
    [field: SerializeField] AnimationCurve presentAppearChance;

    [field: Header("Present Generator Properties")]
    //刈った草数（プレゼントが出たら、０に戻る）
    [field: SerializeField] int CuttedGrassCount { set; get; }

    [field: Header("Prefab")]
    [field: SerializeField] GameObject prefPresent;

    [field: Header("Script References")]
    [field: SerializeField] GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    //プレゼントを生成する関数
    //posCuttedは刈った草の座標
    //プレゼントが出なかったら➝次のプレゼント出る可能性が増やす（++CuttedGrassCount　➝　AnimationCurve presentAppearChanceはCuttedGrassCountの値を使う）
    //出たらCuttedGrassCountは0にリセット
    //GameManagerに報告
    public bool GeneratePresent(Vector3 posCutted)
    {
        float presentAppearPercentage = presentAppearChance.Evaluate(CuttedGrassCount);
        float randomAppearPercentage = Random.Range(0.0f, 100.0f);
        ++CuttedGrassCount;
        if (randomAppearPercentage < presentAppearPercentage)
        {
            CuttedGrassCount = 0;
            {
                gameManager.SetGameStatusPresentAppear();

                GameObject obj = Instantiate(prefPresent);
                posCutted.y += 0.5f;
                obj.transform.position = posCutted;
                return true;
            }
        }
        return false;

    }



}
