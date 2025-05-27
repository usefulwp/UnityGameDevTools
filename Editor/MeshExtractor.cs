using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshExtractor : MonoBehaviour
{
    [MenuItem("WP/提取选中模型的所有 Mesh（含Skinned）")]
    static void ExtractAllMeshes()
    {
        Object obj = Selection.activeObject;
        GameObject go = obj as GameObject;

        if (go == null)
        {
            Debug.LogError("请选中一个包含 Mesh 的模型预制体！");
            return;
        }

        string path = AssetDatabase.GetAssetPath(obj);
        string folderPath = Path.GetDirectoryName(path);

        int count = 0;

        // 处理 MeshFilter
        MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in meshFilters)
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) continue;
            SaveCopiedMesh(mesh, folderPath, ref count);
        }

        // 处理 SkinnedMeshRenderer
        SkinnedMeshRenderer[] skinnedRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinnedRenderers)
        {
            Mesh mesh = smr.sharedMesh;
            if (mesh == null) continue;
            SaveCopiedMesh(mesh, folderPath, ref count);
        }

        AssetDatabase.SaveAssets();
        Debug.LogFormat("成功创建 {0} 个新的 Mesh Asset 文件。", count);
    }

    static void SaveCopiedMesh(Mesh sourceMesh, string folderPath, ref int count)
    {
        Mesh copiedMesh = new Mesh
        {
            name = sourceMesh.name + "_Copy"
        };

        copiedMesh.indexFormat = sourceMesh.indexFormat;

        copiedMesh.vertices = sourceMesh.vertices;
        copiedMesh.normals = sourceMesh.normals;
        copiedMesh.tangents = sourceMesh.tangents;
        copiedMesh.colors = sourceMesh.colors;
        copiedMesh.colors32 = sourceMesh.colors32;

        copiedMesh.uv = sourceMesh.uv;
        copiedMesh.uv2 = sourceMesh.uv2;
        copiedMesh.uv3 = sourceMesh.uv3;
        copiedMesh.uv4 = sourceMesh.uv4;
#if UNITY_2018_2_OR_NEWER
    copiedMesh.uv5 = sourceMesh.uv5;
    copiedMesh.uv6 = sourceMesh.uv6;
    copiedMesh.uv7 = sourceMesh.uv7;
    copiedMesh.uv8 = sourceMesh.uv8;
#endif

        copiedMesh.bindposes = sourceMesh.bindposes;
        copiedMesh.boneWeights = sourceMesh.boneWeights;

        copiedMesh.subMeshCount = sourceMesh.subMeshCount;
        for (int i = 0; i < sourceMesh.subMeshCount; i++)
        {
            // 获取每个 subMesh 的三角形索引
            int[] triangles = sourceMesh.GetTriangles(i);
            copiedMesh.SetTriangles(triangles, i);
        }

        // 复制 blend shapes
        for (int shapeIndex = 0; shapeIndex < sourceMesh.blendShapeCount; shapeIndex++)
        {
            string shapeName = sourceMesh.GetBlendShapeName(shapeIndex);
            int frameCount = sourceMesh.GetBlendShapeFrameCount(shapeIndex);
            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                float frameWeight = sourceMesh.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
                Vector3[] deltaVertices = new Vector3[sourceMesh.vertexCount];
                Vector3[] deltaNormals = new Vector3[sourceMesh.vertexCount];
                Vector3[] deltaTangents = new Vector3[sourceMesh.vertexCount];
                sourceMesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                copiedMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
            }
        }

        copiedMesh.RecalculateBounds();

        string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, copiedMesh.name + ".asset")
        );

        AssetDatabase.CreateAsset(copiedMesh, newAssetPath);
        count++;
    }

}
