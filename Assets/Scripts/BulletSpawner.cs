using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public List<Vector3> fireLocalDirections;

    public float delayBetweenShots=0.035f;
    public int volleysPerAttack = 1;
    public float delayBetweenAttacks = 2f;
    public float delayBetweenVolleys = 0f;
    public float angleError = 0f;

    [Header("Battery")]
    public bool shootsBattery;

    [Header("Generator")]
    public int directionsAmount;

    enum AttackType
    {
        FireAll,
        FireSequence
    }

    private void OnEnable()
    {
        StartCoroutine(FireAllRoutine());
        GameController.OnGameOver += Stop;
    }

    public void Stop()
    {
        StopAllCoroutines();
        GameController.OnGameOver -= Stop;
    }
    public IEnumerator FireAllRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            for (int j = 0; j < volleysPerAttack; j++)
            {

                for (int i = 0; i < fireLocalDirections.Count; i++)
                {
                    Shoot(fireLocalDirections[i] + Player.AddNoiseOnAngle(-angleError,angleError));
                    if(delayBetweenShots>0)
                        yield return new WaitForSeconds(delayBetweenShots);
                }
                if (delayBetweenVolleys > 0)
                    yield return new WaitForSeconds(delayBetweenVolleys);
            }
            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }

    public void Shoot(Vector3 direction)
    {
        Bullet bullet = null;
        if (shootsBattery)
        {
            bullet = PoolManager.instance.GetEnemyBatteryBullet();
        }
        else
        {
            bullet = PoolManager.instance.GetEnemyArrowBullet();
        }
        if (bullet == null) return;
        bullet.transform.position = transform.position;
        bullet.Shoot(transform.TransformVector(direction));
    }

    private void OnDrawGizmos()
    {

        for (int i = 0; i < fireLocalDirections.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.TransformVector(fireLocalDirections[i]*3));
        }
    }

    [ContextMenu("GenerateDirection")]
    public void GenerateDirection()
    {
        fireLocalDirections.Clear();
        for (int i = 0; i < directionsAmount; i++)
        {
            fireLocalDirections.Add(new Vector3(Mathf.Cos((i * (360f / directionsAmount)) * Mathf.Deg2Rad), Mathf.Sin((i * (360f / directionsAmount)) * Mathf.Deg2Rad),0));
        }    
    }
    [ContextMenu("InvertDirection")]

    public void InvertDirection()
    {
        fireLocalDirections.Reverse();
    }
}
