using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HoopBottomTrigger : MonoBehaviour
{
    public AudioClip swishSound;
    public AudioClip achievementSound;
    public GainPointEffect gainPointEffect;
    private AudioSource audioSource;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        gainPointEffect = transform.parent.Find("Point Text").GetComponent<GainPointEffect>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        if (BallPassTracker.SharedInstance.HasEnteredTop(other.attachedRigidbody))
        {
            audioSource.PlayOneShot(achievementSound,0.8f);
            gameManager.AddScore(1);
            gainPointEffect.PlayPointGainEffect(1);
        }
    }
}
