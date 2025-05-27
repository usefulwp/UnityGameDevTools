using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

public class AssetBundleDependencyChecker : EditorWindow
{
    private Dictionary<string, List<string>> dependencyGraph = new Dictionary<string, List<string>>();
    private HashSet<string> visited = new HashSet<string>();
    private HashSet<string> recursionStack = new HashSet<string>();
    private List<string> cycleDetected = new List<string>();

    private bool isLogAssetBundleInfo = false; // 是否打印资源包的依赖关系

    private string manifestFilePath = "Assets/AssetBundles/AssetBundles.manifest";  // 默认 manifest 文件路径

    [MenuItem("WP/资源包循环依赖检查")]
    public static void ShowWindow()
    {
        GetWindow<AssetBundleDependencyChecker>("AssetBundle Dependency Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("AssetBundle Dependency Checker", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 路径选择按钮
        GUILayout.Label("Manifest File Path:");
        manifestFilePath = EditorGUILayout.TextField(manifestFilePath);

        if (GUILayout.Button("Browse for Manifest"))
        {
            string path = EditorUtility.OpenFilePanel("Select Manifest File", manifestFilePath, "manifest");
            if (!string.IsNullOrEmpty(path))
            {
                manifestFilePath = path;
            }
        }
        GUILayout.Space(10);

        isLogAssetBundleInfo= EditorGUILayout.Toggle("是否打印资源包的依赖关系", isLogAssetBundleInfo);

        GUILayout.Space(10);

        if (GUILayout.Button("Check for Circular Dependencies"))
        {
            CheckForCircularDependencies();
        }

        if (cycleDetected.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Circular Dependencies Detected:", EditorStyles.boldLabel);
            foreach (var cycle in cycleDetected)
            {
                GUILayout.Label(cycle);
            }
        }
    }

    private void CheckForCircularDependencies()
    {
        dependencyGraph.Clear();
        visited.Clear();
        recursionStack.Clear();
        cycleDetected.Clear();

        if (!File.Exists(manifestFilePath))
        {
            Debug.LogError("Manifest file not found at path: " + manifestFilePath);
            return;
        }

        string[] manifestLines = File.ReadAllLines(manifestFilePath);
        string currentBundleName = null;
        bool inDependencies = false;

        // 新的正则表达式匹配 YAML 格式
        Regex infoRegex = new Regex(@"^\s*Info_\d+:");
        Regex nameRegex = new Regex(@"^\s*Name:\s*(.+)");
        Regex dependenciesStartRegex = new Regex(@"^\s*Dependencies:");

        foreach (var line in manifestLines)
        {
            // 检查是否是新的 Info 块
            if (infoRegex.IsMatch(line))
            {
                currentBundleName = null;
                inDependencies = false;
                continue;
            }

            // 检查是否是 Name 行
            Match nameMatch = nameRegex.Match(line);
            if (nameMatch.Success)
            {
                currentBundleName = nameMatch.Groups[1].Value.Trim();
                if (!dependencyGraph.ContainsKey(currentBundleName))
                {
                    dependencyGraph[currentBundleName] = new List<string>();
                }
                continue;
            }

            // 检查是否进入 Dependencies 部分
            if (dependenciesStartRegex.IsMatch(line))
            {
                inDependencies = true;
                continue;
            }

            // 如果在 Dependencies 部分且不是空依赖
            if (inDependencies && currentBundleName != null )
            {
                // 解析依赖项
                Match depMatch = Regex.Match(line.Trim(), @"Dependency_\d+: (.+)");
                if (depMatch.Success)
                {
                    string dependency = depMatch.Groups[1].Value.Trim();
                    dependencyGraph[currentBundleName].Add(dependency);
                }
            }
        }

        foreach (var item in dependencyGraph)
        {
            if (item.Value.Count > 0) {
                StringBuilder sb = new StringBuilder();
                foreach (var dep in item.Value)
                {
                    sb.Append(dep + "\n");
                }
                if (isLogAssetBundleInfo) {
                    Debug.LogFormat("资源包: {0} ,依赖项: {1}", item.Key, sb.ToString());
                }
            }
        }

        // 检测循环依赖
        foreach (var bundle in dependencyGraph.Keys)
        {
            if (!visited.Contains(bundle))
            {
                recursionStack.Clear(); // 重要：每次开始新的检查时清空递归栈
                if (DetectCycle(bundle))
                {
                    Debug.LogErrorFormat("Found circular dependency starting from: {0}", bundle);
                    Debug.LogErrorFormat("{0}", string.Join(" -> ", cycleDetected.ToArray()));
                }
            }
        }

        if (cycleDetected.Count == 0)
        {
            Debug.Log("No circular dependencies detected.");
        }
    }

    private bool DetectCycle(string bundleName)
    {
        if (recursionStack.Contains(bundleName))
        {
            // 发现循环依赖，构建循环路径
            List<string> cyclePath = new List<string>(recursionStack);
            cyclePath.Add(bundleName);
            cycleDetected.Add("Circular dependency: " + string.Join(" -> ", cyclePath.ToArray()));
            return true;
        }

        if (visited.Contains(bundleName))
        {
            return false;
        }

        visited.Add(bundleName);
        recursionStack.Add(bundleName);

        if (dependencyGraph.ContainsKey(bundleName))
        {
            foreach (var dependency in dependencyGraph[bundleName])
            {
                if (DetectCycle(dependency))
                {
                    return true;
                }
            }
        }

        recursionStack.Remove(bundleName);
        return false;
    }
}
