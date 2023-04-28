using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public int maxHP;
    public int HP;

    private HitTarget hitTarget;

    public UnityEvent OnDie;

    private void Awake()
    {
        HP = maxHP;
        if (TryGetComponent<HitTarget>(out var target)){
            hitTarget = target;
            hitTarget.OnHit.AddListener(TakeDamage);
        }
    }

    private void OnDestroy()
    {
        if (hitTarget)
        {
            hitTarget.OnHit.RemoveListener(TakeDamage);
        }
    }


    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            OnDie?.Invoke();
        }
    }
}
