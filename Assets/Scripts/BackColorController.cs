using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackColorController : MonoBehaviour
{
    public Gradient idleGradient;
    public Gradient bossGradient;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        SetIdle();
    }

    public void SetIdle()
    {
        sr.DOColor(idleGradient.Evaluate(0f), 1f).OnComplete(()=> { 
            sr.DOGradientColor(idleGradient, 9f).SetLoops(-1, LoopType.Restart);
        });
    }

    public void SetBoss()
    {
        sr.DOGradientColor(bossGradient, 3f).SetLoops(-1, LoopType.Restart);
    }
}
