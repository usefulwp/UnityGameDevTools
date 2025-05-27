using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class BundleDependencyCheckerWindow : EditorWindow
{
    private string bundleAName = "";
    private string bundleBName = "";

    [MenuItem("WP/检查AB依赖资源")]
    public static void ShowWindow()
    {
        GetWindow<BundleDependencyCheckerWindow>("AB依赖检查");
    }

    private void OnGUI()
    {
        GUILayout.Label("检查 AssetBundle A 是否依赖 B 的资源", EditorStyles.boldLabel);

        bundleAName = EditorGUILayout.TextField("AssetBundle A 名称", bundleAName);
        bundleBName = EditorGUILayout.TextField("AssetBundle B 名称", bundleBName);

        if (GUILayout.Button("检查依赖"))
        {
            if (string.IsNullOrEmpty(bundleAName) || string.IsNullOrEmpty(bundleBName))
            {
                Debug.LogWarning("请输入有效的 AssetBundle 名称");
                return;
            }

            CheckDependencies(bundleAName, bundleBName);
        }
    }

    private void CheckDependencies(string bundleAPath, string bundleBPath)
    {
        string[] assetsA = AssetDatabase.GetAssetPathsFromAssetBundle(bundleAPath);
        string[] assetsB = AssetDatabase.GetAssetPathsFromAssetBundle(bundleBPath);

        if (assetsA == null || assetsA.Length == 0)
        {
            Debug.LogWarningFormat("AssetBundle A ({0}) 中没有找到资源。", bundleAPath);
            return;
        }

        if (assetsB == null || assetsB.Length == 0)
        {
            Debug.LogWarningFormat("AssetBundle B ({0}) 中没有找到资源。", bundleBPath);
            return;
        }

        HashSet<string> bAssets = new HashSet<string>(assetsB);
        HashSet<string> usedAssetsFromB = new HashSet<string>();

        foreach (var asset in assetsA)
        {
            string[] dependencies = AssetDatabase.GetDependencies(asset, true);
            foreach (var dep in dependencies)
            {
                if (bAssets.Contains(dep))
                {
                    usedAssetsFromB.Add(dep);
                }
            }
        }

        if (usedAssetsFromB.Count == 0)
        {
            Debug.LogFormat("✅ AssetBundle '{0}' 不依赖 AssetBundle '{1}' 中的任何资源。", bundleAPath, bundleBPath);
        }
        else
        {
            Debug.LogFormat("❗AssetBundle '{0}' 依赖 AssetBundle '{1}' 中的以下资源：", bundleAPath, bundleBPath);
            foreach (var asset in usedAssetsFromB)
            {
                Debug.Log("    - " + asset);
            }
        }
    }
}
