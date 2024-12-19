using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Firebase.Auth;
using Firebase;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

namespace Dana
{
    /// <summary>
    /// Loads saved account and changes log-in UI slots accordingly.
    /// </summary>

    public class UsersLogin : MonoBehaviour
    {
        #region Variables
        public string accountSaveFileName = "SavedAccounts.json";

        [Header("UI Elements")]
        [SerializeField] private TMP_Text[] nameTexts;
        [SerializeField] private TMP_InputField[] passwordInputs;
        [SerializeField] private TMP_Text feedbackText;
        [SerializeField] private FirebaseAuthenticationNewUser firebaseAuthenticationNewUser;
        [SerializeField] private PlayerAuthentication playerAuthentication;

        private string _localDataPath;
        private List<UserAccount> _accounts = new List<UserAccount>();
        #endregion

        private void Start()
        {
            _localDataPath = Path.Combine(Application.persistentDataPath, accountSaveFileName);
            RefreshLoginSlots();
        }

        #region Public Functions
        public void RefreshLoginSlots()
        {
            LoadAccounts();
            UpdateSlots();
        }

        public void ShowFeedback(string message, int hideFeedbackTime)
        {
            feedbackText.text = message;
            Invoke("HideFeedback", hideFeedbackTime);
        }

        public void OnConfirmButtonPressed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _accounts.Count)
            {
                ShowFeedback("No account associated with this slot.", 3);
                return;
            }

            string email = _accounts[slotIndex].email;
            string enteredPassword = passwordInputs[slotIndex].text;

            if (string.IsNullOrEmpty(enteredPassword))
            {
                ShowFeedback("Please enter your password.", 1);
                return;
            }
            StartCoroutine(AuthenticateUser(email, enteredPassword));
        }

        /// <summary>
        /// To be assigned to the button responsible of deleting specific account data according to which slot it is stored in.
        /// </summary>
        public void OnDeleteButtonPressed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _accounts.Count)
            {
                ShowFeedback("No account associated with this slot.", 3);
                return;
            }

            _accounts.RemoveAt(slotIndex);
            SaveAccounts();
            UpdateSlots();
            ShowFeedback("Account deleted.", 2);
            playerAuthentication.isAuthenticated = false;
        }
        #endregion

        #region Private Functions
        private void HideFeedback()
        {
            feedbackText.text = string.Empty;
        }

        private void LoadAccounts()
        {
            if (!File.Exists(_localDataPath)) return;
            string json = File.ReadAllText(_localDataPath);
            _accounts = JsonUtility.FromJson<AccountList>(json)?.accounts ?? new List<UserAccount>();
        }

        private void SaveAccounts()
        {
            string json = JsonUtility.ToJson(new AccountList(_accounts), true);
            File.WriteAllText(_localDataPath, json);
        }

        private void UpdateSlots()
        {
            for (int i = 0; i < nameTexts.Length; i++)
            {
                if (i < _accounts.Count)
                {
                    nameTexts[i].text = _accounts[i].username;
                }
                else
                {
                    nameTexts[i].text = "No Account";
                }
            }
        }
        IEnumerator AuthenticateUser(string email, string password)
        {
            Task<AuthResult> signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);

            while (!signInTask.IsCompleted)
            {
                yield return null; 
            }

            if (signInTask.IsFaulted)
            {
                ShowFeedback("Login failed. Please check your log in details.", 3);
                yield break; 
            }

            if (signInTask.IsCanceled)
            {
                ShowFeedback("Login canceled.", 2);
                yield break;
            }

            if (signInTask.IsCompletedSuccessfully)
            {
                FirebaseUser user = signInTask.Result.User;

                if (user != null)
                {
                    ShowFeedback("Login successful. You're now authenticated.", 2);

                    playerAuthentication.isAuthenticated = true;

                    UserProfile profile = new UserProfile { DisplayName = user.Email };
                    Task updateProfileTask = user.UpdateUserProfileAsync(profile);

                    while (!updateProfileTask.IsCompleted)
                    {
                        yield return null;
                    }

                    if (updateProfileTask.IsCompletedSuccessfully)
                    {
                        Debug.Log("User profile updated successfully.");
                    }
                    else
                    {
                        Debug.LogWarning("User profile could not be updated.");
                    }
                    SceneManager.LoadScene(1);
                }
                else
                {
                    ShowFeedback("Unexpected error occurred. Please try again.", 2);
                }
            }
        }



        #endregion
    }
}
