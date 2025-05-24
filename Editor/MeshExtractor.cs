using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshExtractor : MonoBehaviour
{
    [MenuItem("WP/提取选中模型的Mesh")]
    static void ExtractMesh()
    {
        Object obj = Selection.activeObject;
        GameObject go = obj as GameObject;

        if (go != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            // 尝试获取模型中的 Mesh
            MeshFilter meshFilter = go.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("未找到 MeshFilter 或 Mesh！");
                return;
            }

            Mesh sourceMesh = meshFilter.sharedMesh;

            // 拷贝 mesh 内容
            Mesh copiedMesh = new Mesh
            {
                vertices = sourceMesh.vertices,
                triangles = sourceMesh.triangles,
                normals = sourceMesh.normals,
                uv = sourceMesh.uv,
                tangents = sourceMesh.tangents,
                colors = sourceMesh.colors,
                name = sourceMesh.name + "_Copy"
            };

            copiedMesh.RecalculateBounds();

            // 生成新 asset 文件
            string folderPath = Path.GetDirectoryName(path);
            string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + copiedMesh.name + ".asset");

            AssetDatabase.CreateAsset(copiedMesh, newAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log("成功创建新的 Mesh Asset: " + newAssetPath);
        }
        else
        {
            Debug.LogError("请选中一个包含 Mesh 的模型预制体！");
        }
    }
}
