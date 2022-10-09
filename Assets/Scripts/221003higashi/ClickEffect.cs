using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;


    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        //ps = GetComponentInChildren<ParticleSystem>();

        ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            mousePos.z = 3f;
            Instantiate(ps, Camera.main.ScreenToWorldPoint(mousePos), Quaternion.identity);

            ps.Play();
        }
    }
}
