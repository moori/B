using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AmmoBar : MonoBehaviour
{
    public List<Image> images;
    public Color lowHP;
    private Color currentColor;
    private bool isFlashing;

    public void Recharge(float percent)
    {

        if (percent < 0)
            Flash(Color.red, 0.3f);
        else
            Flash(Color.magenta, 0.6f);
    }

    public void UpdateBar(float percent)
    {
        currentColor = percent <= .2f ? lowHP : Color.white;
        foreach (var item in images)
        {
            item.fillAmount = percent;
            if (!isFlashing)
                item.color = currentColor;
        }
    }


    public void Flash(Color color, float duration)
    {
        isFlashing = true;
        foreach (var item in images)
        {
            item.DOColor(color, duration * 0.5f).OnComplete(() =>
            {
                item.DOColor(currentColor, duration * 0.5f).OnComplete(() =>
                {
                    isFlashing = false;
                });
            });
        }
    }
}
