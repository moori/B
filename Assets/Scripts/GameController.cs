using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Enemies")]
    public List<Enemy> enemiesAlive = new List<Enemy>();
    public int maxEnemies = 3;
    public float spawnDelay = 3f;
    public Enemy enemyPrefab;
    private Player player;
    private System.Random rand;

    [Header("Battery")]
    public List<Transform> batterSpawnPoints;

    [Header("GameOver")]
    public GameObject gameoverScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Enemy.OnEnemyDeath += OnEnemyDeath;
        player = FindObjectOfType<Player>();

        rand = new System.Random();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < maxEnemies; i++)
        {
            Spawn();
            yield return new WaitForSeconds(0.66f);
        }

        StartCoroutine(BatterySpawnerRoutine());
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
        DOVirtual.DelayedCall(6f, () => { 
            Spawn();
        });

    }

    public void GameOver()
    {
        StopAllCoroutines();
        foreach (var enemy in enemiesAlive)
        {
            foreach (var weapon in enemy.GetComponentsInChildren<BulletSpawner>())
            {
                weapon.gameObject.SetActive(false);
            }
        }

        DOVirtual.DelayedCall(1f, () =>
        {
            gameoverScreen.SetActive(true);
        });
        DOVirtual.DelayedCall(5f, () => {
            SceneManager.LoadScene(0);
        });
    }

    public IEnumerator BatterySpawnerRoutine()
    {
        while (true)
        {
            var ammoPercent = player.ammo / (float)player.maxAmmo;
            if(ammoPercent < .2f)
            {
                if(Random.value <= .5f)
                {
                    SpawnBattery();
                    yield return new WaitForSeconds(10f);
                }
            }
            else if (ammoPercent < .5f)
            {
                if (Random.value <= .15f)
                {
                    SpawnBattery();
                    yield return new WaitForSeconds(10f);
                }
            }
            yield return new WaitForSeconds(10f);
        }
    }

    public void SpawnBattery()
    {
        var point = batterSpawnPoints[rand.Next(batterSpawnPoints.Count)];

        var battery = PoolManager.instance.GetBattery();
        battery.transform.position = point.position;
        battery.SpawnBattery(point.up);
    }

}
