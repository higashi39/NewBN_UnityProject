using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field: Header("Animation")]
    [field: SerializeField] bool IsHovered { set; get; } = false;
    [field: SerializeField] float AnimationTime { set; get; } = 0.15f;
    [field: SerializeField] bool IsDoScaleFlag { set; get; } = true;
    [field: SerializeField] Vector3 ScaleTarget { set; get; }
    [field: SerializeField] public bool IsCanAnimation { set; get; }


    // Update is called once per frame
    void Update()
    {
        //アニメションできないかアニメションフラグ（）が読んだ後何もしない
        if (!IsCanAnimation) return;
        if (IsDoScaleFlag) return;

        if (IsHovered)
        {
            transform.DOScale(ScaleTarget, AnimationTime).SetEase(Ease.InQuart);
        }
        else
        {
            transform.DOScale(Vector3.one, AnimationTime).SetEase(Ease.InQuart);
        }
        IsDoScaleFlag = true;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;
        IsDoScaleFlag = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
        IsDoScaleFlag = false;
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

}
