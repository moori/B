using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("UI")]
    public PauseMenu pauseMenu;
    public RenamePanel renamePanel;
    public GameObject loading;

    public static bool IsGamePaused;
    public static bool IsGameOver;

    public static System.Action OnGameOver;

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
        gameoverScreen.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        loading.gameObject.SetActive(true);
        loading.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        this.DelayAction(0.2f, () => { 
            loading.gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        if (canRestart && Input.anyKeyDown)
        {
            IsGameOver = false;
            loading.SetActive(true);
            this.DelayAction(0, () => { 
                SceneManager.LoadScene("Game");
            });
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !GameController.IsGamePaused)
        {
            pauseMenu.Pause();
            return;
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
    }

    public void GameOver()
    {
        IsGameOver = true;
        OnGameOver?.Invoke();
        StopAllCoroutines();
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
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

        newBest.gameObject.SetActive(score>HighScoreManager.GetBestScore());

        if (score > HighScoreManager.GetBestScore())
        {
            HighScoreManager.SetBestScore(score);
            var waiting = true;
            renamePanel.Show();
            renamePanel.OnRenameComplete = () => waiting = false;
            yield return new WaitUntil(()=>!waiting);
            HighScoreManager.SendHighscore(score);
        }
        bestScore.text = $"best score\n{string.Format("{0:000000000}",HighScoreManager.GetBestScore())}";
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

    public static bool IsTouchingBounds(Vector3 pos)
    {
        return Mathf.Abs(pos.x) >= HORIZONTAL_MOVEMENT_BOUND || Mathf.Abs(pos.y) >= VERTICAL_MOVEMENT_BOUND;
    }


    public IEnumerator RuneWave(WaveData srcWaveData)
    {
        //currentWave=
        yield return null;
    }
}
