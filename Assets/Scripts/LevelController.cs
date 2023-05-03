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

    private int[] wavesPerLevelProgression = new int[] { 2, 2, 3, 3, 3, 4, 4, 4, 4, 5 };

    [Header("DBs")]
    public List<SpawnGroup> waveDB;
    public List<WaveData> randosDB;
    public List<BossData> bossDB;

    private System.Random rand;
    private float timeToExpireWave;
    private Coroutine waveTimer;
    private bool waveExpired;

    private void Awake()
    {
        rand = new System.Random();
        Enemy.OnEnemyDeath += OnEnemyDeath;
    }

    private void OnDestroy()
    {

        Enemy.OnEnemyDeath -= OnEnemyDeath;
    }

    public void StartLevel()
    {
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
            }
        }
        else
        {
            //check spawnRando
        }
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

        EvaluateWave();
    }

    public LevelData CreateLevel(int levelIndex)
    {
        var level = new LevelData();
        level.mainSequence = new List<SpawnGroup>();


        for (int i = 0; i < wavesPerLevelProgression[levelIndex]; i++)
        {
            var w = waveDB.Where(x => x.minLevel >= levelIndex).ToList().GetRandom();
            level.mainSequence.Add(w);
        }

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

public class BossData
{
    public Enemy boss;
}