using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlling behaviour of game. This is singleton class.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Scripts")]
    public SpawnController spawnController;
    public CameraController cameraController;

    [Header("Objects")]
    public GameObject levelLostObj;
    public GameObject levelWonObj;
    public GameObject startingUIObj;
    public GameObject currentLevelObj;
    public GameObject nextLevelObj;
    public GameObject settingsUI;
    public GameObject scoreAddedObj;

    [Header("UI")]
    public Text scoreText;
    public Text bestScoreText;
    public Text levelProgressText;
    public Text levelWonText;
    public Text currentLevelText;
    public Text nextLevelText;
    public Image progressBarImage;
    public Image backgroundBarImage;

    [Header("Color")]
    public Color nextLevelColor;
    public Color currentLevelColor;

    [Header("Other")]
    public bool EOG;    // end of game
    public bool forceQuit;
    public bool levelCleared;

    [HideInInspector]
    public int level = 0;

    int score;
    float platformAmount;
    float percent = 0;

    public static GameManager instance;

    /// <summary>
    /// Awake is first function that is called and here are defined all variables.
    /// </summary>
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        score = 0;
        platformAmount = spawnController.platformNumber;
        scoreText.text = score + "";
        bestScoreText.text = "BEST: " + PlayerPrefs.GetInt("best_score");

        if (PlayerPrefs.GetInt("level") == 0)
            PlayerPrefs.SetInt("level", 1);

        level = PlayerPrefs.GetInt("level");

        currentLevelText.text = level + "";
        nextLevelText.text = level + 1 + "";
    }

    /// <summary>
    /// Checks every frame if Escape or left mouse button is pressed.
    /// ESC is for quit.
    /// Left mouse button for new level or restarting.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            forceQuit = true;
            Application.Quit();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EOG)
                RestartLevel();
            else if (levelCleared)
                StartNewLevel();
        }
    }

    /// <summary>
    /// Restart all variables to default values.
    /// </summary>
    void RestartLevel()
    {
        spawnController.RestartLevel();

        levelLostObj.SetActive(false);
        EOG = false;

        score = 0;
        scoreText.text = score + "";
        bestScoreText.text = "BEST: " + PlayerPrefs.GetInt("best_score");
        startingUIObj.SetActive(true);

        platformAmount = spawnController.platformNumber;
        progressBarImage.fillAmount = 0;
    }

    /// <summary>
    /// Set all varibales to match next level values.
    /// </summary>
    void StartNewLevel()
    {
        levelWonObj.SetActive(false);
        levelCleared = false;

        level++;
        currentLevelText.text = level + "";
        nextLevelText.text = level + 1 + "";
        PlayerPrefs.SetInt("level", level);

        bestScoreText.text = "BEST: " + PlayerPrefs.GetInt("best_score");
        startingUIObj.SetActive(true);

        platformAmount = spawnController.platformNumber;
        progressBarImage.fillAmount = 0;

        spawnController.NewLevel();
    }

    /// <summary>
    /// Fill image that shows current progress.
    /// </summary>
    public void RefreshProgress()
    {
        platformAmount--;

        if (platformAmount > 0)
            percent = 1f - platformAmount / spawnController.platformNumber;
        else
            percent = 1;

        progressBarImage.fillAmount = percent;
    }

    /// <summary>
    /// Add points to score and show score using UI element.
    /// </summary>
    /// <param name="points">Amount of points to add.</param>
    public void AddScore(int points)
    {
        GameObject obj = Instantiate(scoreAddedObj, GameObject.Find("Canvas").transform);
        obj.GetComponent<ScoreTextController>().point = points;

        score += points;
        scoreText.text = score + "";
    }

    /// <summary>
    /// Define end of game and show it.
    /// </summary>
    public void EndOfGame()
    {
        EOG = true;
        levelProgressText.text = Mathf.Round(percent * 10000) / 100 + "% COMPLETED";
        levelLostObj.SetActive(true);
        SaveScore();
    }

    /// <summary>
    /// Define level won and show it to player.
    /// </summary>
    public void LevelWon()
    {
        RefreshProgress();
        levelCleared = true;

        levelWonText.text = "Level " + level + " passed";
        levelWonObj.SetActive(true);

        SaveScore();
    }

    /// <summary>
    /// Save score to player prefs if it is new best score.
    /// </summary>
    void SaveScore()
    {
        if (score > PlayerPrefs.GetInt("best_score"))
            PlayerPrefs.SetInt("best_score", score);
    }

    /// <summary>
    /// Turn on/off settings.
    /// </summary>
    public void SettingsButtonToggle()
    {
        settingsUI.SetActive(!settingsUI.activeSelf);
    }

    /// <summary>
    /// Turn on/off sound.
    /// </summary>
    public void SoundToggle()
    {
        SoundManager.instance.ToggleSound();
    }
}
