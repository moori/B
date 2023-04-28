using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public bool isShaking;
    public float duration;
    public float intensity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    [ContextMenu("Shake")]
    public void Shake()
    {
        Shake(this.duration, this.intensity);
    }

    public void Shake(float duration, float intensity)
    {
        if (isShaking) return;
        StartCoroutine(ShakeRoutine(duration, intensity));
    }

    public IEnumerator ShakeRoutine(float duration, float intensity)
    {
        isShaking = true;
        var startTime = Time.time;
        Vector3 originalPosition = transform.position;
        while (Time.time - startTime < duration)
        {
            var pos = originalPosition + Random.insideUnitSphere * intensity;
            transform.position = new Vector3(pos.x, pos.y, -10);
            yield return new WaitForEndOfFrame();
        }
        isShaking = false;
        transform.position = originalPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Shake();
        }
    }
}
