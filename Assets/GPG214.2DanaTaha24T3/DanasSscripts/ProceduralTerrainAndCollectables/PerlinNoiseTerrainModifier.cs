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
        [SerializeField] private int terrainWidth;
        [SerializeField] private int terrainHeight;
        [SerializeField] private int terrainDepth;
        [SerializeField] private float scale;
        [SerializeField] private float bumpHeight;
        [SerializeField] private Vector2 offset;
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
