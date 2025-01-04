using UnityEngine;

public class RaycastGenerator : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Start()
    {
        // 获取当前物体的 BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("没有找到 BoxCollider2D！");
        }
    }

    void Update()
    {
        if (boxCollider != null)
        {
            GenerateRays();
        }
    }

    void GenerateRays()
    {
        // 获取碰撞体的边界信息
        Bounds bounds = boxCollider.bounds;

        // 计算中点和两条射线的起点
        Vector2 center = bounds.center;
        Vector2 leftRayOrigin = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x, bounds.min.y);

        // 射线长度
        float rayLength = bounds.extents.y;

        // 从中点向下绘制射线
        Debug.DrawRay(center, Vector2.down * rayLength, Color.green);

        // 从两条边界向下绘制射线
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        // 检测射线是否碰到物体
        RaycastHit2D centerHit = Physics2D.Raycast(center, Vector2.down, rayLength);
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength);

        // 检测条件
        if (leftHit.collider != null && rightHit.collider != null)
        {
            print("111");
        }
    }
}
