using UnityEditor;
using UnityEngine;

public static class MissingScriptRemover2017
{
    public static int RemoveMissingScriptsRecursively(GameObject go)
    {
        int count = 0;

        // 删除当前 GameObject 上的 missing scripts
        count += RemoveMissingScriptsFromGameObject(go);

        // 递归处理所有子对象
        foreach (Transform child in go.transform)
        {
            count += RemoveMissingScriptsRecursively(child.gameObject);
        }

        return count;
    }

    public static int RemoveMissingScriptsFromGameObject(GameObject go)
    {
        SerializedObject so = new SerializedObject(go);
        SerializedProperty prop = so.FindProperty("m_Component");
        int removedCount = 0;

        if (prop != null && prop.isArray)
        {
            for (int i = prop.arraySize - 1; i >= 0; i--)
            {
                var element = prop.GetArrayElementAtIndex(i);
                var component = element.objectReferenceValue;

                if (component == null)
                {
                    prop.DeleteArrayElementAtIndex(i);
                    removedCount++;
                }
            }

            if (removedCount > 0)
                so.ApplyModifiedProperties();
        }

        return removedCount;
    }
}
