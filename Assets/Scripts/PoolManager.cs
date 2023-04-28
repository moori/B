using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public Pool hitParticlePool;
    public Pool playerBulletPool;
    public Pool explosionParticletPool;

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

    public ParticleSystem GetHitParticle()
    {
        return hitParticlePool.GetItem().GetComponent<ParticleSystem>();
    }
    public Bullet GetPlayerBullet()
    {
        return playerBulletPool.GetItem().GetComponent<Bullet>();
    }
    public ParticleSystem GetExplosionParticles()
    {
        return explosionParticletPool.GetItem().GetComponent<ParticleSystem>();
    }
}
