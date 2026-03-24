using TMPro;
using UnityEngine;

public class WelcomeUIHandler : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (descriptionText != null)
        {
            descriptionText.text = descriptionText.text.Replace(":name", DataManager.Instance.currentPlayerName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
