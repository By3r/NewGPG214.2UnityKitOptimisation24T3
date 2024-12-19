using UnityEditor;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Creates a button in the inspector which instantiates prefabs into scene withouut having to run the game.
    /// </summary>
    [CustomEditor(typeof(PrefabSpawnerBaseOnTextureReading))]
    public class PrefabSpawnerBasedOnReadTexture : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PrefabSpawnerBaseOnTextureReading textureToPrefab = (PrefabSpawnerBaseOnTextureReading)target;
            if (GUILayout.Button("Generate Prefabs"))
            {
                textureToPrefab.GeneratePrefabsFromTexture();
            }
        }
    }
}
