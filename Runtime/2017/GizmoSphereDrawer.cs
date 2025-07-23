using UnityEngine;

/// <summary>
/// 在场景视图中绘制一个球体的 Gizmo。
/// </summary>
public class GizmoSphereDrawer : MonoBehaviour
{
    public Color sphereColor = Color.green; // 球颜色
    public float radius = 0.5f;             // 球半径
    public bool wireframe = false;          // 是否是线框球

    private void OnDrawGizmos()
    {
        Gizmos.color = sphereColor;

        if (wireframe)
            Gizmos.DrawWireSphere(transform.position, radius); // 线框球
        else
            Gizmos.DrawSphere(transform.position, radius);      // 实心球
    }
}
