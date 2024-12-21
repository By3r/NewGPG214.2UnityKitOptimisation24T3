using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Dana
{
    public class AssetBundleLoaderAndDownloader : MonoBehaviour
    {
        #region Variables
        [Header("Asset Bundle Info")]
        [Tooltip("Place the google drive download link for the asset bundle.")]
        [SerializeField] private string assetBundleUrl = "https://drive.google.com/uc?id=1654VnkT7Php08TF4HMzdNQgV6otI7iP3";

        [Tooltip("Write the name of the asset bundle file.")]
        [SerializeField] private string bundleName = "basiclevelassets_19.12.24";

        [Header("Script references")]
        [Tooltip("Reference to the prefab spawner based on texture read script.")]
        [SerializeField] private PrefabSpawnerBasedOnTextureRead prefabSpawnerScript;

        private string _localBundlePath;
        #endregion

        private void Start()
        {
            SetLocalPathForAssetBundle(Application.persistentDataPath, bundleName);
            StartCoroutine(DownloadAndLoadAssetBundle());
        }

        #region Private Functions
        /// <summary>
        /// Download the asset bundle and load it.
        /// </summary>
        private IEnumerator DownloadAndLoadAssetBundle()
        {
            Debug.Log($"Downloading the asset bundle from {assetBundleUrl}");

            UnityWebRequest bundleRequest = UnityWebRequest.Get(assetBundleUrl);
            bundleRequest.downloadHandler = new DownloadHandlerFile(_localBundlePath);

            yield return bundleRequest.SendWebRequest();

            if (bundleRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download asset bundle: {bundleRequest.error}");
                yield break;
            }

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
                    Debug.LogError("Make sure the script is assigned!!");
                }
            }
            else
            {
                Debug.LogError("Could not load asset bundle...");
            }
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
}
