using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportAssetBundles
{
    [MenuItem("Assets/Build AssetBundle")]
    static void ExportResource()
    {
        string folderName = "AssetBundles";
        string filePath = Path.Combine(Application.streamingAssetsPath, folderName);

#if UNITY_WSA_10_0
        BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
        Debug.Log("Asset Bundles built for WSAPlayer");
#elif UNITY_XBOXONE
        BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.XboxOne);
        Debug.Log("Asset Bundles built for XboxOne");
#elif UNITY_STANDALONE_WIN
        BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        Debug.Log("Asset Bundles built for StandaloneWindows64");
#endif
    }
}
