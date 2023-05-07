using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Battery : MonoBehaviour
{
    public Transform modelTransform;

    public float speed = 2;
    public float percentCharge = 1;
    public Vector3 moveDirection;
    public bool isActive;
    public UnityEvent OnSpawn;
    public ParticleSystem bubbleResonancePart;
    private void OnEnable()
    {
        OnBubbleEnable(Player.isBubbling);
        modelTransform.DOLocalRotate(new Vector3(30, 90, 120), 1f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        Player.OnBubbleEnable += OnBubbleEnable;
    }
    private void OnDisable()
    {
        Player.OnBubbleEnable -= OnBubbleEnable;
    }

    private void Update()
    {
        if (!isActive) return;

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void OnBubbleEnable(bool isEnabled)
    {
        bubbleResonancePart.gameObject.SetActive(isEnabled);
    }

    public void SpawnBattery(Vector3 moveDir)
    {
        isActive = true;
        gameObject.SetActive(true);
        moveDirection = moveDir;
        OnSpawn?.Invoke();
    }

    public void SpawnBattery()
    {
        SpawnBattery(Vector3.zero);
    }

    internal void Collect()
    {
        gameObject.SetActive(false);
        isActive = false;
        OnBubbleEnable(false);
    }
}
