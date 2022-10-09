using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentBoxDisplay : MonoBehaviour
{

    [field: SerializeField] float RotationSpeed { set; get; } = 7.0f;
    [field: SerializeField] float UpSpeed { set; get; } = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);

        Vector3 newPos = transform.position;
        newPos.y += UpSpeed * Time.fixedDeltaTime;
        transform.position = newPos;
    }

    public void StopMove()
    {
        RotationSpeed = 0.0f;
        UpSpeed = 0.0f;
    }
}
