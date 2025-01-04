using UnityEngine;

public class RaycastGenerator : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Start()
    {
        // ��ȡ��ǰ����� BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("û���ҵ� BoxCollider2D��");
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
        // ��ȡ��ײ��ı߽���Ϣ
        Bounds bounds = boxCollider.bounds;

        // �����е���������ߵ����
        Vector2 center = bounds.center;
        Vector2 leftRayOrigin = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x, bounds.min.y);

        // ���߳���
        float rayLength = bounds.extents.y;

        // ���е����»�������
        Debug.DrawRay(center, Vector2.down * rayLength, Color.green);

        // �������߽����»�������
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        // ��������Ƿ���������
        RaycastHit2D centerHit = Physics2D.Raycast(center, Vector2.down, rayLength);
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength);

        // �������
        if (leftHit.collider != null && rightHit.collider != null)
        {
            print("111");
        }
    }
}
