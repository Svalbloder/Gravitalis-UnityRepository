using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundObject
{
    public GameObject prefab; // 预制体
    public float moveSpeed;   // 移动速度
    public bool soloMode;     // 单独模式设置，true 为单实例生成，false 为连续生成
    public bool once;
}

[System.Serializable]
public class BackgroundObjectList
{
    public List<BackgroundObject> objects; // 包含多个背景物体的列表
}

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance; // 单例实例;
    public List<BackgroundObject> backgroundObjects; // 当前背景物体列表
    public List<BackgroundObjectList> predefinedBackgroundLists; // 固定的背景列表集合
    public Transform pointA; // 上边缘（A 点）
    public Transform pointB; // 下边缘（B 点）
    public BackgroundObject cloud;
    private List<GameObject> activeBackgrounds = new List<GameObject>();
    public GameObject fakeObject;

    public int ChangeToIndex;
    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    void Start()
    {
        foreach (var bgObject in backgroundObjects)
        {
            if (!bgObject.soloMode)
            {
                FillBackground(bgObject);
            }
            else
            {
                SpawnSingleInstance(bgObject);
            }
        }
        fakeObject.SetActive(false);
    }

    void Update()
    {
        MoveBackgrounds();

        foreach (var bgObject in backgroundObjects)
        {
            if (!bgObject.soloMode)
            {
                CheckAndRecycleBackgrounds(bgObject);
            }
            else
            {
                CheckAndRespawnSingleInstance(bgObject);
            }
        }
    }

    // 填充从 A 到 B 之间的距离，确保没有空隙
    private void FillBackground(BackgroundObject bgObject)
    {
        float currentY = pointB.position.y;
        while (currentY < pointA.position.y)
        {
            GameObject prefab = bgObject.prefab;
            float height = GetBackgroundHeight(prefab);

            // 确保 x 轴对齐预制体
            Vector3 spawnPosition = new Vector3(
                prefab.transform.position.x,
                currentY + height / 2,
                prefab.transform.position.z
            );

            GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            activeBackgrounds.Add(instance);

            currentY += height; // 更新位置
        }
    }

    // 生成单个背景物体实例
    private void SpawnSingleInstance(BackgroundObject bgObject)
    {
        GameObject prefab = bgObject.prefab;
        float height = GetBackgroundHeight(prefab);
        Vector3 spawnPosition = new Vector3(
            prefab.transform.position.x,
            pointA.position.y + height / 2,
            prefab.transform.position.z
        );

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        activeBackgrounds.Add(instance);
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

    // 检查背景物体是否超出 B 点，并回收或生成新的物体
    private void CheckAndRecycleBackgrounds(BackgroundObject bgObject)
    {
        // 回收超出 B 点的背景物体
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            GameObject bg = activeBackgrounds[i];
            if (GetCorrespondingBackgroundObject(bg) != bgObject)
                continue;

            float bgHeight = GetBackgroundHeight(bg);

            if (bg.transform.position.y + bgHeight / 2 < pointB.position.y)
            {
                activeBackgrounds.RemoveAt(i);
                Destroy(bg);
            }
        }

        // 为该背景物体生成新实例
        GameObject lastBg = FindLastBackground(bgObject.prefab);
        if (lastBg != null)
        {
            float lastBgHeight = GetBackgroundHeight(lastBg);

            if (lastBg.transform.position.y - lastBgHeight / 2 < pointA.position.y)
            {
                GameObject newPrefab = bgObject.prefab;
                float newHeight = GetBackgroundHeight(newPrefab);

                Vector3 spawnPosition = new Vector3(
                    newPrefab.transform.position.x,
                    lastBg.transform.position.y + lastBgHeight / 2 + newHeight / 2,
                    newPrefab.transform.position.z
                );

                GameObject newInstance = Instantiate(newPrefab, spawnPosition, Quaternion.identity, transform);
                activeBackgrounds.Add(newInstance);
            }
        }
    }

    // 检查单实例模式下的背景物体是否超出 B 点，并重新生成
    private void CheckAndRespawnSingleInstance(BackgroundObject bgObject)
    {
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeBackgrounds[i];
            if (GetCorrespondingBackgroundObject(obj) != bgObject)
                continue;

            float objectHeight = GetBackgroundHeight(obj);

            if (obj.transform.position.y + objectHeight / 2 < pointB.position.y)
            {
                Destroy(obj);
                activeBackgrounds.RemoveAt(i);
                if(!bgObject.once)
                SpawnSingleInstance(bgObject);
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

    // 获取与背景物体对应的 BackgroundObject
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

    // 切换到指定的背景列表
    public void SwitchBackgroundList()
    {
        if (ChangeToIndex < 0 || ChangeToIndex >= predefinedBackgroundLists.Count)
        {
            Debug.LogWarning("Invalid list index!");
            return;
        }

        // 清除当前场景中的所有物体
        foreach (GameObject bg in activeBackgrounds)
        {
            Destroy(bg);
        }
        activeBackgrounds.Clear();

        // 替换为新的背景列表
        backgroundObjects = predefinedBackgroundLists[ChangeToIndex].objects;

        // 重新生成背景
        foreach (var bgObject in backgroundObjects)
        {
            if (!bgObject.soloMode)
            {
                FillBackground(bgObject);
            }
            else
            {
                SpawnSingleInstance(bgObject);
            }
        }
    }

    public void AddBackgroundObject(BackgroundObject newBgObject)
    {
        if (backgroundObjects.Contains(newBgObject))
        {
            Debug.LogWarning("The background object is already in the list!");
            return;
        }

        backgroundObjects.Add(newBgObject);
        if (newBgObject.soloMode)
        {
            SpawnSingleInstance(newBgObject);
        }
        else
        {
            FillBackground(newBgObject);
        }
    }

    public void RemoveBackgroundObject(BackgroundObject objectToRemove)
    {
        // 从背景物体列表中移除
        backgroundObjects.Remove(objectToRemove);
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            if (GetCorrespondingBackgroundObject(activeBackgrounds[i]) == objectToRemove)
            {               
                activeBackgrounds.RemoveAt(i);
            }
        }
    }

    public void ToSwitchScene()
    {
        if (cloud == null)
        {
            Debug.LogWarning("Cloud object is not assigned!");
            return;
        }

        // 生成 cloud 实例
        GameObject prefab = cloud.prefab;
        float height = GetBackgroundHeight(prefab);
        Vector3 spawnPosition = new Vector3(
            prefab.transform.position.x,
            pointA.position.y + height / 2,
            prefab.transform.position.z
        );

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

        // 临时管理 cloud 的单独移动和销毁逻辑
        StartCoroutine(MoveAndDestroyCloud(instance, cloud.moveSpeed));
    }
    private IEnumerator MoveAndDestroyCloud(GameObject cloudInstance, float moveSpeed)
    {
        while (cloudInstance != null)
        {
            // 向下移动
            cloudInstance.transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            // 检查是否超出边界
            if (cloudInstance.transform.position.y + GetBackgroundHeight(cloudInstance) / 2 < pointB.position.y)
            {
                Destroy(cloudInstance);
                break;
            }

            yield return null; // 等待下一帧
        }
    }
}
