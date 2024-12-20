using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Simple coin collecton script. Everytime the Player collides with the coin gameobject. 
    /// </summary>
    
    public class CoinCollection : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int coinValue = 1; 
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Coin collected!");
                // Update collected coins amount of the player.
                // Make the player's data update the amount && I'll then test out account switch to see if there is a diffference in stored data.
                // Try to make it trackable through analytics.
                Destroy(gameObject);
            }
        }
    }
}
