using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float speed = 14f;
    public bool isDead;
    public float lifetime = 2f;
    private float startTime;
    public LayerMask targetLayerMask;
    public Vector3 collisionOffset;
    public float radius;
    Collider2D[] hitsBuffer = new Collider2D[1];
    Collider2D[] detectionHitsBuffer = new Collider2D[8];

    public static System.Action OnBulletHit;
    public UnityEvent OnBulletHitEvent;
    public UnityEvent OnBulletSpawn;

    [Header("Tracking")]
    public bool isTrackingBullet;
    public float turningSpeed;
    public float detectionRadius;
    public Transform target;

    [Header("Audio")]
    public AudioClip hitClip;
    public AudioClip spawnClip;

    public void Shoot(Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.up = direction;
        isDead = false; 
        startTime = Time.time;
        OnBulletSpawn?.Invoke();

        target = null;

        if(spawnClip)
            AudioManager.GetAudioSource().PlayOneShot(spawnClip);
    }

    public void Kill()
    {
        isDead = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        if (target != null)
        {
            var adjustSpeed = Mathf.Lerp(turningSpeed*2f, turningSpeed, Vector2.Distance(transform.position, target.position) / 2f);
            transform.up = Vector3.Lerp(transform.up, (target.position - transform.position).normalized, adjustSpeed * Time.deltaTime);
        }
        transform.position += transform.up * speed * Time.deltaTime;
        
        if(lifetime>0)
            if (Time.time - startTime >= lifetime) Kill();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        HandleTracking();

        HandleHit();

    }

    private void HandleTracking()
    {
        int numDetectionHits = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRadius, detectionHitsBuffer, targetLayerMask.value);

       

        if (target == null)
        {
            float minDistance = 100f;
            for (int i = 0; i < numDetectionHits; i++)
            {
                var obj = detectionHitsBuffer[i].transform;

                var distance = Vector3.Distance(transform.position, obj.position);
                if (distance < minDistance)
                {
                    target = obj;
                }
            }
        }
        else if (!target.gameObject.activeInHierarchy) target = null;
    }

    private void HandleHit()
    {
        int numHits = Physics2D.OverlapCircleNonAlloc(transform.TransformPoint(collisionOffset), radius, hitsBuffer, targetLayerMask.value);

        for (int i = 0; i < numHits; i++)
        {
            var hitTarget = hitsBuffer[i].gameObject.GetComponent<HitTarget>();
            hitTarget.Hit(damage);
            var p = PoolManager.instance.GetHitParticle();
            p.transform.position = hitsBuffer[i].ClosestPoint(transform.position);
            p.transform.forward = -transform.up;
            p.gameObject.SetActive(true);
            p.Play();

            gameObject.SetActive(false);
            OnBulletHit?.Invoke();
            OnBulletHitEvent?.Invoke();
            AudioManager.GetAudioSource().PlayOneShot(hitClip, 0.8f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + collisionOffset, radius);
    }
}
