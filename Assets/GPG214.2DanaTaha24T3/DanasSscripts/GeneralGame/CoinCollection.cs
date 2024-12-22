using System.Collections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Simple coin collection script. Every time the Player collides with the coin GameObject, it records an analytics event.
    /// </summary>
  
    public class CoinCollection : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int coinValue = 1; 
        #endregion

        private IEnumerator Start()
        {
            yield return UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (PlayerDataManager.instance != null)
                {
                    PlayerDataManager.instance.AddCoins(coinValue);
                }

                CustomEvent coinCollectionEvent = new CustomEvent("CoinCollectionEvent")
                {
                    { "coinValue", coinValue },
                    { "coinPositionX", transform.position.x },
                    { "coinPositionY", transform.position.y },
                    { "coinPositionZ", transform.position.z },
                };

                AnalyticsService.Instance.RecordEvent(coinCollectionEvent);
                Destroy(gameObject);
            }
        }
    }
}
