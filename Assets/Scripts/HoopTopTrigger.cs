using UnityEngine;

public class HoopTopTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        BallPassTracker.SharedInstance.MarkEnteredTop(other.attachedRigidbody);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        BallPassTracker.SharedInstance.ClearEnteredTop(other.attachedRigidbody);
    }
}
