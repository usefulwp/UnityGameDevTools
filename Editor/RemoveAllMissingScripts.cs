using UnityEditor;
using UnityEngine;

public class RemoveAllMissingScripts
{
    [MenuItem("WP/�Ƴ��������ж�ʧ�Ľű�")]
    static void RemoveAllMissingScriptsInScene()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>(); // ���������ҵ�δ���������
        int removedCount = 0;

        foreach (GameObject go in allObjects)
        {
            if (go.hideFlags == HideFlags.None) // ���˵�Unity�ڲ������ض��󣬱���Ӱ������ϵͳ
            {
                int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                removedCount += count;
            }
        }

        Debug.Log($"Removed {removedCount} missing scripts from the scene (including inactive objects).");
    }
}
