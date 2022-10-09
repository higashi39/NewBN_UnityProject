using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentManager : MonoBehaviour
{
    [field: Header("Present Percentage")]
    [field: SerializeField] public float PresentAppearPercentage { private set; get; } = 70.0f;
    [field: SerializeField] public int PresentAppearMin { private set; get; } = 3;

    [field: SerializeField] int PresentAppearCount { set; get; } = 0;
    [field: SerializeField] static public int PresentAccummulated { private set; get; } = 0;

    [field: Header("Prefab")]
    [field: SerializeField] GameObject prefPresent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GeneratePresent(Vector3 spawn_pos)
    {
        bool isSpawnPresent = false;

        isSpawnPresent = Random.Range(0.0f, 100.0f) < PresentAppearPercentage ? true : false;

        if (!isSpawnPresent)
        {
            ++PresentAppearCount;
            if (PresentAppearCount != PresentAppearMin)
            {
                return;
            }
        }
        PresentAppearCount = 0;
        SpawnPresent(spawn_pos);
    }

    void SpawnPresent(Vector3 spawn_pos)
    {
        GameObject obj = Instantiate(prefPresent);
        obj.transform.position = spawn_pos;
    }


}
