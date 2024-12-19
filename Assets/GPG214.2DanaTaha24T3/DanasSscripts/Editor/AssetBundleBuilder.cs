using UnityEditor;
using UnityEngine;
using System.IO;

namespace Dana
{
    /// <summary>
    /// Responsible for building all asset bundles in the project. Modify the output Directory to ideally match the directory used for firebase hosting. In the case below, 
    /// the AssetBundles folder is being used for firebase hosting. If you keep the script the same, it will create the 'AssetBundles' folder to place asset bundles within.
    /// </summary>

    public class AssetBundleBuilder
    {
        [MenuItem("Assets/Build Asset Bundles")]
        public static void BuildAllAssetBundles()
        {
            string outputDirectory = "Assets/AssetBundles";

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            BuildPipeline.BuildAssetBundles(outputDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            Debug.Log($"Asset Bundles built successfully and stored in: {outputDirectory}");
        }
    }
}