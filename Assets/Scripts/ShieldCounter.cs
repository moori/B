using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class ShieldCounter : MonoBehaviour
{
    public Image border;
    public Image fill;

    public Color emptyColor;
    public Color fullColor;
    public bool isFull;
    public bool isInUse;
    private float chargeStartTime;
    private float chargeDuration;
    private bool isCharging;


    private float useStartTime;
    private float useDuration;

    public UnityEvent OnEndUse;
    public UnityEvent OnEndRecharge;


    private void Start()
    {
        UpdateFill(0f);  
    }

    private void Update()
    {
        if (isCharging)
        {
            if(Time.time - chargeStartTime < chargeDuration)
            {
                UpdateFill((Time.time - chargeStartTime)/ chargeDuration);
                SetColor(emptyColor);
            }
            else
            {
                UpdateFill(1f);
                isFull = true;
                isCharging = false;
                SetColor(fullColor);
                OnEndRecharge?.Invoke();
            }
        }

        if (isInUse)
        {
            if (Time.time - useStartTime < useDuration)
            {
                UpdateFill(1f - ((Time.time - useStartTime) / useDuration));
                SetColor(fullColor);
            }
            else
            {
                UpdateFill(0);
                SetColor(emptyColor);
                isFull = false;
                isCharging = false;
                isInUse = false;
                OnEndUse?.Invoke();
            }
        }
    }

    public void UpdateFill(float percent)
    {
        fill.fillAmount = percent;
    }

    public void SetColor(Color color)
    {
        fill.color = color;
        border.color = color;
    }
    public void StartCharging(float duration)
    {
        chargeStartTime = Time.time;
        chargeDuration = duration;
        isCharging = true;
    }
    public void StartUsing(float duration)
    {
        useStartTime = Time.time;
        useDuration = duration;
        isInUse = true;
    }
}
