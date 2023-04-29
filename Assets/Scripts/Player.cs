using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;
using System;

public class Player : MonoBehaviour
{
    private HealthComponent healthComponent;

    [Header("Movement")]
    public float speed=9f;
    public float slowSpeed=3f;
    public float currentSpeed;
    private Vector3 inputDir;
    public float horizontalBoundarie;
    public float verticalBoundarie;

    [Header("Weapon")]
    //public BulletPool bulletPool;
    public float delayBetweenShots;
    private float timeLastShot;
    public float minAngleError = 15;
    public Transform turret;
    public List<Vector3> lastMoveDirections = new List<Vector3>();
    public int lastMoveDirectionsAmount = 10;
    private int moveDirIndex = 0;
    private bool isShooting;
    public int maxAmmo = 20000;
    public int ammo;
    Collider2D[] hitsBuffer = new Collider2D[10];

    public UnityEvent<int> OnShoot;
    public UnityEvent<float> OnAmmoChange;
    public UnityEvent<float> OnRecharge;
    public UnityEvent OnDie;

    [Header("Bubble")]
    public float bubbleRadius;
    public float bubbleDuration;
    public bool isBubbling;
    public float bubbleStartTime;
    public SpriteRenderer bubbleSprite;
    public LayerMask collectibleLayerMask;

    [Header("Coins")]
    public float coinAttractionRadius;
    public float coinCollectionRadius;
    public float coinAttractionSpeed;
    public LayerMask coinCollectibleLayerMask;
    Collider2D[] coinHitsBuffer = new Collider2D[20];
    public int coins = 0;
    public UnityEvent<int> OnCollectCoin;

    private void Awake()
    {
        for (int i = 0; i < lastMoveDirectionsAmount; i++)
        {
            lastMoveDirections.Add(Vector3.down);
        }

        ammo = maxAmmo; 
        bubbleSprite.transform.localScale = Vector3.zero;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        healthComponent = GetComponent<HealthComponent>();
    }

    void Update()
    {
        HandleMovement();
        HandleTurret();
        HandleBubble();
        HandleShot();
    }

    private void FixedUpdate()
    {
        if (isBubbling)
        {
            HandleFixedBubble();
        }

        HandleFixedCoins();
    }

    private void HandleFixedBubble()
    {
        int numHits = Physics2D.OverlapCircleNonAlloc(transform.position, bubbleRadius, hitsBuffer, collectibleLayerMask.value);
        for (int i = 0; i < numHits; i++)
        {
            if (hitsBuffer[i].gameObject.TryGetComponent<Battery>(out var battery))
            {
                Recharge(battery.percentCharge);
                battery.Collect();
            }

            if (hitsBuffer[i].gameObject.TryGetComponent<Bullet>(out var enemyBullet))
            {
                enemyBullet.Kill();
            }
        }
    }
    private void HandleFixedCoins()
    {
        int numHits = Physics2D.OverlapCircleNonAlloc(transform.position, coinAttractionRadius, coinHitsBuffer, coinCollectibleLayerMask.value);
        for (int i = 0; i < numHits; i++)
        {
            if (coinHitsBuffer[i].gameObject.TryGetComponent<Coin>(out var coin))
            {
                var distance = Vector3.Distance(transform.position, coin.transform.position);
                if (distance <= coinCollectionRadius)
                {
                    coin.Kill();
                    CollectCoin();
                    continue;
                }

                var attractionDir = (transform.position - coin.transform.position).normalized;
                coin.transform.position += attractionDir * coinAttractionSpeed * Mathf.Lerp(3,1, distance/coinAttractionRadius ) * Time.fixedDeltaTime;
            }
        }
    }

    private void CollectCoin()
    {
        coins++;
        OnCollectCoin?.Invoke(coins);
    }

    public void Recharge(float percent)
    {
        ammo = Mathf.Clamp(ammo + Mathf.RoundToInt(maxAmmo * percent), 0, maxAmmo);
        OnRecharge?.Invoke(percent);
        OnAmmoChange?.Invoke(ammo / (float)maxAmmo);
    }

    private void HandleMovement()
    {
        inputDir = new Vector2(PlayerInputs.Horizontal, PlayerInputs.Vertical).normalized;
        currentSpeed = PlayerInputs.Slow ? slowSpeed : speed;
        var moveVec = inputDir * currentSpeed * Time.deltaTime;
        var pos = transform.position + new Vector3(moveVec.x, moveVec.y, 0);

        pos = new Vector3(Mathf.Clamp(pos.x, -horizontalBoundarie, horizontalBoundarie), Mathf.Clamp(pos.y, -verticalBoundarie, verticalBoundarie), 0);

        transform.position = pos;
    }

    private void HandleTurret() {
        if (isShooting) return;
        if (PlayerInputs.Horizontal != 0 || PlayerInputs.Vertical != 0)
        {
            moveDirIndex = (moveDirIndex + 1) % lastMoveDirectionsAmount;
            lastMoveDirections[moveDirIndex] = inputDir;
            var turretForward = -new Vector3(lastMoveDirections.Average(x => x.x), lastMoveDirections.Average(x => x.y), lastMoveDirections.Average(x => x.y));
            turret.rotation = Quaternion.LookRotation(Vector3.forward, turretForward);
        }
    }

    private void HandleBubble()
    {
        if (isBubbling)
        {
            if (Time.time - bubbleStartTime >= bubbleDuration)
            {
                isBubbling = false;
                bubbleSprite.transform.DOScale(0f, 0.075f);
            }
        }
        else
        {
            if (PlayerInputs.Bubble)
            {
                bubbleStartTime = Time.time;
                bubbleSprite.transform.localScale = Vector3.zero;
                bubbleSprite.transform.DOScale(0.5f, 0.075f);
                isBubbling = true;
            }
        }
    }

    private void HandleShot()
    {
        if (PlayerInputs.Fire)
        {
            if(Time.time - timeLastShot >= delayBetweenShots)
            {
                Shoot();
                isShooting = true;
            }
        }
        else
        {
            isShooting = false;
        }
    }

    private void Shoot()
    {
        if (ammo <= 0) return;
        var b = PoolManager.instance.GetPlayerBullet();
        if (!b) return;
        timeLastShot = Time.time;
        b.transform.position = turret.transform.TransformPoint(Vector3.up * 0.25f);
        var shotDir = turret.transform.up + turret.transform.TransformVector(AddNoiseOnAngle(-minAngleError,minAngleError));
        b.Shoot(shotDir);
        OnShoot?.Invoke(1);
        OnAmmoChange?.Invoke(ammo/(float)maxAmmo);
        ammo -= 1;
    }

    Vector3 AddNoiseOnAngle(float min, float max)
    {
        float xNoise = UnityEngine.Random.Range(min, max);

        Vector3 noise = new Vector3(Mathf.Sin(2 * Mathf.PI * xNoise / 360), 0, 0);
        return noise;
    }

    public void TakeHit(int damage)
    {
        CameraController.instance.Shake(0.2f, 0.6f);
        Recharge(-.2f);
        if (ammo <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Game Over");

        OnDie?.Invoke();
        gameObject.SetActive(false);

        CameraController.instance.Shake(0.4f, 0.6f);
        var part = PoolManager.instance.GetExplosionParticles();
        part.transform.position = transform.position;
        part.gameObject.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, bubbleRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, coinAttractionRadius);
        Gizmos.DrawWireSphere(transform.position, coinCollectionRadius);
    }

    public float GetHP_Percent()
    {
        return healthComponent.HP / healthComponent.maxHP;
    }
}
