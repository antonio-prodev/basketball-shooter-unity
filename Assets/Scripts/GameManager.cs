using NUnit.Framework;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public GameObject gamePlayUI;
    public GameObject welcomePanel;
    public GameObject gameOverPanel;
    public GameObject ballPrefab;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI finalScoreText;
    public GameObject timerGameObject;
    public TextMeshProUGUI timeUpText;
    public Slider powerIndicatorSlider;
    public Vector3 ballSpawnPosition = new Vector3(0, 2, 0);
    public float[] zRange = { 5, 12 };
    public float[] yRange = { 1, 3 };
    private int score;
    [SerializeField] private int timer = 60;
    public bool isGameActive;
    public Cloth netCloth;

    [Header("Sound")]
    public AudioClip gameOverSound;
    private AudioSource audioSource;
    public TextMeshProUGUI bestScoreText;
    private int originalTimer;
    public GameObject pauseUI;
    public Button pauseButton;
    public TextMeshProUGUI gameOverText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        powerIndicatorSlider.wholeNumbers = false;
        powerIndicatorSlider.minValue = 0;
        powerIndicatorSlider.maxValue = 1;
        powerIndicatorSlider.value = 0;

        bestScoreText.text = "Best Score: 0";
    }

    public void StartGame(int duration)
    {
        timer = duration;
        originalTimer = timer;
        isGameActive = true;
        welcomePanel.SetActive(false);
        gamePlayUI.SetActive(true);
        SpawnBall();
        StartCoroutine(CountdownTimer());
        LinkPooledBallsToPowerIndicator();

        UpdateBestScore();
    }

    void UpdateBestScore()
    {
        if (DataManager.Instance.bestScores != null)
        {
            DataManager.BestScore bestScore = DataManager.Instance.bestScores.Find(bestScore => bestScore.time == originalTimer);
            Debug.Log("Finding if there is a best score for timer == " + originalTimer);
            if (bestScore != null)
            {
                if (score <= bestScore.score)
                {
                    Debug.Log("score <= bestScore.score");
                    bestScoreText.text = $"Best Score: {bestScore.score} ({bestScore.playerName})";
                }
                else
                {
                    gameOverText.text = "New Best Score";
                    gameOverText.color = Color.green;
                    bestScoreText.text = $"Best Score: {score} ({DataManager.Instance.currentPlayerName})";
                    bestScore.score = score;
                    bestScore.playerName = DataManager.Instance.currentPlayerName;
                }
            }
            else
            {
                bestScore = new DataManager.BestScore
                {
                    time = originalTimer,
                    score = score,
                    playerName = DataManager.Instance.currentPlayerName
                };

                DataManager.Instance.bestScores.Add(bestScore);
            }
        }
        else
        {
            Debug.LogError("DataManager.Instance.bestScores is null");
        }
    }

    public void EndGame()
    {
        finalScoreText.text = score.ToString();
        audioSource.PlayOneShot(gameOverSound);
        isGameActive = false;
        timerGameObject.SetActive(false);
        timeUpText.GetComponent<BlinkingText>().StartBlinking();
        DeactiavatePooledBalls();

        UpdateBestScore();
        gameOverPanel.SetActive(true);
        DataManager.Instance.SaveNewData();
    }

    Vector3 GenerateRandomSpawnPos()
    {
        float zPos = Random.Range(zRange[0], zRange[1]);
        float yPos = Random.Range(yRange[0], yRange[1]);

        return new Vector3(0, yPos, zPos);
    }

    IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            timerText.text = timer.ToString();
        }
        EndGame();
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
        Debug.Log("Score: " + score);
    }

    public void SpawnBall()
    {
        Debug.Log("Spawning ball...");
        GameObject pooledObject = ObjectPooler.SharedInstance.GetPooledObject();
        if (pooledObject != null)
        {
            pooledObject.transform.position = ballSpawnPosition;
            pooledObject.transform.rotation = Quaternion.identity;
            pooledObject.SetActive(true);
            pooledObject.GetComponent<SphereCollider>().isTrigger = true; // Set to trigger so the ball ignores collision while in kinimatic state
        }

        ballSpawnPosition = GenerateRandomSpawnPos(); // Generate a new random spawn position for the next spawn
    }

    void LinkPooledBallsToPowerIndicator()
    {
        foreach (GameObject ball in ObjectPooler.SharedInstance.pooledObjects)
        {
            ball.GetComponent<BallController>().LinkBallToPowerIndicator();
        }
    }

    void DeactiavatePooledBalls()
    {
        foreach (GameObject ball in ObjectPooler.SharedInstance.pooledObjects)
        {
            if (ball.activeInHierarchy)
            {
                ball.SetActive(false);
            }
        }

        powerIndicatorSlider.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToPlayerMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseUI.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}
