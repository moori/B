using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public List<Enemy> enemiesAlive = new List<Enemy>();
    public int maxEnemies = 3;
    public float spawnDelay = 3f;
    public Enemy enemyPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Enemy.OnEnemyDeath += OnEnemyDeath;

    }

    private void Start()
    {

        for (int i = 0; i < maxEnemies; i++)
        {
            Spawn();
        }
    }

    private void OnDestroy()
    {
        instance = null;
        Enemy.OnEnemyDeath -= OnEnemyDeath;
    }

    public void Spawn()
    {
        if (enemiesAlive.Count > maxEnemies) return;

        var pos = UnityEngine.Random.insideUnitSphere * 8f;
        pos = new Vector3(Mathf.Clamp(pos.x, -8f, 8f), Mathf.Clamp(pos.y, -4f, 4f), 0);

        var pulse = PoolManager.instance.GetPulse();
        pulse.transform.position = pos;
        pulse.StartPulse();
        DOVirtual.DelayedCall(1f, () => {
            var enemy = PoolManager.instance.GetSimpleEnemy();
            enemy.transform.position = pos;
      
            enemy.gameObject.SetActive(true);
            enemiesAlive.Add(enemy);
        });
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemiesAlive.Remove(enemy);
        DOVirtual.DelayedCall(3f, () => { 
            Spawn();
        });

    }

}
