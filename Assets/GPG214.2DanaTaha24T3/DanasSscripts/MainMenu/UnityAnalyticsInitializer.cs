using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Dana
{
    public class UnityAnalyticsInitializer : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return UnityServices.InitializeAsync();

            AnalyticsService.Instance.StartDataCollection();

            yield return null;
        }
    }
}
