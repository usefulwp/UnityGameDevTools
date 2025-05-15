using UnityEditor;
using UnityEngine;

public class RemoveAllMissingScripts
{
    [MenuItem("WP/移除场景所有丢失的脚本")]
    static void RemoveAllMissingScriptsInScene()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>(); // 这样可以找到未激活的物体
        int removedCount = 0;

        foreach (GameObject go in allObjects)
        {
            if (go.hideFlags == HideFlags.None) // 过滤掉Unity内部的隐藏对象，避免影响其他系统
            {
                #if UNITY_2018_3_OR_NEWER
                         int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                #else
                         int count = MissingScriptRemover2017.RemoveMissingScriptsRecursively(go);
                #endif
                removedCount += count;
            }
        }

        Debug.LogFormat("Removed {0} missing scripts from the scene (including inactive objects).", removedCount);
    }
}
