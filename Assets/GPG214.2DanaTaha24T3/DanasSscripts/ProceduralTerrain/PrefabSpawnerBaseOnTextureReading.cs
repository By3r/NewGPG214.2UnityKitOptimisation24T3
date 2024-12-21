using System.Collections.Generic;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Spawns prefabs based on read texture pixel data. The prefabs are taken from and asset bundle and are specified
    /// through the inspector on which assets are needed from the bundle at what position (colour position).
    /// </summary>

    public class PrefabSpawnerBasedOnTextureRead : MonoBehaviour
    {
        #region Variables
        [Tooltip("Write the names of assets you want to spawn and have in the asset bundle referenced/ used in AssetBundleLoaderAndDownloader " +
            "scriptinto the empty slots. Make sure that the amount of names in the list match the amount of corresponding colours. ")]
        [SerializeField] private List<string> desiredPrefabNames = new List<string>();
        [Tooltip("Place a clean pixel texture that contains colours for different asset placement in the scene. Make sure your texture is set to read/write enabled")]
        [SerializeField] private Texture2D texture;
        [Tooltip("Place all the colours used in the texture into the list for the script to recognise which pixels to read and compare.")]
        [SerializeField] private List<Color> targetColors;
        [Tooltip("Spacing between your spawned prefabs. The higher the number the bigger the space is.")]
        [SerializeField] private float spacing = 1f;
        [Tooltip("Set the layer mask to macth the layer the terrain is on")]
        [SerializeField] private LayerMask terrainLayermask;
        [Tooltip("The higher the number, the more the spawned prefabs will float over the terrain's surface.")]
        [SerializeField] private float heightOffset = 0.5f;

        private List<GameObject> _prefabs = new List<GameObject>();
        private AssetBundle _assetBundle;
        #endregion

        #region Public Functions

        /// <summary>
        /// Assigns the Asset Bundle and extracts prefabs for spawning based on the desired prefab names.
        /// </summary>
        public void LoadPrefabsFromAssetBundle(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;

            string[] assetNames = _assetBundle.GetAllAssetNames();
            foreach (string assetName in assetNames)
            {
                Debug.Log($"Asset found in bundle: {assetName}");
            }

            _prefabs.Clear();

            foreach (string desiredName in desiredPrefabNames)
            {
                string matchingAssetPath = FindAssetInBundle(assetNames, desiredName);

                if (!string.IsNullOrEmpty(matchingAssetPath))
                {
                    Debug.Log($"Attempting to load prefab: {desiredName}");
                    GameObject prefab = _assetBundle.LoadAsset<GameObject>(matchingAssetPath);
                    if (prefab != null)
                    {
                        _prefabs.Add(prefab);
                        Debug.Log($"Successfully loaded prefab: {prefab.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to load prefab: {desiredName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Prefab with name '{desiredName}' not found in Asset Bundle.");
                }
            }

            Debug.Log($"Total _prefabs successfully loaded: {_prefabs.Count}");
        }

        /// <summary>
        /// Instantiates prefabs according to the texture's width and height if the texture's
        /// pixels match the target colour. The prefabs get instantiated in the position of the matched colours
        /// and the function is mindful for the position spacing between each prefab. The prefabs get instnatiated on
        /// top of the terrain's surface.
        /// </summary>
        public void GeneratePrefabsFromTexture()
        {
            if (texture == null)
            {
                return;
            }

            if (targetColors.Count != _prefabs.Count || targetColors.Count == 0)
            {
                return;
            }

            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            Vector3 spawnerPosition = transform.position;

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixelColor = texture.GetPixel(x, y);

                    for (int i = 0; i < targetColors.Count; i++)
                    {
                        if (IsColorMatch(pixelColor, targetColors[i]))
                        {
                            Vector3 position = spawnerPosition + new Vector3(x * spacing, 100f, y * spacing);

                            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayermask))
                            {
                                Vector3 spawnPosition = hit.point + hit.normal * heightOffset;
                                Instantiate(_prefabs[i], spawnPosition, Quaternion.identity, transform);
                            }

                            break;
                        }
                    }
                }
            }

            Debug.Log("Prefab generation complete!");
        }
        #endregion

        #region Private Functions

        /// <summary>
        /// Checks if the texture's pixel matches a target color. For example: The texture contains only red pixels and white pixels.
        /// If the pixel color matches a target color, corresponding prefab logic is triggered.
        /// </summary>
        /// <param name="color1">Place texture pixel in this parameter.</param>
        /// <param name="color2">Place your target color in this parameter.</param>
        private bool IsColorMatch(Color color1, Color color2)
        {
            float tolerance = 0.3f;
            return Mathf.Abs(color1.r - color2.r) < tolerance &&
                   Mathf.Abs(color1.g - color2.g) < tolerance &&
                   Mathf.Abs(color1.b - color2.b) < tolerance;
        }

        /// <summary>
        /// This function searches for the asset using the name you provide it to search for
        /// within the asset bundle. 
        /// </summary>
        /// <param name="assetNames"> The names of the assets to be compared to the desire Name on lower case character bases of better search and comparison.</param>
        /// <param name="desiredName">Write the name of the asset you want to access from the bundle.</param>
        /// <returns></returns>
        private string FindAssetInBundle(string[] assetNames, string desiredName)
        {
            string lowerCaseDesiredName = desiredName.ToLower();
            foreach (string assetName in assetNames)
            {
                if (assetName.ToLower().Contains(lowerCaseDesiredName))
                {
                    return assetName;
                }
            }
            return null;
        }


        #endregion
    }
}
