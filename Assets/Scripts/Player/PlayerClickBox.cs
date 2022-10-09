using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerClickCheck();
    }

    void PlayerClickCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100.0f);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if(hit.transform.gameObject.CompareTag("PresentBox"))
                {
                    PresentBox box = hit.transform.GetComponent<PresentBox>();

                    box.clickedObj = hit.collider.gameObject;
                    box.OpenBox();
                    break;
                }
            }

        }
    }

}
