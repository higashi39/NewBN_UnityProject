using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerBody : MonoBehaviour
{
    [field: SerializeField] public bool IsInsideBush { set; get; } = false;

    int bushCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            IsInsideBush = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            IsInsideBush = false;
        }
    }

}
