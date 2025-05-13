using UnityEngine;
using UnityEditor;

public class PrintSelectedPath : MonoBehaviour
{
    [MenuItem("工具/打印选中物体路径")] // 快捷键 Ctrl/Cmd + Shift + P
    static void PrintSelectedObjectPath()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("没有选中任何物体！");
            return;
        }

        string path = GetFullPath(selected.transform);
        Debug.LogFormat("选中物体路径：{0}", path);
    }

    static string GetFullPath(Transform current)
    {
        string path = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        return path;
    }
}
