using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public string currentPlayerName;
    public int currentPlayerScore;
    public string bestPlayerName;
    public string bestScore;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    class SaveData
    {
        public string currentPlayerName;
        public int currentPlayerScore;
        public string bestPlayerName;
        public string bestScore;
    }

    public void SaveNewData()
    {
        SaveData data = new SaveData();
        data.currentPlayerName = currentPlayerName;
        data.currentPlayerScore = currentPlayerScore;
        data.bestPlayerName = bestPlayerName;
        data.bestScore = bestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/basket-blitz-data.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/basket-blitz-data.json";
        if (File.Exists(path))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            currentPlayerName = data.currentPlayerName;
            currentPlayerScore = data.currentPlayerScore;
            bestPlayerName = data.bestPlayerName;
            bestScore = data.bestScore;
        }
    }
}
