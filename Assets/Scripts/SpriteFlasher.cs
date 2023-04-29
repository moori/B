using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlasher : MonoBehaviour
{
    [Header("Flash")]
    private SpriteRenderer sprite;
    private Color originalColor;
    private bool isFlashing;

    public Color color = Color.red;
    public int amount = 4;
    public float flashDuration =0.4f;

    [Header("Shake")]
    public float intensity;
    public Vector3 originalLocalPosition;

    public void Setup(Color color, int amount, float flashDuration)
    {
        this.color = color;
        this.amount = amount;
        this.flashDuration = flashDuration;
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
        originalLocalPosition = transform.localPosition;
    }

    public void Flash()
    {
        if (isFlashing) return;

        StartCoroutine(FlashRoutine());
    }

    private void OnEnable()
    {
        sprite.transform.localPosition = originalLocalPosition;
        sprite.color = originalColor;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        sprite.transform.localPosition = originalLocalPosition;
        sprite.color = originalColor;
    }

    private IEnumerator FlashRoutine()
    {
        var t = (flashDuration / (float)amount) * 0.5f;
        if (intensity >= 0) sprite.transform.DOShakePosition(flashDuration, intensity);
        for (int i = 0; i < amount; i++)
        {
            sprite.color = color;
            yield return new WaitForSeconds(t);
            sprite.color = originalColor;
            yield return new WaitForSeconds(t);
        }
        sprite.transform.localPosition = originalLocalPosition;
        yield return null;
    }
}
