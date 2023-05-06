using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pulse : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    public SpriteRenderer sprite;
    public float duration;
    public float fromScale;
    public float toScale;
    [Header("menu")]
    public bool isRepeat;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        if (isRepeat)
        {
            PulseRepeat();
        }
    }

    public void PulseRepeat()
    {
        StartPulse(PulseRepeat);
    }

    public void StartPulse(Color startColor, Color endColor,float duration, float fromScale, float toScale, System.Action callback)
    {
        this.startColor = startColor;
        this.endColor = endColor;
        this.duration = duration;
        this.fromScale = fromScale;
        this.toScale = toScale;
        StartPulse(callback);
    }

    public void StartPulse()
    {
        StartPulse(null);
    }

    public void StartPulse(System.Action callback)
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
            callback?.Invoke();
        });
    }

    private Color SetAlpha(Color color,  float value)
    {
        return new Color(color.r, color.g, color.b, value);
    }
}
