namespace Dana
{
    /// <summary>
    /// Tracks each player's amount of coins collected and the status of their ability to use the melee.
    /// </summary>

    [System.Serializable]
    public struct PlayerData
    {
        #region Variables
        public int coinsCollected;
        public bool isSwordUnlockable;
        #endregion

        #region Struct instance.
        public PlayerData(int coinsCollected, bool isSwordUnlocked)
        {
            this.coinsCollected = coinsCollected;
            this.isSwordUnlockable = isSwordUnlocked;
        }
        #endregion
    }
}