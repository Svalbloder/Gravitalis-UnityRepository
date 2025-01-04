using System.Collections.Generic;
using UnityEngine;


public class Background : MonoBehaviour
{
    public List<BackgroundObject> backgroundObjects; // 包含多个背景物体的列表
    public Transform pointA;                         // A点（上边缘）
    public Transform pointB;                         // B点（下边缘）

    private List<GameObject> activeBackgrounds = new List<GameObject>(); // 当前活跃的背景物体列表

    void Start()
    {
        FillBackground();
    }

    void Update()
    {
        MoveBackgrounds();
        CheckAndRecycleBackgrounds();
    }

    // 填充从A到B之间的距离，确保没有空隙，每个背景物体都独立填充
    private void FillBackground()
    {
        foreach (var bgObject in backgroundObjects)
        {
            float currentY = pointB.position.y;
            while (currentY < pointA.position.y)
            {
                GameObject prefab = bgObject.prefab;
                float height = GetBackgroundHeight(prefab);

                // 保证物体的 x 轴位置与 prefab 一致
                Vector3 spawnPosition = new Vector3(prefab.transform.position.x, currentY + height / 2, pointA.position.z);
                GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
                activeBackgrounds.Add(instance);

                currentY += height; // 更新位置，准备放置下一个实例
            }
        }
    }

    // 获取背景物体的高度
    private float GetBackgroundHeight(GameObject background)
    {
        SpriteRenderer renderer = background.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y;
        }
        Debug.LogWarning("Background prefab is missing SpriteRenderer component!");
        return 0f;
    }

    // 移动背景物体
    private void MoveBackgrounds()
    {
        foreach (GameObject bg in activeBackgrounds)
        {
            if (bg != null)
            {
                BackgroundObject bgObject = GetCorrespondingBackgroundObject(bg);
                if (bgObject != null)
                {
                    bg.transform.position += Vector3.down * bgObject.moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    // 检查背景物体是否超出B点，并回收或生成新的物体
    private void CheckAndRecycleBackgrounds()
    {
        // 回收超出B点的背景物体
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            GameObject bg = activeBackgrounds[i];
            float bgHeight = GetBackgroundHeight(bg);

            if (bg.transform.position.y + bgHeight / 2 < pointB.position.y)
            {
                activeBackgrounds.RemoveAt(i);
                Destroy(bg);
            }
        }

        // 为每种背景物体生成新实例
        foreach (var bgObject in backgroundObjects)
        {
            GameObject lastBg = FindLastBackground(bgObject.prefab);
            if (lastBg != null)
            {
                float lastBgHeight = GetBackgroundHeight(lastBg);

                if (lastBg.transform.position.y - lastBgHeight / 2 < pointA.position.y)
                {
                    GameObject newPrefab = bgObject.prefab;
                    float newHeight = GetBackgroundHeight(newPrefab);

                    // 保证新物体的 x 轴位置与 prefab 一致
                    Vector3 spawnPosition = new Vector3(
                        newPrefab.transform.position.x,  // 使用预设的 x 轴位置
                        lastBg.transform.position.y + lastBgHeight / 2 + newHeight / 2,
                        pointA.position.z
                    );

                    GameObject newInstance = Instantiate(newPrefab, spawnPosition, Quaternion.identity, transform);
                    activeBackgrounds.Add(newInstance);
                }
            }
        }
    }

    // 查找特定类型背景物体的最后一个实例
    private GameObject FindLastBackground(GameObject prefab)
    {
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            if (activeBackgrounds[i].name.StartsWith(prefab.name))
            {
                return activeBackgrounds[i];
            }
        }
        return null;
    }

    // 获取与背景物体对应的BackgroundObject
    private BackgroundObject GetCorrespondingBackgroundObject(GameObject background)
    {
        foreach (var bgObject in backgroundObjects)
        {
            if (background.name.StartsWith(bgObject.prefab.name))
            {
                return bgObject;
            }
        }
        return null;
    }
}
