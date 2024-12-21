using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// This script modifies the terrain the script is attached to using perlin noise. 
    /// </summary>
    
    public class PerlinNoiseTerrainModifier : MonoBehaviour
    {
        #region Variables
        [Header("Terrain Modifier Variables")]
        [Tooltip("This affects the width size of the terrain. Place any number between 0 up to the max " +
            "size set for the terrain. e.g. if the terrain size is 65 x 65 x 65 then the maximum is 65.")]
        [SerializeField] private int terrainWidth;
        [Tooltip("This affects the amount of width surface affected by other terrain settings. Place any " +
            "number between 0 up to the max size set for the terrain. e.g. if the terrain size is 65 x " +
            "65 x 65 then the maximum is 65. Avoid making the number bigger than the amount given to terrainWidth.")]
        [SerializeField] private int terrainHeight;
        [Tooltip("The higher the number the bumpier the surface will be.")]
        [SerializeField] private float scale;
        [Tooltip("The higher the number the taller the bump is.")]
        [SerializeField] private float bumpHeight;
        [Tooltip("Use this to change the noise's position to suit your needs.")]
        [SerializeField] private Vector2 offset;
        [Tooltip("Change the terrain size settings through this variable.")]
        [SerializeField] private Vector3 terrainSize = new Vector3(65, 50, 65);
        #endregion

        void Start()
        {
            GenerateTerrain();
        }

        #region Public Functions.
        public void GenerateTerrain()
        {
            Terrain terrain = GetComponent<Terrain>();
            if (terrain == null)
            {
                return;
            }

            TerrainData terrainData = terrain.terrainData;

            terrainData.size = terrainSize;

            terrainData.heightmapResolution = terrainWidth + 1;

            terrainData.SetHeights(0, 0, GenerateHeights());

            TerrainCollider terrainCollider = GetComponent<TerrainCollider>();
            terrainCollider.terrainData = GetComponent<Terrain>().terrainData;

        }

        #endregion

        #region Private Functions.
        private float[,] GenerateHeights()
        {
            float[,] heights = new float[terrainWidth, terrainHeight];

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    float xCoord = (float)x / (terrainWidth - 1) * scale + offset.x;
                    float yCoord = (float)y / (terrainHeight - 1) * scale + offset.y;

                    heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord) * (bumpHeight / terrainSize.y);
                }
            }

            return heights; 
        }
        #endregion
    }
}
