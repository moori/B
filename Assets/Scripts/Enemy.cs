using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public bool isActive;
    public Color[] hpColors;
    [HideInInspector]
    public HealthComponent healthComponent;
    private Player player;

    public static System.Action<Enemy> OnEnemyDeath;
    public System.Action<float> OnHPChangePercent;
    public System.Action OnDie;
    public UnityEvent OnRespawn;

    [Header("Movement")]
    public bool facePlayer;
    public float turningSpeed;
    public float speed;
    public MovePattern movePattern;
    public List<Vector3> moveTargetsRelativeToPlayer = new List<Vector3>();

    [Header("Drop")]
    public int coinAmount;
    public int baseEnergyShardAmount;
    public List<BatteryTimer> batteryTimers = new List<BatteryTimer>();
    public float batteryLifetime;

    [Header("Pulse")]
    public Color pulseStartColor;
    public Color pulseEndColor;
    public float pulseDuration;
    public float pulseFromScale;
    public float pulseToScale;

    private System.Random rand;

    [Header("Audio")]
    public AudioClip spawnClip;
    public AudioClip spawnPulseClip;
    public AudioClip deathClip;

    public enum MovePattern
    {
        Static,
        Stalker
    }

    private void Awake()
    {
        if(TryGetComponent<HealthComponent>(out var h))
        {
            healthComponent = h;
            healthComponent.OnDie.AddListener(Die);
        }

        player = FindObjectOfType<Player>();
        rand = new System.Random();
    }

    private void OnDestroy()
    {
        healthComponent?.OnDie.RemoveListener(Die);
    }

    private void Update()
    {
        if (facePlayer)
        {
            transform.up = Vector3.Lerp(transform.up, (player.transform.position - transform.position).normalized, turningSpeed * Time.deltaTime);
        }

    }

    public void SetMovementPattern()
    {

        switch (movePattern)
        {
            case MovePattern.Stalker:
                StartCoroutine(StalketMovementPatternRoutine());
                break;
            case MovePattern.Static:
            default:
                break;
        }
    }


    private IEnumerator StalketMovementPatternRoutine()
    {
        var point = moveTargetsRelativeToPlayer[rand.Next(moveTargetsRelativeToPlayer.Count)];
        while (!GameController.IsGameOver)
        {
            var target = GameController.GetClosestPositionInsideBounds(player.transform.position + point);
            if (Vector3.Distance(target, transform.position) < 1f)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            var dir = (target - transform.position).normalized;
            transform.position += dir * speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void Enable()
    {
        transform.localScale = Vector3.one;

        foreach (var batteryTimer in batteryTimers)
        {
            batteryTimer.Setup();
        }
        StepBatteryCounter();
        SetMovementPattern();
    }

    public void Respawn(Vector3 pos)
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        var  p = PoolManager.instance.GetPulse();
        transform.position = pos;
        p.transform.position = pos;
        AudioManager.GetAudioSource().PlayOneShot(spawnPulseClip, 0.6f);
        p.StartPulse(pulseStartColor,pulseEndColor, pulseDuration, pulseFromScale, pulseToScale, ()=> {
            isActive = true;
            Enable();
            AudioManager.GetAudioSource().PlayOneShot(spawnClip,0.75f);
            OnRespawn?.Invoke();
        });
    }

    public void StepBatteryCounter()
    {
        var counter = batteryTimers.FirstOrDefault(x => !x.isExpired);
        if (counter)
        {
            counter.StartTimer(batteryLifetime);
        }
    }

    public void OnTakeDamage(int damage)
    {
        OnHPChangePercent?.Invoke(healthComponent.HP / (float)healthComponent.maxHP);
    }

    public void Die()
    {
        //kaboom
        CameraController.instance.Shake(0.1f, 0.05f);
        var part = PoolManager.instance.GetExplosionParticles();
        part.transform.position = transform.position;
        part.gameObject.SetActive(true);
        gameObject.SetActive(false);
        OnEnemyDeath?.Invoke(this);

        //coin
        for (int i = 0; i < coinAmount; i++)
        {
            var coin = PoolManager.instance.GetCoin();
            coin.Spawn(transform.position);
        }

        for (int i = 0; i < baseEnergyShardAmount; i++)
        {
            var shard = PoolManager.instance.GetBatteryShard();
            shard.Spawn(transform.position);
        }

        if (batteryTimers.Count > 0)
        {
            var batShards = batteryTimers.Count(x => !x.isExpired);
            for (int i = 0; i < batShards; i++)
            {
                var shard = PoolManager.instance.GetBatteryShard();
                shard.Spawn(transform.position);
            }
        }

        StopAllCoroutines();
        AudioManager.GetAudioSource().PlayOneShot(deathClip);
    }

    private void OnDrawGizmosSelected()
    {
        if (moveTargetsRelativeToPlayer.Count > 0)
        {
            Gizmos.color = Color.blue;
            var reference = player ? player.transform.position : Vector3.zero; 
            for (int i = 0; i < moveTargetsRelativeToPlayer.Count; i++)
            {
                var p = moveTargetsRelativeToPlayer[i];
                Gizmos.DrawSphere(reference + p, 0.2f);
            }
        }
    }
}
