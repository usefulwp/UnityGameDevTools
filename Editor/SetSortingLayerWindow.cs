using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SetSortingLayerWindow : EditorWindow
{
    private List<GameObject> selectedObjects = new List<GameObject>();
    private int selectedSortingLayerIndex = 0;
    private int sortOrderOffset = 0;
    private string[] sortingLayerNames;
    private struct SortingInfo
    {
        public string layerName;
        public int order;

        public SortingInfo(string layer, int ord)
        {
            layerName = layer;
            order = ord;
        }
    }
    // 保存修改前的状态（用于撤回）
    private Dictionary<Component, SortingInfo> previousStates = new Dictionary<Component, SortingInfo>();

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

        GUILayout.Label("Drag & Drop Scene Objects Below:", EditorStyles.miniBoldLabel);
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            selectedObjects[i] = (GameObject)EditorGUILayout.ObjectField(selectedObjects[i], typeof(GameObject), true);
        }

        if (GUILayout.Button("Add Object"))
        {
            selectedObjects.Add(null);
        }

        if (GUILayout.Button("Clear Selection"))
        {
            selectedObjects.Clear();
        }

        if (sortingLayerNames.Length > 0)
        {
            selectedSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", selectedSortingLayerIndex, sortingLayerNames);
        }

        sortOrderOffset = EditorGUILayout.IntField("Sorting Order Offset", sortOrderOffset);

        if (GUILayout.Button("Set Layer for Selected"))
        {
            if (selectedObjects.Count > 0)
            {
                previousStates.Clear(); // 清除旧的记录
                string selectedLayer = sortingLayerNames[selectedSortingLayerIndex];
                foreach (var obj in selectedObjects)
                {
                    if (obj != null)
                    {
                        ApplySortingLayer(obj, selectedLayer, sortOrderOffset);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObjects selected!");
            }
        }

        // 添加 Reset 按钮
        if (GUILayout.Button("Reset Last Change"))
        {
            if (previousStates.Count == 0)
            {
                Debug.LogWarning("No previous change to reset.");
            }
            else
            {
                foreach (var kvp in previousStates)
                {
                 
                    if (kvp.Key is Canvas )
                    {
                        Canvas canvas = kvp.Key as Canvas;
                        canvas.sortingLayerName = kvp.Value.layerName;
                        canvas.sortingOrder = kvp.Value.order;
                    }
                    else if (kvp.Key is ParticleSystemRenderer )
                    {
                        ParticleSystemRenderer psr= (ParticleSystemRenderer)kvp.Key;
                        psr.sortingLayerName = kvp.Value.layerName;
                        psr.sortingOrder = kvp.Value.order;
                    }
                }

                Debug.Log("Reverted last sorting layer change.");
                previousStates.Clear();
            }
        }
    }

    private void LoadSortingLayers()
    {
        sortingLayerNames = SortingLayer.layers.Select(layer => layer.name).ToArray();
    }

    private void ApplySortingLayer(GameObject obj, string layerName, int orderOffset)
    {
        var canvasArr = obj.GetComponentsInChildren<Canvas>(true);
        foreach (var canvas in canvasArr)
        {
            previousStates[canvas] = new SortingInfo(canvas.sortingLayerName, canvas.sortingOrder); // 保存原状态
            canvas.sortingLayerName = layerName;
            canvas.sortingOrder += orderOffset;
        }

        if (canvasArr.Length > 0)
        {
            Debug.LogFormat("Set {0} Canvas in {1} to Sorting Layer: {2} (+Order {3})", canvasArr.Length, obj.name, layerName, orderOffset);
        }
        else
        {
            Debug.LogWarningFormat("No Canvas found in {0}", obj.name);
        }

        var renderers = obj.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var renderer in renderers)
        {
            previousStates[renderer] = new SortingInfo(renderer.sortingLayerName, renderer.sortingOrder); // 保存原状态
            renderer.sortingLayerName = layerName;
            renderer.sortingOrder += orderOffset;
        }

        if (renderers.Length > 0)
        {
            Debug.LogFormat("Set {0} Particle Systems in {1} to Sorting Layer: {2} (+Order {3})", renderers.Length, obj.name, layerName, orderOffset);
        }
        else
        {
            Debug.LogWarningFormat("No Particle System found in {0}", obj.name);
        }
    }
}
