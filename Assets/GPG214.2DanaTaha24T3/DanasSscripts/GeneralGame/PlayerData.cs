[System.Serializable]
public struct PlayerData 
{
    #region Variables
    public int checkpoint; 
    public int hp; 
    public int enemiesKilled; 
    public int coinsCollected; 
    #endregion

    #region Class instance
    public PlayerData(int checkpoint, int hp, int enemiesKilled, int coinsCollected)
    {
        this.checkpoint = checkpoint;
        this.hp = hp;
        this.enemiesKilled = enemiesKilled;
        this.coinsCollected = coinsCollected;
    }
    #endregion
}
