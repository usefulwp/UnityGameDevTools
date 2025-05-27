using UnityEditor;
using UnityEngine;
using System.IO;

public class SetABNameWindow : EditorWindow
{
    private string abName = "";
    private string folderPath = "";

    [MenuItem("WP/设置所选文件夹的 AssetBundle 名（递归）")]
    public static void ShowWindow()
    {
        SetABNameWindow window = GetWindow<SetABNameWindow>("设置 AssetBundle 名");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 120);
        window.UpdateSelectedFolder(); // 打开窗口时设置一次
        Selection.selectionChanged += window.OnSelectionChanged; // 监听选中变化
        window.Show();
    }

    private void OnDestroy()
    {
        Selection.selectionChanged -= OnSelectionChanged; // 窗口关闭时移除监听
    }

    private void OnSelectionChanged()
    {
        UpdateSelectedFolder();
        Repaint(); // 更新 UI
    }

    private void UpdateSelectedFolder()
    {
        Object obj = Selection.activeObject;
        string selectedPath = AssetDatabase.GetAssetPath(obj);

        if (!string.IsNullOrEmpty(selectedPath) && AssetDatabase.IsValidFolder(selectedPath))
        {
            folderPath = selectedPath;
        }
        else
        {
            folderPath = "";
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("当前选中文件夹：", string.IsNullOrEmpty(folderPath) ? "无效或未选择文件夹" : folderPath);
        abName = EditorGUILayout.TextField("AssetBundle 名：", abName);

        GUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(folderPath));
        if (GUILayout.Button("开始设置"))
        {
            if (string.IsNullOrEmpty(abName))
            {
                EditorUtility.DisplayDialog("错误", "请输入有效的 AssetBundle 名！", "确定");
                return;
            }

            int count = SetAssetBundleNames(folderPath, abName);
            EditorUtility.DisplayDialog("完成", string.Format("✅ 共为 {0} 个资源设置 AB 名：{1}", count, abName), "好的");
            Close();
        }

        if (GUILayout.Button("重置所有 AssetBundle 名"))
        {
            int count = SetAssetBundleNames(folderPath, null);
            EditorUtility.DisplayDialog("完成", string.Format("共清空了 {0} 个资源的 AssetBundle 名", count), "好的");
        }
        EditorGUI.EndDisabledGroup();
    }

    private int SetAssetBundleNames(string folderPath, string abName)
    {
        int count = 0;
        string fullPath = Path.Combine(Application.dataPath, folderPath.Substring("Assets/".Length));
        string[] files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            if (file.EndsWith(".meta")) continue;

            string assetPath = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                if (abName != null || !string.IsNullOrEmpty(importer.assetBundleName))
                {
                    importer.assetBundleName = abName;
                    count++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return count;
    }
}
