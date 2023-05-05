using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AmmoBar : MonoBehaviour
{
    public List<Image> images;
    public List<Image> baseImages;
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

    public void UpdateBar(int ammo, int maxAmmo)
    {
        currentColor = ammo <= Player.BASE_MAX_AMMO * .2f ? lowHP : Color.white;

        foreach (var item in images)
        {
            //item.fillAmount = ammo / (float)Player.ABSOLUTE_MAX_AMMO;
            item.fillAmount = Mathf.Lerp(0f, 0.5f, ammo / (float)Player.ABSOLUTE_MAX_AMMO);
        }
        foreach (var item in baseImages)
        {
            if (!isFlashing)
                item.color = currentColor;
        }
    }


    public void Flash(Color color, float duration)
    {
        isFlashing = true;
        foreach (var item in baseImages)
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
