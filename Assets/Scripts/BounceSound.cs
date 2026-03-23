// BounceSound.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BounceSound : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Clip to play for bounces.")]
    public AudioClip bounceClip;

    [Tooltip("If null, the AudioSource on this GameObject will be used.")]
    public AudioSource audioSource;

    [Header("Impact → Volume mapping")]
    [Tooltip("Minimum impact speed that produces a (very small) sound.")]
    public float minImpactSpeed = 0.5f;

    [Tooltip("Impact speed that maps to full (1.0) on the curve.")]
    public float maxImpactSpeed = 8f;

    [Tooltip("Global multiplier applied after the curve evaluation (0..1).")]
    [Range(0f, 1f)]
    public float volumeMultiplier = 1f;

    [Tooltip("Curve that shapes how normalized impact (0..1) maps to volume.")]
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Pitch Variation")]
    [Tooltip("Maximum pitch offset (±) applied based on impact intensity.")]
    public float maxPitchVariation = 0.15f;

    [Header("Collision Filtering")]
    [Tooltip("Only trigger when colliding with objects using this tag.")]

    [Header("Events")]
    public UnityEvent onBounce; // optional hook for VFX, particles, etc.

    private void Reset()
    {
        // sensible defaults
        minImpactSpeed = 0.5f;
        maxImpactSpeed = 8f;
        volumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError($"{nameof(BounceSound)} requires an AudioSource (attach one or assign in inspector).", this);
            enabled = false;
            return;
        }

        audioSource.playOnAwake = false;
        // recommended for positional SFX; change to 0 for UI/2D
        audioSource.spatialBlend = 0.8f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag)) return;

        if (bounceClip == null)
        {
            Debug.LogWarning("BounceSound: bounceClip is not assigned.", this);
            return;
        }

        // Use the collision relative velocity as impact intensity
        float impactSpeed = collision.relativeVelocity.magnitude;

        // Map impactSpeed to 0..1 using min/max then evaluate the curve
        float t = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impactSpeed);
        t = Mathf.Clamp01(t);
        float curveVal = volumeCurve.Evaluate(t);

        float finalVolume = Mathf.Clamp01(curveVal * volumeMultiplier);
        if (finalVolume <= 0f) return; // nothing to play

        // Apply pitch variation based on intensity (so harder impacts sound "sharper")
        float pitchOffset = (t - 0.5f) * 2f * maxPitchVariation; // maps t=0..1 to -max..+max
        float prevPitch = audioSource.pitch;
        audioSource.pitch = Mathf.Clamp(1f + pitchOffset, 0.5f, 2f);

        audioSource.PlayOneShot(bounceClip, finalVolume);

        // restore pitch so other calls aren't affected
        audioSource.pitch = prevPitch;

        onBounce?.Invoke();
    }
}