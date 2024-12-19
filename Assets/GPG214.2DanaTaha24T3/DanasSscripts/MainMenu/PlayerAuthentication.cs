using Firebase.Auth;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Responsible for handling user authentication.
    /// </summary>

    public class PlayerAuthentication : MonoBehaviour
    {
        #region Variables
        public bool isAuthenticated { get; set; } = false;

        private FirebaseAuth _firebaseAuth;
        #endregion

        private void Start()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }

    }
}