using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dana
{
    public class PrefabSpawnerBaseOnTextureReading : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Texture2D texture;
        [SerializeField] private List<Color> targetColors;
        [SerializeField] private List<GameObject> prefabs;
        [SerializeField] private float spacing = 1f;
        [SerializeField] private LayerMask terrainLayer;
        [SerializeField] private float heightOffset = 0.5f;
        #endregion

        #region Public Functions
        /// <summary>
        /// Instantiates prefabs according to the texture's width and height if the texture's
        /// pixels match the target colour. The prefabs get instantiated in the position of the matched colours
        /// and the function is mindful for the position spacing between each prefab.
        /// </summary>
        public void GeneratePrefabsFromTexture()
        {
            if (texture == null)
            {
                return;
            }

            if (targetColors.Count != prefabs.Count || targetColors.Count == 0)
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

                            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer))
                            {
                                Vector3 spawnPosition = hit.point + hit.normal * heightOffset;
                                Instantiate(prefabs[i], spawnPosition, Quaternion.identity, transform);
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
        #endregion
    }
}
