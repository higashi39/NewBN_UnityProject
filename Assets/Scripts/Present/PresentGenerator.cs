using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentGenerator : MonoBehaviour
{
    // 5% - 100%
    // pity = 9 -> 100%
    [field: SerializeField] AnimationCurve presentAppearChance;
    [field: SerializeField] int CuttedGrassCount { set; get; }

    [field: SerializeField] GameObject prefPresent;

    [field: SerializeField] GameManager gameManager;

    // ----------------------------
    // add higashi
    // ----------------------------
    [Header("References")]
    NewPresentBoxManager newPresentBoxManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        newPresentBoxManager = FindObjectOfType<NewPresentBoxManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GeneratePresent(Vector3 posCutted)
    {

        float presentAppearPercentage = presentAppearChance.Evaluate(CuttedGrassCount);
        float randomAppearPercentage = Random.Range(0.0f, 100.0f);
        ++CuttedGrassCount;
        Debug.Log("Random : " + randomAppearPercentage + " ,Appear : " + presentAppearPercentage);
        if (randomAppearPercentage < presentAppearPercentage)
        {
            CuttedGrassCount = 0;
            //Make present appear
            Debug.Log("Present Appear");
            {
                gameManager.SetGameStatusPresentAppear();

                GameObject obj = Instantiate(prefPresent);
                posCutted.y += 0.5f;
                float randomRotY = Random.Range(0.0f, 360.0f);

                obj.transform.position = posCutted;
                obj.transform.Rotate(0.0f, randomRotY, 0.0f);

                StartCoroutine(BoxAppearAnimation(obj));
            }
        }

    }

    IEnumerator BoxAppearAnimation(GameObject present)
    {
        PresentBoxDisplay presentBoxDisplay = present.GetComponent<PresentBoxDisplay>();
        yield return new WaitForSeconds(1.5f);
        presentBoxDisplay.StopMove();

        //-----------------------------------------
        // Call That Scene
        //-----------------------------------------
        // ----------------------------
        // add higashi
        // ----------------------------
        newPresentBoxManager.pnlPresentScene.SetActive(true);
        newPresentBoxManager.presentSceneSetActive = true;

        yield return null;
    }

    public void PlayBoxDisappearAnimation()
    {
        StartCoroutine(BoxDisappearAnimation());
    }


    IEnumerator BoxDisappearAnimation()
    {
        yield return null;
    }
}
