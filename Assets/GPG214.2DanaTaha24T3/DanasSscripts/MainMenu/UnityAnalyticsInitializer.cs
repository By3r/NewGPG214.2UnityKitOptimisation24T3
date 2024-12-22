using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Initializes Unity analytics to track what kind of device model, platform and type the player uses.
    /// </summary>
   
    public class UnityAnalyticsInitializer : MonoBehaviour
    {
        private IEnumerator Start()
        { 
            yield return UnityServices.InitializeAsync();

            AnalyticsService.Instance.StartDataCollection();

            CustomEvent onUnityServiceInitialisation = new CustomEvent("UnityIntialisedIformation")
            {
                {"DeviceModel", SystemInfo.deviceModel },
                {"DevicePlatform", Application.platform.ToString() } ,
                {"DeviceType", SystemInfo.deviceType.ToString()}
            };

            AnalyticsService.Instance.RecordEvent(onUnityServiceInitialisation);

            yield return null;
        }
    }
}
