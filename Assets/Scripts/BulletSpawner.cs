using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public List<Vector3> fireLocalDirections;

    public float delayBetweenShots=0.035f;
    public float delayBetweenAttacks=2f;


    private void OnEnable()
    {
        StartCoroutine(FireAllRoutine());
    }

    public IEnumerator FireAllRoutine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            for (int i = 0; i < fireLocalDirections.Count; i++)
            {
                Shoot(fireLocalDirections[i]);
            }

            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }

    public void Shoot(Vector3 direction)
    {
        var bullet = PoolManager.instance.GetEnemyArrowBullet();
        bullet.transform.position = transform.position;
        bullet.Shoot(transform.TransformVector(direction));
    }

    private void OnDrawGizmos()
    {

        for (int i = 0; i < fireLocalDirections.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + fireLocalDirections[i]);
        }
    }
}
