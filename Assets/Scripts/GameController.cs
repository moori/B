using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Waves")]
    public List<WaveData> waveDatas;
    public WaveData currentWave;
    public int waveNumber;

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
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI newBest;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI restart;
    private bool canRestart;

    [Header("Score")]
    public TextMeshProUGUI scoreText;
    public int score;
    private bool scoreTextTweening;

    public const int COIN_SCORE = 20;
    public const int BULLET_HIT_SCORE = 1;


    public const float HORIZONTAL_MOVEMENT_BOUND = 11;
    public const float VERTICAL_MOVEMENT_BOUND = 5.5f;

    private LevelController levelController;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Enemy.OnEnemyDeath += OnEnemyDeath;
        player = FindObjectOfType<Player>();

        rand = new System.Random();

        Bullet.OnBulletHit += OnBulletHit;

        levelController = FindObjectOfType<LevelController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (canRestart && Input.anyKeyDown)
        {
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator Start()
    {
        gameoverScreen.SetActive(false);
        yield return new WaitForSeconds(2f);

        //for (int i = 0; i < maxEnemies; i++)
        //{
        //    Spawn();
        //    yield return new WaitForSeconds(0.66f);
        //}
        levelController.StartLevel();

        StartCoroutine(BatterySpawnerRoutine());
        UpdateScoreText(0);
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
        var enemy = PoolManager.instance.GetSimpleEnemy();
        enemy.Respawn(pos);
        enemiesAlive.Add(enemy);
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        return;
        enemiesAlive.Remove(enemy);
        DOVirtual.DelayedCall(6f, () =>
        {
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
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        var savedBest = PlayerPrefs.GetInt("bestScore", 0);
;       yield return new WaitForSeconds(1f);

        gameoverScreen.SetActive(true);
        finalScore.gameObject.SetActive(false);
        newBest.gameObject.SetActive(false);
        bestScore.gameObject.SetActive(false);
        restart.gameObject.SetActive(false);


        yield return new WaitForSeconds(1f);
        finalScore.gameObject.SetActive(true);
        var ss = 0;
        var elapsedTime = 0f;
        while (ss<score)
        {
            ss = Mathf.RoundToInt( Mathf.Lerp(0, score, elapsedTime / 1.5f));
            elapsedTime += Time.deltaTime;
            finalScore.text = $"final score\n{string.Format("{0:000000000}", ss)}";
            yield return new WaitForEndOfFrame();
        }

        newBest.gameObject.SetActive(score>savedBest);

        if (score > savedBest)
        {
            PlayerPrefs.SetInt("bestScore", score);
            savedBest = PlayerPrefs.GetInt("bestScore", 0);
        }
        bestScore.text = $"best score\n{string.Format("{0:000000000}",savedBest)}";
        bestScore.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        restart.gameObject.SetActive(true);
        canRestart = true;
    }

    public IEnumerator BatterySpawnerRoutine()
    {
        while (true)
        {
            var ammoPercent = player.ammo / (float)player.maxAmmo;
            if (ammoPercent < .2f)
            {
                if (Random.value <= .5f)
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

    public void OnCollectCoin()
    {
        score += player.coins + COIN_SCORE;
        UpdateScoreText(score);
    }
    public void OnBulletHit()
    {
        score += BULLET_HIT_SCORE;
        UpdateScoreText(score);
    }
        

    public void UpdateScoreText(int score)
    {
        scoreText.text = string.Format("{0:000000000}", score);
        if (!scoreTextTweening)
        {
            scoreTextTweening = true;
            scoreText.transform.DOPunchScale(Vector3.one * 0.1f, 0.15f).OnComplete(() =>
            {
                scoreTextTweening = false;
            });
        }
    }

    public static Vector3 GetClosestPositionInsideBounds(Vector3 pos)
    {
        return new Vector3(Mathf.Clamp(pos.x, -HORIZONTAL_MOVEMENT_BOUND, HORIZONTAL_MOVEMENT_BOUND), Mathf.Clamp(pos.y, -VERTICAL_MOVEMENT_BOUND, VERTICAL_MOVEMENT_BOUND), 0);
    }


    public IEnumerator RuneWave(WaveData srcWaveData)
    {
        //currentWave=
        yield return null;
    }
}
