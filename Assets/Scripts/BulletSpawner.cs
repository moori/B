using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public List<Vector3> fireLocalDirections;

    public float initialDelay=0;
    public float delayBetweenShots=0.035f;
    public int volleysPerAttack = 1;
    public float delayBetweenAttacks = 2f;
    public float delayBetweenVolleys = 0f;
    public float angleError = 0f;

    [Header("Bullet")]
    public BulletType bulletType;

    [Header("Generator")]
    public int directionsAmount;


    [Header("Audio")]
    public AudioSource source;
    public AudioClip attackClip;
    public AudioClip volleyClip;
    public AudioClip shotClip;

    enum AttackType
    {
        FireAll,
        FireSequence
    }


    public enum BulletType
    {
        Arrow,
        Battery,
        Missile
    }
    private void OnEnable()
    {
        StartCoroutine(FireAllRoutine());
        GameController.OnGameOver += Stop;
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Stop()
    {
        StopAllCoroutines();
        GameController.OnGameOver -= Stop;
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        if (source == null)
        {
            AudioManager.GetAudioSource().PlayOneShot(clip);
            return;
        }
        if (source.isPlaying) return;

        source.clip = clip;
        source.Play();
    }

    public IEnumerator FireAllRoutine()
    {
        yield return new WaitForSeconds(3f+ initialDelay);
        while (true)
        {

            PlayClip(attackClip);
            for (int j = 0; j < volleysPerAttack; j++)
            {
                PlayClip(volleyClip);

                for (int i = 0; i < fireLocalDirections.Count; i++)
                {
                    Shoot(fireLocalDirections[i] + Player.AddNoiseOnAngle(-angleError,angleError));
                    PlayClip(shotClip);
                    if (delayBetweenShots>0)
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
        switch (bulletType)
        {
            default:
            case BulletType.Arrow:
                bullet = PoolManager.instance.GetEnemyArrowBullet();
                break;
            case BulletType.Battery:
                bullet = PoolManager.instance.GetEnemyBatteryBullet();
                break;
            case BulletType.Missile:
                bullet = PoolManager.instance.GetEnemyMissileBullet();
                break;
        }

        if (bullet == null) return;
        bullet.transform.position = transform.position;
        bullet.Shoot(transform.TransformVector(direction));
    }

    private void OnDrawGizmos()
    {
        if (fireLocalDirections == null) return;
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
