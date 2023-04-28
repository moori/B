using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;

public class Player : MonoBehaviour
{
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

    public UnityEvent<int> OnShoot;

    [Header("Bubble")]
    public float bubbleRadius;
    public float bubbleDuration;
    public bool isBubbling;
    public float bubbleStartTime;
    public SpriteRenderer bubbleSprite;

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
    }

    void Update()
    {
        HandleMovement();
        HandleTurret();
        HandleBubble();
        HandleShot();
    }

    private void HandleMovement()
    {
        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        currentSpeed = Input.GetKey(KeyCode.L) ? slowSpeed : speed;
        var moveVec = inputDir * currentSpeed * Time.deltaTime;
        var pos = transform.position + new Vector3(moveVec.x, moveVec.y, 0);

        pos = new Vector3(Mathf.Clamp(pos.x, -horizontalBoundarie, horizontalBoundarie), Mathf.Clamp(pos.y, -verticalBoundarie, verticalBoundarie), 0);

        transform.position = pos;
    }

    private void HandleTurret() {
        if (isShooting) return;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
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
            if (Input.GetKey(KeyCode.K))
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
        if (Input.GetKey(KeyCode.J))
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
        ammo -= 1;
    }

    Vector3 AddNoiseOnAngle(float min, float max)
    {
        float xNoise = UnityEngine.Random.Range(min, max);

        Vector3 noise = new Vector3(Mathf.Sin(2 * Mathf.PI * xNoise / 360), 0, 0);
        return noise;
    }
}
