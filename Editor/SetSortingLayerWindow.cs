using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SetSortingLayerWindow : EditorWindow
{
    private List<GameObject> selectedObjects = new List<GameObject>();
    private int selectedSortingLayerIndex = 0;
    private string[] sortingLayerNames;

    [MenuItem("WP/设置游戏物体的排序层（包括canvas组件和粒子系统组件)")]
    public static void ShowWindow()
    {
        GetWindow<SetSortingLayerWindow>("Set Particle Layer");
    }

    private void OnEnable()
    {
        LoadSortingLayers();
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Set Particle Sorting Layer", EditorStyles.boldLabel);

        // 允许拖拽多个 GameObject 进来
        GUILayout.Label("Drag & Drop Scene Objects Below:", EditorStyles.miniBoldLabel);

        for (int i = 0; i < selectedObjects.Count; i++)
        {
            selectedObjects[i] = (GameObject)EditorGUILayout.ObjectField(selectedObjects[i], typeof(GameObject), true);
        }

        // 添加新物体
        if (GUILayout.Button("Add Object"))
        {
            selectedObjects.Add(null);
        }

        // 清空所有选择
        if (GUILayout.Button("Clear Selection"))
        {
            selectedObjects.Clear();
        }

        // Sorting Layer 下拉菜单
        if (sortingLayerNames.Length > 0)
        {
            selectedSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", selectedSortingLayerIndex, sortingLayerNames);
        }

        // 批量设置 Sorting Layer
        if (GUILayout.Button("Set Layer for Selected"))
        {
            if (selectedObjects.Count > 0)
            {
                string selectedLayer = sortingLayerNames[selectedSortingLayerIndex];
                foreach (var obj in selectedObjects)
                {
                    if (obj != null)
                    {
                        ApplySortingLayer(obj, selectedLayer);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObjects selected!");
            }
        }
    }

    private void LoadSortingLayers()
    {
        sortingLayerNames = SortingLayer.layers.Select(layer => layer.name).ToArray();
    }

    private static void ApplySortingLayer(GameObject obj, string layerName)
    {
        var canvasArr = obj.GetComponentsInChildren<Canvas>(true);

        if (canvasArr.Length == 0)
        {
            Debug.LogWarning($"No Canvas  found in {obj.name}");
        }
        else {
            foreach (var canvas in canvasArr)
            {
                canvas.sortingLayerName = layerName;
            }

            Debug.Log($"Set {canvasArr.Length} Canvas in {obj.name} to Sorting Layer: {layerName}");
        }

  


        var renderers = obj.GetComponentsInChildren<ParticleSystemRenderer>(true);

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"No Particle System found in {obj.name}");
        }
        else {
            foreach (var renderer in renderers)
            {
                renderer.sortingLayerName = layerName;
            }

            Debug.Log($"Set {renderers.Length} Particle Systems in {obj.name} to Sorting Layer: {layerName}");
        }


    }
}
