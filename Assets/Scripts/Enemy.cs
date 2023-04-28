using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Color[] hpColors;
    private HealthComponent healthComponent;

    private void Awake()
    {
        if(TryGetComponent<HealthComponent>(out var h))
        {
            healthComponent = h;
            healthComponent.OnDie.AddListener(Die);
        }
    }

    private void OnDestroy()
    {
        healthComponent?.OnDie.RemoveListener(Die);
    }

    public void OnTakeDamage(int damage)
    {

    }

    public void Die()
    {
        //kaboom
        CameraController.instance.Shake(0.1f, 0.05f);
        var part = PoolManager.instance.GetExplosionParticles();
        part.transform.position = transform.position;
        part.gameObject.SetActive(true);
        Destroy(gameObject,0.01f);
    }
}
