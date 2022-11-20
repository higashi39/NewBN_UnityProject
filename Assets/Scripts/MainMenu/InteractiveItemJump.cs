using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractiveItemJump : MonoBehaviour
{
    [field: Header("Property")]
    [field: SerializeField] float JumpPower { set; get; }
    [field: SerializeField] float JumpDuration { set; get; }
    [field: SerializeField] bool isClickable { set; get; }

    [field: Header("Script References")]
    [field: SerializeField] ShakeBox shakeBox;

    private void Awake()
    {
        shakeBox = GetComponent<ShakeBox>();
    }


    private void OnMouseDown()
    {
        if (!isClickable) return;

        StartCoroutine(StartJumpAnimation());

    }

    IEnumerator StartJumpAnimation()
    {
        isClickable = false;

        if (shakeBox)
        {
            shakeBox.enabled = false;
        }

        transform.DOLocalJump(transform.position, JumpPower, 1, JumpDuration);


        yield return new WaitForSeconds(JumpDuration);
        isClickable = true;

        if (shakeBox)
        {
            shakeBox.enabled = true;
        }
    }

}
