
using UnityEditor;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Created a button in the inspector to generate terrains on demand.
    /// </summary>
    [CustomEditor(typeof(PerlinNoiseTerrainModifier))]
    public class PerlinNoiseTerrainEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PerlinNoiseTerrainModifier perlinNoiseTarget = (PerlinNoiseTerrainModifier)target;

            if (GUILayout.Button("Generate Terrain"))
            {
                perlinNoiseTarget.GenerateTerrain();
            }
        }
    }
}
