using Firebase.Auth;
using UnityEngine;
using System.IO;
using TMPro;
using System.Collections.Generic;
using Firebase.Database;
using System;

namespace Dana
{
    /// <summary>
    /// Creates a new account, saves it locally and adds the user to Firebase.
    /// </summary>

    public class FirebaseAuthenticationNewUser : MonoBehaviour
    {
        #region Variables
        public FirebaseAuth firebaseAuth;

        [Header("UI Elements")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_InputField confirmPasswordInput;
        [SerializeField] private TMP_Text feedbackText;
        [SerializeField] private UsersLogin usersLogin;

        private string _localDataPath;
        #endregion

        private void Start()
        {
            firebaseAuth = FirebaseAuth.DefaultInstance;
            _localDataPath = Path.Combine(Application.persistentDataPath, usersLogin.accountSaveFileName);
            CreateFileIfNotExists();
        }

        #region Public Functions
        public void OnSignUpButton()
        {
            string username = usernameInput.text;
            string email = emailInput.text;
            string password = passwordInput.text;
            string confirmPassword = confirmPasswordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                usersLogin.ShowFeedback("Do not leave any of the fields empty.", 3);
                return;
            }
            if (!IsValidEmail(email))
            {
                usersLogin.ShowFeedback("Email invalid.", 1);
                return;
            }

            if (password.Length < 6)
            {
                usersLogin.ShowFeedback("Password must be at least 6 characters long.", 2);
                return;
            }

            if (password != confirmPassword)
            {
                usersLogin.ShowFeedback("The passwords do not match.", 2);
                return;
            }

            usersLogin.ShowFeedback("Account successfully created. Log-in to start the game.", 3);
            RegisterUser(username, email, password);
            Invoke("ClearInputFields", 2);
        }
        #endregion

        #region Private Functions
        private void CreateFileIfNotExists()
        {
            if (!File.Exists(_localDataPath))
            {
                File.WriteAllText(_localDataPath, JsonUtility.ToJson(new AccountList(new List<UserAccount>()), true));
            }
        }

        private void RegisterUser(string username, string email, string password)
        {
            if (AccountExistsLocally(username, email))
            {
                usersLogin.ShowFeedback("An account with this username or email already exists.", 3);
                return;
            }

            CheckFirebaseForDuplicates(email, (exists) =>
            {
                if (exists)
                {
                    usersLogin.ShowFeedback("An account with this email already exists.", 3);
                }
                else
                {
                    RegisterUser(username, email, password);
                }
            });

            firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    usersLogin.ShowFeedback("Account creation failed. Please try again.", 2);
                    return;
                }

                FirebaseUser newUser = task.Result.User;

                if (newUser != null)
                {
                    SaveAccountLocally(username, email, newUser.UserId, password);
                    usersLogin.ShowFeedback("Account created successfully! Go to Log-in page", 2);
                }
            });
        }

        #region Checks if account already exists locally or in firebase.
        private bool AccountExistsLocally(string username, string email)
        {
            List<UserAccount> accounts = LoadAccounts();

            foreach (var account in accounts)
            {
                if (account.email.Equals(email, System.StringComparison.OrdinalIgnoreCase) ||
                    account.username.Equals(username, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckFirebaseForDuplicates(string email, Action<bool> callback)
        {
            FirebaseDatabase.DefaultInstance.GetReference("users")
                .OrderByChild("email")
                .EqualTo(email)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled || task.Result == null)
                    {
                        Debug.LogError("Can't check firebase for dupes.");
                        callback(false);
                        return;
                    }
                    callback(task.Result.Exists);
                });
        }
        #endregion

        private void SaveAccountLocally(string username, string email, string userId, string password)
        {
            List<UserAccount> accounts = LoadAccounts();
            accounts.Add(new UserAccount(username, email, userId, password));
            string json = JsonUtility.ToJson(new AccountList(accounts), true);
            File.WriteAllText(_localDataPath, json);
            Debug.Log("Saved account locally.");

            usersLogin.RefreshLoginSlots();
        }
        private List<UserAccount> LoadAccounts()
        {
            if (!File.Exists(_localDataPath)) return new List<UserAccount>();
            string json = File.ReadAllText(_localDataPath);
            return JsonUtility.FromJson<AccountList>(json)?.accounts ?? new List<UserAccount>();
        }
        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.EndsWith(".com");
        }

        private void ClearInputFields()
        {
            usernameInput.text = string.Empty;
            emailInput.text = string.Empty;
            passwordInput.text = string.Empty;
            confirmPasswordInput.text = string.Empty;
        }
        #endregion
    }
}
