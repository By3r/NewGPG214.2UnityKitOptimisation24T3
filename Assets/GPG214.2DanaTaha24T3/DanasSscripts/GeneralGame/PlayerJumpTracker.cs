using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Records the number of times the player jumps (by pressing space bar) and records it to analytics event.
    /// </summary>
  
    public class PlayerJumpTracker : MonoBehaviour
    {
        #region Variables
        private int _jumpCount = 0; 
        #endregion

        private IEnumerator Start()
        {
            yield return UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jumpCount++;

                RecordJumpEvent();
            }
        }

        private void RecordJumpEvent()
        {
            CustomEvent jumpEvent = new CustomEvent("PlayerJumpEvent")
            {
                { "jumpCount", _jumpCount },
            };

            AnalyticsService.Instance.RecordEvent(jumpEvent);
        }
    }
}
