using UnityEngine;
using UnityEditor;
using System.IO;

public class ModelAnimationExtractorWindow : EditorWindow
{
    private Object modelFile; // 选中的模型（FBX）

    [MenuItem("Tools/模型动画提取器")]
    public static void ShowWindow()
    {
        GetWindow<ModelAnimationExtractorWindow>("模型动画提取器");
    }

    void OnGUI()
    {
        GUILayout.Label("选择一个模型文件（如 .fbx）", EditorStyles.boldLabel);
        modelFile = EditorGUILayout.ObjectField("模型文件", modelFile, typeof(Object), false);

        if (modelFile != null)
        {
            if (GUILayout.Button("提取动画"))
            {
                ExtractAnimationClips();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("请先选择一个 FBX 模型文件", MessageType.Info);
        }
    }

    void ExtractAnimationClips()
    {
        string assetPath = AssetDatabase.GetAssetPath(modelFile);

        if (!assetPath.ToLower().EndsWith(".fbx"))
        {
            Debug.LogError("请选择一个 .fbx 文件！");
            return;
        }

        string fileName = Path.GetFileNameWithoutExtension(assetPath);
        string directory = Path.GetDirectoryName(assetPath);
        string targetFolder = Path.Combine(directory, fileName + "_Clips");

        // 创建保存动画片段的文件夹
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            AssetDatabase.CreateFolder(directory, fileName + "_Clips");
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        int count = 0;

        foreach (Object obj in assets)
        {
            var clip = obj as AnimationClip;
            if (clip)
            {
                if (clip.name.Contains("__preview__")) continue;

                AnimationClip newClip = new AnimationClip();
                EditorUtility.CopySerialized(clip, newClip);

                string savePath = Path.Combine(targetFolder, clip.name + ".anim");
                AssetDatabase.CreateAsset(newClip, savePath);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(string.Format("成功提取 {0} 个动画片段到：{1}", count, targetFolder));
        EditorUtility.DisplayDialog("完成", string.Format("成功提取 {0} 个动画片段！", count), "好的");
    }
}
