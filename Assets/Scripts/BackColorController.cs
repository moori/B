using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackColorController : MonoBehaviour
{
    public Gradient idleGradient;
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public IEnumerator Start()
    {
        sr.DOGradientColor(idleGradient, 9f).SetLoops(-1, LoopType.Restart);
        yield return null;
    }
}
