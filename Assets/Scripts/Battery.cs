using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Battery : MonoBehaviour
{
    public Transform modelTransform;

    public float speed = 2;
    public float percentCharge = 1;
    public Vector3 moveDirection;

    private void OnEnable()
    {
        modelTransform.DOLocalRotate(new Vector3(30, 90, 120), 1f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    internal void Collect()
    {
        gameObject.SetActive(false);
    }
}
