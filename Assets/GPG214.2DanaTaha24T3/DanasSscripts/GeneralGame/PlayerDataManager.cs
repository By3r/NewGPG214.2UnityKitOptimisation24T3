using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.IO;

namespace Dana
{
    /// <summary>
    /// 
    /// </summary>

    public class PlayerDataManager : MonoBehaviour
    {
        #region Variables
        private FirebaseAuth _auth;
        private DatabaseReference _database;
        private string _localDataPath;
        #endregion

        private void Start()
        {
            _auth = FirebaseAuth.DefaultInstance;
            _database = FirebaseDatabase.DefaultInstance.RootReference;
            _localDataPath = Path.Combine(Application.persistentDataPath, "PlayerGameData.json");

            TryLoadPlayerData();
        }

        #region Public Functions
        /// <summary>
        /// Saves player data to Firebase and locally if authenticated; otherwise saves data only locally.
        /// </summary>
        public void SavePlayerData(PlayerData data)
        {
            FirebaseUser user = _auth.CurrentUser;

            if (user != null)
            {
                SaveDataToFirebase(user.UserId, data);
                SaveDataLocally(data);
            }
            else
            {
                SaveDataLocally(data);
            }
        }

        /// <summary>
        /// Loads player data from Firebase if they're authenticated; otherwise loads their data locally.
        /// </summary>
        public void LoadPlayerData(System.Action<PlayerData> onDataLoaded)
        {
            FirebaseUser user = _auth.CurrentUser;

            if (user != null)
            {
                LoadDataFromFirebase(user.UserId, onDataLoaded);
            }
            else
            {
                LoadDataLocally(onDataLoaded);
            }
        }
        #endregion

        #region Private Functions
        private void TryLoadPlayerData()
        {
            LoadPlayerData(data =>
            {
                if (!IsDefaultPlayerData(data))
                {
                    Debug.Log($"Player data loaded successfully: Checkpoint = {data.checkpoint}, HP = {data.hp}, Enemies Killed = {data.enemiesKilled}, Coins Collected = {data.coinsCollected}");

                    SetupGameState(data);
                }
                else
                {
                    Debug.Log("No player data found. Starting with default values.");

                    PlayerData defaultData = new PlayerData(1, 5, 0, 0);

                    SavePlayerData(defaultData);
                    SetupGameState(defaultData);
                }
            });
        }

        private bool IsDefaultPlayerData(PlayerData data)
        {
            return data.checkpoint == 0 && data.hp == 0 && data.enemiesKilled == 0 && data.coinsCollected == 0;
        }


        private void SaveDataToFirebase(string userId, PlayerData data)
        {
            string json = JsonUtility.ToJson(data);

            _database.Child("users").Child(userId).Child("playerData").SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save player data to Firebase: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Player data saved to Firebase successfully.");
                }
            });
        }

        private void LoadDataFromFirebase(string userId, System.Action<PlayerData> onDataLoaded)
        {
            _database.Child("users").Child(userId).Child("playerData").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to load player data from Firebase: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        string json = snapshot.GetRawJsonValue();
                        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                        Debug.Log("Player data loaded from Firebase successfully.");
                        onDataLoaded?.Invoke(data);
                    }
                    else
                    {
                        Debug.LogWarning("No player data found in Firebase.");
                        onDataLoaded?.Invoke(default);
                    }
                }
            });
        }

        private void SaveDataLocally(PlayerData data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(_localDataPath, json);
            Debug.Log("Player data saved locally.");
        }

        private void LoadDataLocally(System.Action<PlayerData> onDataLoaded)
        {
            if (File.Exists(_localDataPath))
            {
                string json = File.ReadAllText(_localDataPath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("Player data loaded locally.");
                onDataLoaded?.Invoke(data);
            }
            else
            {
                Debug.LogWarning("No local player data found.");
                onDataLoaded?.Invoke(default);
            }
        }

        private void SetupGameState(PlayerData data)
        {
            Debug.Log($"Setting up game: Checkpoint {data.checkpoint}, HP {data.hp}, Enemies Killed {data.enemiesKilled}, Coins Collected {data.coinsCollected}");
        }
        #endregion
    }
}