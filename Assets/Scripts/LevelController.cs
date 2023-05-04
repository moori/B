using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class LevelController : MonoBehaviour
{
    public LevelData levelData;
    public int currentLevel = 0;
    public List<Enemy> enemiesAlive = new List<Enemy>();
    public List<Enemy> currentWaveEnemies = new List<Enemy>();
    public Enemy currentBoss;

    private int[] wavesPerLevelProgression = new int[] { 2, 2, 3, 3, 3, 4, 4, 4, 4, 5 };

    [Header("DBs")]
    public List<SpawnGroup> waveDB;
    public List<WaveData> randosDB;
    public List<BossData> bossDB;

    private System.Random rand;
    private float timeToExpireWave;
    private Coroutine waveTimer;
    private bool waveExpired;

    [Header("UI")]
    public FillImageHelper bossHPBar;

    private void Awake()
    {
        rand = new System.Random();
        Enemy.OnEnemyDeath += OnEnemyDeath;

        PrespawnBosses();
    }

    private void OnDestroy()
    {

        Enemy.OnEnemyDeath -= OnEnemyDeath;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log($"killing {enemiesAlive.Count} enemies");
            for (int i = enemiesAlive.Count - 1; i >= 0; i--)
            {

                enemiesAlive[i].Die();
            }
        }
#endif
    }

    public void PrespawnBosses()
    {
        foreach (var b in bossDB)
        {
            b.boss = Instantiate<Enemy>(b.boss);
            b.boss.transform.SetParent(transform);
            b.boss.gameObject.SetActive(false);
        }
    }

    public void StartLevel()
    {
        BackColorController.instance.SetIdle();
        if (levelData == null)
        {
            levelData = CreateLevel(currentLevel);
        }
        else
        {
            currentLevel++;
            levelData = CreateLevel(currentLevel);
        }

        enemiesAlive.Clear();
        currentBoss = null;
        currentWaveEnemies.Clear();

        StartCoroutine(SpawnGroupRoutine(levelData.GetNextWave()));
    }

    private void EvaluateWave()
    {
        if (currentWaveEnemies.Count == 0 )
        {
            //end wave
            StopCoroutine(waveTimer);
            levelData.currentWave++;
            var wave = levelData.GetNextWave();
            if (wave)
            {
                StartCoroutine(SpawnGroupRoutine(wave));
                Debug.Log("Start Wave");
            }
            else
            {
                //boss
                Debug.Log("boss");
                StartCoroutine(SpawnBossRoutine());
            }
        }
        else
        {
            //check spawnRando
        }
    }

    private IEnumerator SpawnBossRoutine()
    {
        BackColorController.instance.SetBoss();
        bossHPBar.Show();
        yield return new WaitForSeconds(0.5f);
        bossHPBar.SetFill(1f, 1f);
        yield return new WaitForSeconds(3f);

        //var boss = Instantiate<Enemy>(levelData.boss.boss, levelData.boss.point, Quaternion.identity);
        var boss = levelData.boss.boss;
        boss.OnHPChangePercent += bossHPBar.SetFill;
        boss.Respawn(levelData.boss.point);
        currentBoss = boss;
    }

    private IEnumerator WaveDurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ExpireWave();
    }

    private void ExpireWave()
    {
        levelData.currentWave++;
        var wave = levelData.GetNextWave();
        if (wave)
        {
            StartCoroutine(SpawnGroupRoutine(wave));
            Debug.Log("Start Wave");
        }
    }

    private IEnumerator SpawnGroupRoutine(SpawnGroup group)
    {
        yield return new WaitForSeconds(2f);
        waveTimer = StartCoroutine(WaveDurationRoutine(group.waveDuration));
        foreach (var spawnData in group.spawns)
        {
            var pos = spawnData.GetPositionInRegion();
            Enemy enemy = null;
            switch (spawnData.spawnable)
            {
                case Spawnable.StaticMinion:
                    enemy = PoolManager.instance.GetSimpleEnemy();
                    break;
                case Spawnable.Stalker:
                    enemy = PoolManager.instance.GetStalkerEnemy();
                    break;
                default:
                    break;
            }

            if (enemy)
            {
                enemy.Respawn(pos);
                currentWaveEnemies.Add(enemy);
                enemiesAlive.Add(enemy);
            }
            yield return new WaitForSeconds(group.delayBetweenSpawns);
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        if (enemiesAlive.Contains(enemy))
        {
            enemiesAlive.Remove(enemy);
        }
        if (currentWaveEnemies.Contains(enemy))
        {
            currentWaveEnemies.Remove(enemy);
        }

        if(enemy == currentBoss)
        {
            StartCoroutine( EndLevel());
            return;
        }

        EvaluateWave();
    }

    public IEnumerator EndLevel()
    {
        BackColorController.instance.SetIdle();
        yield return new WaitForSeconds(2f);
        bossHPBar.Hide();
        yield return new WaitForSeconds(2f);
        StartLevel();
    }

    public LevelData CreateLevel(int levelIndex)
    {
        var level = new LevelData();
        level.mainSequence = new List<SpawnGroup>();


        for (int i = 0; i < wavesPerLevelProgression[levelIndex]; i++)
        {
            var w = waveDB.Where(x => x.minLevel <= levelIndex).ToList().GetRandom();
            level.mainSequence.Add(w);
        }
        level.boss= bossDB.Where(x => x.minlevel <= levelIndex).ToList().GetRandom();

        return level;
    }
}

public class LevelData
{
    public List<SpawnGroup> mainSequence;
    public List<SpawnData> randomSpawns;
    public BossData boss;

    public int currentWave;

    public int MainEnemiesInLevel()
    {
        return mainSequence.Sum(x => x.spawns.Count);
    }

    public SpawnGroup GetNextWave()
    {
        if(mainSequence.Count > currentWave)
        {
            return mainSequence[currentWave];
        }
        else
        {
            return null;
        }
    }
}

[System.Serializable]
public class BossData
{
    public Enemy boss;
    public Vector3 point;
    public int minlevel;
    public int maxlevel;
}