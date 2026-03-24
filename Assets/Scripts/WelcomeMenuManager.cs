using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class WelcomeMenuManager : MonoBehaviour
{
    public Button continueButton;
    public TMP_InputField playerNameInputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataManager.Instance.LoadData();
        if (!string.IsNullOrWhiteSpace(DataManager.Instance.currentPlayerName))
        {
            playerNameInputField.text = DataManager.Instance.currentPlayerName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNameInputField.text != null)
        {
            if (string.IsNullOrWhiteSpace(playerNameInputField.text)  && continueButton.IsInteractable())
            {
                continueButton.interactable = false;
            }
            else if (!string.IsNullOrWhiteSpace(playerNameInputField.text) && !continueButton.IsInteractable())
            {
                continueButton.interactable = true;
            }
        }

    }

    public void ContinueToGame()
    {
        DataManager.Instance.currentPlayerName = playerNameInputField.text.Trim();
        DataManager.Instance.SaveNewData();
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
