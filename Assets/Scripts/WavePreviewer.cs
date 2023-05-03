using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePreviewer : MonoBehaviour
{
    public SpawnGroup spawnGroup;

    public int regularWaveSelector;

    private void Awake()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (spawnGroup == null) return;

        try
        {
            var regWave = spawnGroup.spawns;
            foreach (var item in regWave)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(item.point, Mathf.Clamp( item.radius,0.2f,12f));
            }
        }catch
        {

        }
    }

}
