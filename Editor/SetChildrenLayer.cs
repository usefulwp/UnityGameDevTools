using UnityEngine;
using UnityEditor;

public class SetChildrenLayer : EditorWindow
{
    private GameObject targetObject;
    private int selectedLayer = 0;

    [MenuItem("WP/设置游戏物体的层级")]
    public static void ShowWindow()
    {
        GetWindow<SetChildrenLayer>("Set Children Layer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select GameObject", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        GUILayout.Label("Select Layer", EditorStyles.boldLabel);
        selectedLayer = EditorGUILayout.LayerField("Layer", selectedLayer);

        if (GUILayout.Button("Set Layer for Children"))
        {
            if (targetObject != null)
            {
                SetLayerRecursively(targetObject, selectedLayer);
                Debug.LogFormat("Set layer '{0}' to all children of {1}", LayerMask.LayerToName(selectedLayer), targetObject.name);
            }
            else
            {
                Debug.LogWarning("Please select a GameObject first.");
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
