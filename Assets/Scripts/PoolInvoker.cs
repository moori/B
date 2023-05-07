using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolInvoker : MonoBehaviour
{
    public PoolType poolType;
    public enum PoolType
    {
        SmallRedExplosion
    }

    public void SpawnInPlace()
    {
        GameObject obj = null;

        switch (poolType)
        {
            case PoolType.SmallRedExplosion:
            obj = PoolManager.instance.GetSmallRedExplosionParticles().gameObject;
                break;
        }

        obj.transform.position = transform.position;
        obj.SetActive(true);
    }
}
