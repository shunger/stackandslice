using System.IO;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Persistence
{
    public class SaveManager : MonoBehaviour
    {
        public SaveData Data { get; private set; }

        private string _savePath;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _savePath = Path.Combine(Application.persistentDataPath, Utils.Constants.SAVE_FILE_NAME);
            Load();
        }

        public void Load()
        {
            if (File.Exists(_savePath))
            {
                try
                {
                    string json = File.ReadAllText(_savePath);
                    Data = JsonUtility.FromJson<SaveData>(json);
                    if (Data == null)
                        Data = new SaveData();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SaveManager] Failed to load save: {e.Message}");
                    Data = new SaveData();
                }
            }
            else
            {
                Data = new SaveData();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(_savePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save: {e.Message}");
            }
        }

        public void RecordGameResult(int score, float towerHeight)
        {
            Data.totalGamesPlayed++;
            Data.TrySetHighScore(score);
            Data.TrySetHighestTower(towerHeight);
            Data.AddLeaderboardEntry(score, towerHeight);
            Save();
        }

        public void UpdateSettings(Persistence.SettingsData settings)
        {
            Data.settings = settings;
            Save();
        }

        public void DeleteSave()
        {
            Data = new SaveData();
            if (File.Exists(_savePath))
                File.Delete(_savePath);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SaveManager>();
        }
    }
}
