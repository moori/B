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

    public void Shake(float duration, float intensity)
    {
        if (isShaking) return;
        isShaking = true;
        StartCoroutine(ShakeRoutine(duration, intensity));
    }

    public IEnumerator ShakeRoutine(float duration, float intensity)
    {
        var startTime = Time.time;
        //Vector3 originalPosition = transform.position;
        while (Time.time - startTime < duration)
        {
            var pos =  Random.insideUnitSphere * intensity;
            transform.localPosition = new Vector3(pos.x, pos.y, -10);
            yield return new WaitForEndOfFrame();
        }
        isShaking = false;
        transform.localPosition = new Vector3(0,0,-10);
    }

}
