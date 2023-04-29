using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pulse : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    private SpriteRenderer sprite;
    public float duration;
    public float fromScale;
    public float toScale;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void StartPulse()
    {
        //sprite.color = SetAlpha(sprite.color,0);
        //sprite.DOGradientColor(colorOverLifetime, duration);
        //sprite.DOFade(1, duration);
        sprite.color = startColor;
        sprite.DOColor(endColor, duration);
        sprite.transform.localScale = Vector3.one * fromScale;
        gameObject.SetActive(true);
        sprite.transform.DOScale(toScale, duration).OnComplete(()=> {
            gameObject.SetActive(false);
        });
    }

    private Color SetAlpha(Color color,  float value)
    {
        return new Color(color.r, color.g, color.b, value);
    }
}
