using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BatteryTimer : MonoBehaviour
{
    public SpriteRenderer border;
    public SpriteRenderer fill;


    public float lifeTime;
    public bool countdownToFull;

    public UnityEvent OnExpire;
    private bool isActive;
    private float startTime;

    private float fullScale = 1.1f;
    public bool isExpired;

    private void Update()
    {
        if (!isActive) return;

        if (Time.time - startTime < lifeTime)
        {
            if (countdownToFull)
                fill.transform.localScale = Vector3.one * Mathf.Lerp(0, fullScale, (Time.time - startTime) / lifeTime);
            else
                fill.transform.localScale = Vector3.one * Mathf.Lerp(fullScale, 0, (Time.time - startTime) / lifeTime);
        }
        else
        {
            Expire();
        }
    }

    private void Expire()
    {
        isActive = false;
        isExpired = true;
        OnExpire?.Invoke();
    }

    public void Setup()
    {
        gameObject.SetActive(true);
        isExpired = false;
        fill.transform.localScale = countdownToFull ? Vector3.zero : Vector3.one * fullScale;
    }

    public void StartTimer(float lifetime)
    {
        this.lifeTime = lifetime;
        startTime = Time.time;
        isActive = true;
    }
}
