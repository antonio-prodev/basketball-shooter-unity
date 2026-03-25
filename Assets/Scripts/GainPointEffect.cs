using System.Collections;
using TMPro;
using UnityEngine;

public class GainPointEffect : MonoBehaviour
{
    private TextMeshPro pointText;
    [SerializeField] private Vector3 initialPosition = new Vector3(-2, 3, 0);
    [SerializeField] private float fadeOutSpeed = 1.0f;
    [SerializeField] private float riseSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pointText == null)
        {
            pointText = GetComponent<TextMeshPro>();

            if (pointText == null)
            {
                Debug.LogError("pointText is null : pointText = GetComponent<T>();");
            }
            else
            {
                pointText.alpha = 0;
            }
        }
    }

    public void PlayPointGainEffect(int point)
    {
        pointText.text = $"+{point}";

        transform.localPosition = initialPosition;

        StartCoroutine(FadeOutEffectRoutine());
        StartCoroutine(RiseEffectRoutine());
    }

    IEnumerator FadeOutEffectRoutine()
    {
        pointText.alpha = 1;
        float startTime = Time.time;
        while (Time.time - startTime < fadeOutSpeed)
        {
            pointText.alpha = Mathf.Lerp(1, 0, Mathf.Clamp01((Time.time - startTime) / fadeOutSpeed)) ;
            yield return null;
        }
        pointText.alpha = 0f;
    }

    IEnumerator RiseEffectRoutine()
    {
        while(pointText.alpha > 0)
        {
            transform.Translate(Vector3.up * Time.deltaTime * riseSpeed);
            yield return null;
        }
    }
}
