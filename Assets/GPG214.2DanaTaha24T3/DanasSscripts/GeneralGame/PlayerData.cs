[System.Serializable]
public struct PlayerData
{
    #region Variables
    public int checkpoint;
    public int hp;
    public int enemiesKilled;
    public int coinsCollected;
    public bool isSwordUnlocked;
    #endregion

    #region Struct instance.
    public PlayerData(int checkpoint, int hp, int enemiesKilled, int coinsCollected, bool isSwordUnlocked)
    {
        this.checkpoint = checkpoint;
        this.hp = hp;
        this.enemiesKilled = enemiesKilled;
        this.coinsCollected = coinsCollected;
        this.isSwordUnlocked = isSwordUnlocked;
    }
    #endregion
}
