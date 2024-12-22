using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Dana
{
    public class AssetBundleLoaderAndDownloader : MonoBehaviour
    {
        #region Variables
        [Header("JSON URL")]
        [Tooltip("URL of the JSON file containing asset bundle data.")]
        [SerializeField] private string jsonFileURL = "https://drive.google.com/uc?id=1wi--oQIJSNw4uETU6V7F8w8qML72dyxK";

        [Header("Asset Bundle Info")]
        [Tooltip("Write the name of a base asset bundle file, the name will change eventually through the JSON file.")]
        [SerializeField] private string bundleName = "basiclevelassets_19.12.24.1";

        [Header("Script References")]
        [Tooltip("Reference to the prefab spawner based on texture read script.")]
        [SerializeField] private PrefabSpawnerBasedOnTextureRead prefabSpawnerScript;

        private string _localBundlePath;
        private AssetBundleJSONData _jsonData;
        #endregion

        private void Start()
        {
            SetLocalPathForAssetBundle(Application.persistentDataPath, bundleName);
            StartCoroutine(CheckAndDownloadBundle());
        }

        #region Private Functions
        /// <summary>
        /// Fetch metadata, compare versions, and download the new bundle if necessary.
        /// </summary>
        private IEnumerator CheckAndDownloadBundle()
        {
            UnityWebRequest metadataRequest = UnityWebRequest.Get(jsonFileURL);
            yield return metadataRequest.SendWebRequest();

            if (metadataRequest.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }

            string json = metadataRequest.downloadHandler.text;
            _jsonData = JsonUtility.FromJson<AssetBundleJSONData>(json);

            if (_jsonData == null)
            {
                yield break;
            }

            if (_jsonData.newBundle.name != bundleName)
            {
                bundleName = _jsonData.newBundle.name;
                SetLocalPathForAssetBundle(Application.persistentDataPath, bundleName);

                yield return DownloadAndLoadAssetBundle(_jsonData.newBundle.url);
            }
            else
            {
                yield return LoadBundleFromDownloadedFile();
            }
        }

        /// <summary>
        /// Download the asset bundle and load it.
        /// </summary>
        private IEnumerator DownloadAndLoadAssetBundle(string url)
        {

            UnityWebRequest bundleRequest = UnityWebRequest.Get(url);
            bundleRequest.downloadHandler = new DownloadHandlerFile(_localBundlePath);

            yield return bundleRequest.SendWebRequest();

            if (bundleRequest.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }

            yield return LoadBundleFromDownloadedFile();
        }

        /// <summary>
        /// Loads the asset bundle from the local file.
        /// </summary>
        private IEnumerator LoadBundleFromDownloadedFile()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(_localBundlePath);
            if (bundle != null)
            {
                if (prefabSpawnerScript != null)
                {
                    prefabSpawnerScript.LoadPrefabsFromAssetBundle(bundle);
                    prefabSpawnerScript.GeneratePrefabsFromTexture();
                }
                else
                {
                    //   Debug.LogError("Prefab spawner script is not assigned.");
                }
            }
            else
            {
                //    Debug.LogError("Failed to load the asset bundle. The file may be corrupted.");
            }

            yield return null;
        }

        /// <summary>
        /// Set the local file path for the asset bundle.
        /// </summary>
        /// <param name="path"> Folder path </param>
        /// <param name="nameOfBundle"> Name of bundle file </param>
        private void SetLocalPathForAssetBundle(string path, string nameOfBundle)
        {
            _localBundlePath = Path.Combine(path, "AssetBundles", nameOfBundle);
        }
        #endregion
    }

    #region Json for parsing
    /// <summary>
    /// Json struct for JSON parsing.
    /// </summary>
    [System.Serializable]
    public class AssetBundleJSONData
    {
        public BundleInfo currentBundle;
        public BundleInfo newBundle;
    }

    [System.Serializable]
    public class BundleInfo
    {
        public string name;
        public string url;
    }
    #endregion
}
