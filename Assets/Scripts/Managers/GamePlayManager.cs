using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager: GameManager
{
    public bool isDebug = false;
    public delegate void GameEnded();
    public delegate void GameStarted();
    public delegate void TakedDamage(bool taked);
    public TakedDamage takedDamage;
    public GameEnded gameEnded;
    public GameEnded gameStarted;
    public enum GameStatus { StandbBy, Started, Paused, Playing, Ended };
    public GameStatus gameStatus = GameStatus.StandbBy;

    public UIManager ui;
    public Transform player;
    public EnemiesManagerModel enemiesManager; 
    //public ConfigurationManager configure;

    public float _playerHealth = 10;
    public float playerHealth
    {
        get
        {
            return _playerHealth;
        }
        set
        {
            _playerHealth = value;
            if (_playerHealth <= 0)
            {
                EndGame();
            }
        }
    }
    public float startDelay;

    public int points = 0;
    private float timer = 0;
    private float duration;
    private Color fogColor;

    float damageTimer = 3;
    [SerializeField]
    AudioSpectrumManager audioSpectrumManager;

    public bool test;
    public PlayerModel playerModel;
    [SerializeField]
    Material plataform;
    [SerializeField]
    AudioSource audioSource;

    public static GamePlayManager obj;

    void Awake()
    {
        if (obj == null)
            _obj = obj = this;
        ui = GetComponent<UIManager>();
        //configure = GetComponent<ConfigurationManager>();
        fogColor = RenderSettings.fogColor;
        level = PlayerPrefs.GetInt("Level", 1);
        enemiesManager.Init();
        DisscardEnemies();

        if (test)
            StartCoroutine(StartTest());
    }
    private void DisscardEnemies()
    {
        int enemyCount = PlayerPrefs.GetInt(GetLevelTag("Enemies"), 2);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = enemyCount; i < enemies.Length; i++)
        {
            enemiesManager.NotifyDied(enemies[i].name);
            Destroy(enemies[i]);
        }


    }
    private void Start()
    {
        //fogColor = configure.SetEnviroment();

        StartCoroutine(StartLights());
    }
    IEnumerator StartLights()
    {
        plataform.SetColor("Color_4FB40C3", Color.black);
        Color intensityLigth = Color.black;
        //Color colorLevel = configure.GetLevel(level).getPlataformColor();
        Color colorLevel = Color.blue;
        do
        {
            intensityLigth = Color.Lerp(intensityLigth, colorLevel, Time.deltaTime * 0.5f);
            plataform.SetColor("Color_4FB40C3", intensityLigth);
            yield return null;
        } while (!intensityLigth.Equals(colorLevel));
        yield return null;
    }
    IEnumerator StartTest()
    {
        //fogColor = configure.SetEnviroment();
        yield return new WaitForSeconds(2);
        //For test
        StartGame();
    }
    public void StartGame()
    {
        duration = audioSpectrumManager.audioSource.clip.length;
        if (gameStarted != null)
        {
            gameStarted();
        }
        StartCoroutine(WaitFor());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStatus == GameStatus.Started)
        {
            timer += Time.deltaTime;
            ui.UpdateTimer((int)(duration - timer));
            if (timer > duration)
            {
                EndGame();
            }
            if (damageTimer < 2)
            {
                damageTimer += Time.deltaTime;

                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColor, Time.deltaTime);
                if (damageTimer >= 2)
                {
                    ResetDamageStatus();
                }
            }
        }
    }
    private void EndGame()
    {
        for (int i = 1; i <= 10; i++)
        {
            string score = PlayerPrefs.GetString("Score_" + i, "");
            if (score.Length > 0)
            {
                if (points > int.Parse(score))
                {
                    string pointsStr = points.ToString();
                    while (pointsStr.Length < 10)
                    {
                        pointsStr = "0" + pointsStr;
                    }
                    PlayerPrefs.SetString(pointsStr, "Score_" + i);
                    break;
                }
            }
        }
        gameStatus = GameStatus.Ended;
        if (gameEnded != null)
        {
            //ui.UpdatePoints(points);
            RenderSettings.fogColor = fogColor;
            gameEnded();
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(startDelay);
        gameStatus = GameStatus.Started;

    }
    public static bool isPlaying()
    {
        return obj.gameStatus == GamePlayManager.GameStatus.Started;
    }
    public override void SetPoints(int newpoints)
    {
        points += newpoints;

        ui.UpdatePoints(points, newpoints);
    }

    public void Damage(int amount, AudioClip clip)
    {
        audioSpectrumManager.audioSource.volume = 0;
        RenderSettings.fogColor = Color.red;

        plataform.color = Color.red;
        plataform.SetColor("_EmissionColor", Color.red);

        damageTimer = 0;
        audioSource.clip = clip;
        audioSource.Play();
        playerHealth -= amount;
        SetPoints(-amount * 100);
        ui.UpdateHealthBar(playerHealth);
        if (takedDamage != null)
        {
            takedDamage(true);
        }
    }
    private void ResetDamageStatus()
    {
        plataform.color = Color.blue;
        plataform.SetColor("_EmissionColor", Color.blue);
        RenderSettings.fogColor = fogColor;
        audioSpectrumManager.audioSource.volume = 1;
        if (takedDamage != null)
        {
            takedDamage(false);
        }
    }
    public void GotoScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public static string GetLevelTag(string tag)
    {
        return "L" + obj.level + "_" + tag;
    }
    public void SpawnNewEnemy(string name)
    {
        Debug.Log("Spawning enemy...");
        enemiesManager.NotifyDied(name);
        StartCoroutine(enemiesManager.SpawnNewEnemyAt(20 - level * 2));
    }

    public override void SetMessage(string message)
    {
        ui.UIDebug(message);
    }
}

