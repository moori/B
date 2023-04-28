using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitTarget : MonoBehaviour
{

    public UnityEvent<int> OnHit;

    public void Hit(int damage)
    {
        OnHit?.Invoke(damage);
    }
}
