using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FillImageHelper : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image border;
    public Image fill;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        fill.fillAmount = 0f;
        canvasGroup.DOFade(1, 0.5f);
    }

    public void Hide()
    {
        canvasGroup.DOFade(0, 0.5f);
    }


    public void SetFill(float amount)
    {
        SetFill(amount , 0f);
    }

    public void SetFill(float amount, float duration = 0f)
    {
        if (duration > 0)
        {
            fill.DOFillAmount(amount, duration);
        }
        else
        {
            fill.fillAmount = amount;
        }
    }
}
