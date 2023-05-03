using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float speed = 14f;
    public bool isDead;
    private float lifetime = 2f;
    private float startTime;
    public LayerMask targetLayerMask;
    public Vector3 collisionOffset;
    public float radius;
    Collider2D[] hitsBuffer = new Collider2D[1];

    public static System.Action OnBulletHit;


    [Header("Audio")]
    public AudioClip hitClip;

    public void Shoot(Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.up = direction;
        isDead = false; 
        startTime = Time.time;
    }

    public void Kill()
    {
        isDead = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        transform.position += transform.up * speed * Time.deltaTime;
        //if (Time.time - startTime >= lifetime) Kill();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
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
            AudioManager.GetAudioSource().PlayOneShot(hitClip, 0.8f);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + collisionOffset, radius);
    }
}
