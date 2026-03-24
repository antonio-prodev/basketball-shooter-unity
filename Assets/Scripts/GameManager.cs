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
    public float[] zRange = {5, 12};
    public float[] yRange = {1, 3};
    private int score;
    [SerializeField] private int timer = 60;
    public bool isGameActive;
    public Cloth netCloth;

    [Header("Sound")]
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        powerIndicatorSlider.wholeNumbers = false;
        powerIndicatorSlider.minValue = 0;
        powerIndicatorSlider.maxValue = 1;
        powerIndicatorSlider.value = 0;
    }

    public void StartGame(int duration)
    {
        timer = duration;
        isGameActive = true;
        welcomePanel.SetActive(false);
        gamePlayUI.SetActive(true);
        SpawnBall();
        StartCoroutine(CountdownTimer());
        LinkPooledBallsToPowerIndicator();
    }

    public void EndGame()
    {
        finalScoreText.text = score.ToString();
        audioSource.PlayOneShot(gameOverSound);
        isGameActive = false;
        timerGameObject.SetActive(false);
        timeUpText.GetComponent<BlinkingText>().StartBlinking();
        DeactiavatePooledBalls();
        gameOverPanel.SetActive(true);
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
            Debug.Log("Activating pooled ball..." + pooledObject.name);
            pooledObject.SetActive(true);
            pooledObject.GetComponent<SphereCollider>().isTrigger = true; // Set to trigger so the ball ignores collision while in kinimatic state
        }
        else
        {
            Debug.LogWarning("No pooled ball available!");
            //Instantiate(ballPrefab, ballSpawnPosition, Quaternion.identity);
        }

        ballSpawnPosition = GenerateRandomSpawnPos(); // Generate a new random spawn position for the next spawn
    }

    void LinkPooledBallsToPowerIndicator()
    {
        foreach(GameObject ball in ObjectPooler.SharedInstance.pooledObjects)
        {
            ball.GetComponent<BallController>().LinkBallToPowerIndicator();
        }
    }

    void DeactiavatePooledBalls()
    {
        foreach(GameObject ball in ObjectPooler.SharedInstance.pooledObjects)
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
}
