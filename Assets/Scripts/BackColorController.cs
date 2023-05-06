using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackColorController : MonoBehaviour
{
    public static BackColorController instance;

    public Gradient idleGradient;
    public Gradient bossGradient;
    private SpriteRenderer sr;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void Start()
    {
        SetIdle();
    }

    public void Flash(Color color, System.Action callback)
    {
        sr.DOKill();
        var originalColor = sr.color;
        var halfDuration = .15f;
        sr.DOColor(color, halfDuration).OnComplete(()=> {
            sr.DOColor(originalColor, halfDuration).OnComplete(() => {
                callback?.Invoke();
            });
        });
    }

    public void SetIdle()
    {
        sr.DOKill();
        sr.DOColor(idleGradient.Evaluate(0f), 1f).OnComplete(()=> { 
            sr.DOGradientColor(idleGradient, 9f).SetLoops(-1, LoopType.Restart);
        });
    }

    public void SetBoss()
    {
        sr.DOKill();
        sr.DOColor(bossGradient.Evaluate(0f), 0.3f).OnComplete(() =>
        {
            sr.DOGradientColor(bossGradient, 3f).SetLoops(-1, LoopType.Restart);
        });
    }
}
