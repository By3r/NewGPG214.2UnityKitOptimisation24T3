using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Dana
{
    public class PlayerDataManager : MonoBehaviour
    {
        /// <summary>
        /// Responsible for managing data related to the player within the game scene. 
        /// Data such as melee unlocking, coin tracking.
        /// Each data is personal to each player.
        /// </summary>

        #region Variables
        public static PlayerDataManager instance;

        [Tooltip("The amount of coins needed to unlock the staff (melee) for the player to weild.")]
        [SerializeField] private int coinTotalForStaff = 5;

        private FirebaseAuth _auth;
        private DatabaseReference _database;
        private string localDataFolderPath;

        private List<MeleeUnlockable> _registeredWeaponPickups = new List<MeleeUnlockable>();
        private PlayerData _currentPlayerData;
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _auth = FirebaseAuth.DefaultInstance;
            _database = FirebaseDatabase.DefaultInstance.RootReference;
            localDataFolderPath = Path.Combine(Application.persistentDataPath, "PlayerData");
            if (!Directory.Exists(localDataFolderPath))
            {
                Directory.CreateDirectory(localDataFolderPath);
            }
        }

        private void Start()
        {
            TryLoadPlayerData();
        }

        #region Public Functions
        /// <summary>
        /// Adds coins to the player's collected total and checks whether the staff should stay locked.
        /// </summary>
        /// <param name="amount"> How much coins does the player gain per coin collision </param>
        public void AddCoins(int amount)
        {
            _currentPlayerData.coinsCollected += amount;
            if (!_currentPlayerData.isSwordLocked && _currentPlayerData.coinsCollected >= coinTotalForStaff)
            {
                UnlockStaff();
            }
            else if (_currentPlayerData.isSwordLocked && _currentPlayerData.coinsCollected <= coinTotalForStaff)
            {
                KeepStaffLocked();
            }
            SavePlayerData(_currentPlayerData);
        }

        /// <summary>
        /// Records the sword unlock event in the player's unique data.
        /// </summary>
        public void RecordSwordUnlockEvent()
        {
            _currentPlayerData.isSwordLocked = true;
            SavePlayerData(_currentPlayerData);
        }
        #region Weapons based Functions
        /// <summary> 
        /// These scripts can be used to be further expanded and used for other weapon unlockables.
        /// </summary>

        /// <summary>
        /// Registers a melee unlockable object to update its state.
        /// </summary>
        public void RegisterWeaponPickup(MeleeUnlockable pickup)
        {
            if (!_registeredWeaponPickups.Contains(pickup))
            {
                _registeredWeaponPickups.Add(pickup);
            }
        }

        /// <summary>
        /// Updates the state of all assigned melee unlockable objects based on the player's data registration
        /// </summary>
        public void UpdateWeaponPickupState()
        {
            foreach (var pickup in _registeredWeaponPickups)
            {
                pickup.TogglePickupScript(_currentPlayerData.isSwordLocked);
            }
        }
        #endregion

        /// <summary>
        /// Saves the player's current data to Firebase and locally.
        /// </summary>
        public void SavePlayerData(PlayerData data)
        {
            _currentPlayerData = data;

            FirebaseUser user = _auth.CurrentUser;
            if (user != null)
            {
                SaveDataToFirebase(user.UserId, data);
                SaveDataLocally(user.UserId, data);
            }
            else
            {
                SaveDataLocally("Guest", data);
            }
        }

        /// <summary>
        /// Loads the player's data from Firebase if authenticated; otherwise, loads locally.
        /// </summary>
        public void LoadPlayerData(System.Action<PlayerData> onDataLoaded)
        {
            FirebaseUser user = _auth.CurrentUser;

            if (user != null)
            {
                LoadDataFromFirebase(user.UserId, data =>
                {
                    if (!IsDefaultPlayerData(data))
                    {
                        onDataLoaded?.Invoke(data);
                    }
                    else
                    {
                        LoadDataLocally(user.UserId, onDataLoaded);
                    }
                });
            }
            else
            {
                LoadDataLocally("Guest", onDataLoaded);
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
                    Debug.Log($"Player data loaded: Checkpoint = Coins = {data.coinsCollected}, Sword Unlocked = {data.isSwordLocked}");
                    _currentPlayerData = data;
                }
                else
                {
                    _currentPlayerData = new PlayerData(0, false);
                    SavePlayerData(_currentPlayerData);
                }

                UpdateWeaponPickupState();
            });
        }

        private bool IsDefaultPlayerData(PlayerData data)
        {
            return data.coinsCollected == 0 && data.isSwordLocked;
        }

        private void UnlockStaff()
        {
            _currentPlayerData.isSwordLocked = true;
            UpdateWeaponPickupState();
        }

        private void KeepStaffLocked()
        {
            _currentPlayerData.isSwordLocked = false;
            UpdateWeaponPickupState();
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
                    Debug.Log("Player data saved to Firebase.");
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
                        Debug.Log("Player data loaded from Firebase.");
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

        private void SaveDataLocally(string userId, PlayerData data)
        {
            string filePath = Path.Combine(localDataFolderPath, $"{userId}.json");
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
            Debug.Log($"Player data saved locally for user: {userId}");
        }

        private void LoadDataLocally(string userId, System.Action<PlayerData> onDataLoaded)
        {
            string filePath = Path.Combine(localDataFolderPath, $"{userId}.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"Player data loaded locally for user: {userId}");
                onDataLoaded?.Invoke(data);
            }
            else
            {
                Debug.LogWarning($"No local player data found for user: {userId}");
                onDataLoaded?.Invoke(default);
            }
        }
        #endregion
    }
}
