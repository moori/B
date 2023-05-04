using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitTarget : MonoBehaviour
{

    public UnityEvent<int> OnHit;
    public float damageMultiplier=1f;

    public void Hit(int damage)
    {
        OnHit?.Invoke(Mathf.RoundToInt(damage*damageMultiplier));
    }
}
