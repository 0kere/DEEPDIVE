using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Advertisements;

[System.Serializable]
public class SaveData
{
    public int highscore;
    public List<bool> unlockedPalletes;
    public int equippedPalleteIndex;
    public int totalBits;
 
    public string ToJson() { return JsonUtility.ToJson(this, true); }

    public void LoadFromJson(string newJsonData) { JsonUtility.FromJsonOverwrite(newJsonData, this); }
}

public class GameManager : MonoBehaviour
{
    private static GameManager gm;
    public static GameManager instance
    {
        get
        {
            if (!gm)
            {
                gm = FindObjectOfType<GameManager>() as GameManager;
                if (!gm)
                {
                }
                else
                {
                    gm.Init();
                }
            }
            return gm;
        }
    }
    private void Init() { }

    [Header("Game Over Values")]
    public bool isGameOver = true;
    public delegate void GameOver();
    public static event GameOver gameOverEvent;
    [SerializeField] private float gameOverDelay;
    public WaitForSeconds gameOverDelayWFS;
    [HideInInspector] public UIManager uiM;

    [Header("Game Loop Values")]
    [HideInInspector] public bool allowColorSwitching;
    [SerializeField] float scorePerSecond;
    private float gameTime;
    private float scoreTime;
    private int score;
    [HideInInspector] public int highScore;
    [SerializeField] private float gameStartDelay;
    private WaitForSeconds gameStartDelayWFS;
    public int bitsEarnt; //per game session
    public int totalBits; //ever
    [SerializeField] private float scoreToBitsMod;
    public float gameSpeed;
    public float initGameSpeed;
    [SerializeField] private float maxGameSpeed;
    [SerializeField] private float timeToMaxSpeed;
    [SerializeField] private AnimationCurve increaseCurve;
    [SerializeField] sObstacleSpawner spawner;

    [Header("Player Values")]
    [HideInInspector] public sPlayerController player;
    [HideInInspector] public sColorControl playerColorCont;
    private Color curPlayerCol;
    [SerializeField] private GameObject playerPrefab;
    public TMPro.TextMeshProUGUI scoreText;
    public bool playerIsPrimary;
    public bool tryGyro; //if input is set to gyro atempt to use gyro

    [Header("Gift Values")]
    [SerializeField] private int gamesForGift;
    private int gamesThisSession = 0;

    [Header("Cheats")]
    public bool isInvincible;

    [HideInInspector] public List<sAudioSource> audioSources = new List<sAudioSource>();
    [Header("Ads")]
    [SerializeField] private bool testMode = true;
    private string gameId = "4082449";
    [SerializeField] private int gamesForAd;
    private void Awake()
    {
        uiM = GetComponent<UIManager>();
        gameStartDelayWFS = new WaitForSeconds(gameStartDelay);
        gameOverDelayWFS = new WaitForSeconds(gameOverDelay);

        LoadData();

    }

    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        { 
            uiM.InitGameMenu();
        }
    }
    #region Game
    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop() //start this at the start of every game
    {
        isGameOver = false;
        yield return gameStartDelayWFS;
        SpawnPlayer();
        spawner.StartSpawning();
        gameTime = 0f;
        scoreTime = 0f;
        allowColorSwitching = true;
        scoreText.text = "0";
        score = 0;
        bitsEarnt = 0;

        while (!isGameOver)
        {
            UpdateGameSpeed();
            gameTime += Time.deltaTime;
            scoreTime += Time.deltaTime;
            if (scoreTime > scorePerSecond)
            {
                scoreTime = 0f;
                score++;
                scoreText.text = score.ToString();
            }

            yield return null;
        }
    }

    private void UpdateGameSpeed()
    {
        if (gameTime / timeToMaxSpeed > 1f) // enough tinme has passed that we are at max speed
        {
            gameSpeed = maxGameSpeed;
        }
        else
        {
            gameSpeed = Mathf.Lerp(initGameSpeed, maxGameSpeed, increaseCurve.Evaluate(gameTime / timeToMaxSpeed));
        }
    }


    private void SpawnPlayer()
    {
        Vector3 rot = new Vector3(0, 0, 180);
        //1.25 from top of screen
        float screenTopYValue = (Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y);

        GameObject temp = Instantiate(playerPrefab, new Vector3(0, screenTopYValue - 1.25f, 0), Quaternion.Euler(rot));
        player = temp.GetComponent<sPlayerController>();
        playerColorCont = temp.GetComponent<sColorControl>();
        playerIsPrimary = true; //player alsways starts primary color
    }

    public Color ReturnPlayerColor()
    {
        return playerColorCont.ReturnCurrentColor();
    }

    public float ReturnScore()
    {
        return score;
    }

    public void OnGameOver()
    {
        isGameOver = true;
        gamesThisSession++;
        allowColorSwitching = false;
        gameOverEvent?.Invoke();
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        //Need to calcuate a bits amount to award players based on score
        bitsEarnt += Mathf.RoundToInt(Mathf.Sqrt(score * score * score) * scoreToBitsMod);
        totalBits += bitsEarnt;
        bool isNewHS = false;
        if (score > highScore)
        {
            highScore = score;
            isNewHS = true;
        }

        if (isNewHS)
        {
            sSoundManager.PlayHighScoreClip();
        }
        else
        {
            sSoundManager.PlayGameOverClip();
        }

        GPSHandler.UpdateHighScoreLeaderboard(score);
        GPSHandler.GamePlayed();
        GPSHandler.UpdateScoreAchievement(score);
        ShowAd();

        StartCoroutine(uiM.EndGameEarlyRoutine(isNewHS));
        yield return gameOverDelayWFS;
        Destroy(player.gameObject);
        StartCoroutine(uiM.EndGameRoutine(isNewHS));

        SaveData();
    }

    public bool ReturnGiftAvailable()
    {
        if (gamesThisSession >= gamesForGift)
        {
            gamesThisSession = 0;
            return true;
        }
        return false;
    }
    #endregion

    #region Save/Load

    public void LoadData()
    {
        string result;
        string path = Path.Combine(Application.persistentDataPath, "saveData.sd");
        SaveData save = new SaveData();

        try
        {
            result = File.ReadAllText(path);

            save.LoadFromJson(result);

            highScore = save.highscore;
            totalBits = save.totalBits;
            sColourSwitchManager.instance.ownedPalletes = save.unlockedPalletes;
            sColourSwitchManager.instance.curPalleteIndex = save.equippedPalleteIndex;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read from  {path} with exception {e}");
            result = "";
        }
    }

    public void SaveData()
    {
        SaveData save = new SaveData();
        save.highscore = highScore;
        save.totalBits = totalBits;
        save.unlockedPalletes = sColourSwitchManager.instance.ownedPalletes;
        save.equippedPalleteIndex = sColourSwitchManager.instance.curPalleteIndex;

        string path = Path.Combine(Application.persistentDataPath, "saveData.sd");

        try
        {
            File.WriteAllText(path, save.ToJson());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to write to {path} with exception {e}");
        }
    }

    #endregion

    #region Ads

    private void ShowAd()
    {
        if (Advertisement.IsReady() && gamesThisSession % gamesForAd == 0)
        {
            Advertisement.Show();
        }
        else
        {
            Debug.Log("Ad unavailable right now. Check connection and try again later");
        }
    }

    #endregion
}
