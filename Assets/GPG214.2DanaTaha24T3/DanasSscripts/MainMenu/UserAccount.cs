using System.Collections.Generic;

namespace Dana
{
    [System.Serializable]
    public class UserAccount
    {
        #region Variables
        public string username;
        public string email;
        public string userId;
        public string password;
        #endregion
        public UserAccount(string username, string email, string userId, string password)
        {
            this.username = username;
            this.email = email;
            this.userId = userId;
            this.password = password;
        }
    }

    [System.Serializable]
    public class AccountList
    {
        public List<UserAccount> accounts;

        public AccountList(List<UserAccount> accounts)
        {
            this.accounts = accounts;
        }
    }
}
