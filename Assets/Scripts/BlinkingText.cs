using System.Collections;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    private float blinkInterval = 0.5f;
    private bool blinking = false;
    private CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartBlinking()
    {
        blinking = true;
        StartCoroutine(BlinkingRoutine());
    }

    public void StopBlinking()
    {
        blinking = false;
    }

    IEnumerator BlinkingRoutine()
    {
        while (blinking)
        {
            canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
