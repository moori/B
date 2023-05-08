using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using DG.Tweening;

public class LevelController : MonoBehaviour
{
    public LevelData levelData;
    public static int currentLevel;
    public List<Enemy> enemiesAlive = new List<Enemy>();
    public List<Enemy> currentWaveEnemies = new List<Enemy>();
    public List<Enemy> currentRandosEnemies = new List<Enemy>();
    public Enemy currentBoss;

    private int[] wavesPerLevelProgression = new int[]  { 2, 2, 3, 3, 3, 3 };
    private int[] randosPerLevelProgression = new int[] { 0, 0, 3, 5, 7, 8, 9, 12, 15, 20};
    private int[] randosinBossProgression = new int[]   { 0, 0, 2, 3, 3, 4, 4, 5, 5, 5};

    [Header("DBs")]
    public List<SpawnGroup> waveDB;
    public List<SpawnData> randosDB;
    public List<BossData> bossDB;

    private System.Random rand;
    private float timeToExpireWave;
    private Coroutine waveTimer;
    private bool waveExpired;

    [Header("UI")]
    public FillImageHelper bossHPBar;
    public TextMeshPro levelText;

    [Header("UpgradePhase")]
    public UpgradePhaseController upgradePhaseController;

    private void Awake()
    {
        currentLevel = 0;

        rand = new System.Random();
        Enemy.OnEnemyDeath += OnEnemyDeath;

        levelText.DOFade(0f, 0f);
        PrespawnBosses();

        GameController.OnGameOver += Stop;

    }

    private void Stop()
    {
        GameController.OnGameOver -= Stop;
    }

    private void OnDestroy()
    {
        Stop();
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach (var item in FindObjectsOfType<Enemy>())
            {
                item.GetComponent<HealthComponent>().HP = 1;
            }
        }
#endif
    }

    public void PrespawnBosses()
    {
        foreach (var b in bossDB)
        {
            var prefab = b.boss;
            b.boss = Instantiate<Enemy>(b.boss);
            b.boss.transform.SetParent(transform);
            b.boss.gameObject.SetActive(false);
            b.boss.gameObject.name = prefab.gameObject.name;
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

        levelText.text = $"level {string.Format("{0:00}", currentLevel+1)}";
        levelText.DOFade(0f, 0f);
        levelText.DOFade(1f, 0.2f).OnComplete(() => {
            levelText.DOFade(0f, 0.2f).SetDelay(3f);
        });
    }

    private void EvaluateWave()
    {
        if (GameController.IsGameOver || TutorialController.IsTutorial || (currentBoss != null && currentBoss.isActive)) return;
        if (currentWaveEnemies.Count == 0 )
        {
            //end wave
            StopCoroutine(waveTimer);
            levelData.currentWave++;
            var wave = levelData.GetNextWave();
            if (wave)
            {
                StartCoroutine(SpawnGroupRoutine(wave));
            }
            else
            {
                //boss
                if (enemiesAlive.Count == 0)
                {
                    StartCoroutine(SpawnBossRoutine());
                }
            }
        }
    }

    private IEnumerator SpawnBossRoutine()
    {
        var boss = levelData.boss.boss;
        boss.healthComponent.IncreaseMaxHPOVerOriginal(30 * currentLevel);

        BackColorController.instance.SetBoss();
        bossHPBar.Show();
        yield return new WaitForSeconds(0.5f);
        bossHPBar.SetFill(1f, 1f);
        yield return new WaitForSeconds(3f);

        boss.OnHPChangePercent += bossHPBar.SetFill;
        boss.Respawn(levelData.boss.point);
        currentBoss = boss;


        yield return new WaitForSeconds(5f);
        for (int i = 0; i < randosinBossProgression[currentLevel]; i++)
        {
            SpawnEnemy(randosDB.GetRandom());
            yield return new WaitForSeconds(.33f);
        }

        //AudioManager.PlayBossMusic();
    }

    private IEnumerator WaveDurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ExpireWave();
    }

    private void ExpireWave()
    {
        if (GameController.IsGameOver) return;
        levelData.currentWave++;
        var wave = levelData.GetNextWave();
        if (wave)
        {
            StartCoroutine(SpawnGroupRoutine(wave));
        }
    }

    private IEnumerator SpawnGroupRoutine(SpawnGroup group)
    {
        yield return new WaitForSeconds(2f);
        waveTimer = StartCoroutine(WaveDurationRoutine(group.waveDuration));
        foreach (var spawnData in group.spawns)
        {
            SpawnEnemy(spawnData);
            yield return new WaitForSeconds(group.delayBetweenSpawns);
        }
    }

    private void SpawnEnemy(SpawnData spawnData)
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
            case Spawnable.LasetTurret:
                enemy = PoolManager.instance.GetLaserTurretEnemy();
                break;
            case Spawnable.MissileTurret:
                enemy = PoolManager.instance.GetMissileTurretEnemy();
                break;
        }

        if (enemy)
        {
            enemy.Respawn(pos);
            currentWaveEnemies.Add(enemy);
            enemiesAlive.Add(enemy);
            enemy.healthComponent.IncreaseMaxHPOVerOriginal(LevelController.currentLevel * 1);
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
        yield return new WaitForSeconds(1f);
        upgradePhaseController.StartUpgradePhase();
        for (int i = enemiesAlive.Count - 1; i >= 0; i--)
        {
            enemiesAlive[i].Die();
        }
        //AudioManager.PlayAmbience();
    }

    public LevelData CreateLevel(int levelIndex)
    {
        var level = new LevelData();
        level.mainSequence = new List<SpawnGroup>();
        int waveProg = levelIndex <= wavesPerLevelProgression.Length - 1 ? wavesPerLevelProgression[levelIndex] : wavesPerLevelProgression.Last();
        int randosProg = levelIndex <= randosPerLevelProgression.Length - 1 ? randosPerLevelProgression[levelIndex] : randosPerLevelProgression.Last();

        string lastPicked = "";
        for (int i = 0; i < waveProg; i++)
        {
            SpawnGroup w = Instantiate(waveDB.Where(x => levelIndex >= x.minLevel && levelIndex <= x.maxLevel).ToList().GetRandom());  
            while (w.name == lastPicked)
            {
                w = waveDB.Where(x => levelIndex >= x.minLevel && levelIndex <= x.maxLevel ).ToList().GetRandom();
            }
            Debug.Log($"Loading wave groud: {w.name}");
            lastPicked = w.name;
            level.mainSequence.Add(w);
        }
        Debug.Log($"Adding {randosProg} randos");

        for (int i = 0; i < randosProg; i++)
        {
            SpawnData rando = randosDB.GetRandom();
            level.mainSequence.GetRandom().spawns.Add(rando);
        }


        level.boss= bossDB.Where(x => levelIndex >= x.minLevel && levelIndex <= x.maxLevel).ToList().GetRandom();

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
    public int minLevel;
    public int maxLevel;
}