using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public string currentPlayerName;
    public int currentPlayerScore;
    public List<BestScore> bestScores;

    private void Awake()
    {
        Debug.Log($"persistentDataPath: {Application.persistentDataPath}");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    public class BestScore
    {
        public float time;
        public string playerName;
        public int score;
    }

    [System.Serializable]
    class SaveData
    {
        public string currentPlayerName;
        public int currentPlayerScore;
        public List<BestScore> bestScores;
    }

    public void SaveNewData()
    {
        SaveData data = new SaveData();
        data.currentPlayerName = currentPlayerName;
        data.currentPlayerScore = currentPlayerScore;
        data.bestScores = bestScores;

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
            bestScores = data.bestScores;

            if(bestScores.Count == 0)
            {
                Debug.LogWarning("No best score available");
            }
            else
            {
                string bestScoreList = "";
                foreach (BestScore bestScore in bestScores)
                {
                    bestScoreList += $"{bestScore.playerName}:{bestScore.score}; ";
                }
                Debug.Log("List of best scores:" + bestScoreList);

            }
        }
    }
}
