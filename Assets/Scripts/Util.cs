using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Util
{
    public static void DelayAction (this MonoBehaviour mb, float delay, System.Action onComplete)
    {
        mb.StartCoroutine(DelayActionRoutine(delay, onComplete));
    }
    public static IEnumerator DelayActionRoutine(float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onComplete();
    }
    public static void TweenTo(this MonoBehaviour mb, float fromValue, float toValue, float duration, System.Action<float> onUpdate)
    {
        mb.StartCoroutine(TweenToRoutine(fromValue, toValue,duration, onUpdate));
    }
    public static IEnumerator TweenToRoutine(float fromValue, float toValue, float duration, System.Action<float> onUpdate)
    {

        var startTime = Time.time;
        var tempValue= fromValue;
        while (Time.time-startTime<duration)
        {
            tempValue = Mathf.Lerp(fromValue, toValue, (Time.time - startTime) / duration);
            onUpdate(tempValue);
            yield return null;
        }
    }

    public static T GetRandom<T>(this IList<T> list)
    {
        if (list == null) return default(T);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
