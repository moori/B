using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Color[] hpColors;
    private HealthComponent healthComponent;
    private Player player;

    public static System.Action<Enemy> OnEnemyDeath;

    [Header("Movement")]
    public bool facePlayer;
    public float turningSpeed;

    [Header("Drop")]
    public int coinAmount;


    private void Awake()
    {
        if(TryGetComponent<HealthComponent>(out var h))
        {
            healthComponent = h;
            healthComponent.OnDie.AddListener(Die);
        }

        player = FindObjectOfType<Player>();
    }

    private void OnDestroy()
    {
        healthComponent?.OnDie.RemoveListener(Die);
    }

    private void Update()
    {
        if (facePlayer)
        {
            //var playerDirection = Quaternion.LookRotation(Vector3.forward, player.transform.position-transform.position);
            transform.up = Vector3.Lerp(transform.up, player.transform.position - transform.position, turningSpeed * Time.deltaTime);
        }
    }

    public void OnTakeDamage(int damage)
    {

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

        Debug.Log($"Kill enemny {gameObject.name}. Spawn {coinAmount} coins.");
        //coin
        for (int i = 0; i < coinAmount; i++)
        {
            var coin = PoolManager.instance.GetCoin();
            coin.Spawn(transform.position);
        }

    }
}
