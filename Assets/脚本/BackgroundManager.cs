using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundObject
{
    public GameObject prefab; // Ԥ����
    public float moveSpeed;   // �ƶ��ٶ�
    public bool soloMode;     // ����ģʽ���ã�true Ϊ��ʵ�����ɣ�false Ϊ��������
    public bool once;
}

[System.Serializable]
public class BackgroundObjectList
{
    public List<BackgroundObject> objects; // �����������������б�
}

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance; // ����ʵ��;
    public List<BackgroundObject> backgroundObjects; // ��ǰ���������б�
    public List<BackgroundObjectList> predefinedBackgroundLists; // �̶��ı����б���
    public Transform pointA; // �ϱ�Ե��A �㣩
    public Transform pointB; // �±�Ե��B �㣩
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

    // ���� A �� B ֮��ľ��룬ȷ��û�п�϶
    private void FillBackground(BackgroundObject bgObject)
    {
        float currentY = pointB.position.y;
        while (currentY < pointA.position.y)
        {
            GameObject prefab = bgObject.prefab;
            float height = GetBackgroundHeight(prefab);

            // ȷ�� x �����Ԥ����
            Vector3 spawnPosition = new Vector3(
                prefab.transform.position.x,
                currentY + height / 2,
                prefab.transform.position.z
            );

            GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            activeBackgrounds.Add(instance);

            currentY += height; // ����λ��
        }
    }

    // ���ɵ�����������ʵ��
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

    // ��ȡ��������ĸ߶�
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

    // �ƶ���������
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

    // ��鱳�������Ƿ񳬳� B �㣬�����ջ������µ�����
    private void CheckAndRecycleBackgrounds(BackgroundObject bgObject)
    {
        // ���ճ��� B ��ı�������
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

        // Ϊ�ñ�������������ʵ��
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

    // ��鵥ʵ��ģʽ�µı��������Ƿ񳬳� B �㣬����������
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

    // �����ض����ͱ�����������һ��ʵ��
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

    // ��ȡ�뱳�������Ӧ�� BackgroundObject
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

    // �л���ָ���ı����б�
    public void SwitchBackgroundList()
    {
        if (ChangeToIndex < 0 || ChangeToIndex >= predefinedBackgroundLists.Count)
        {
            Debug.LogWarning("Invalid list index!");
            return;
        }

        // �����ǰ�����е���������
        foreach (GameObject bg in activeBackgrounds)
        {
            Destroy(bg);
        }
        activeBackgrounds.Clear();

        // �滻Ϊ�µı����б�
        backgroundObjects = predefinedBackgroundLists[ChangeToIndex].objects;

        // �������ɱ���
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
        // �ӱ��������б����Ƴ�
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

        // ���� cloud ʵ��
        GameObject prefab = cloud.prefab;
        float height = GetBackgroundHeight(prefab);
        Vector3 spawnPosition = new Vector3(
            prefab.transform.position.x,
            pointA.position.y + height / 2,
            prefab.transform.position.z
        );

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

        // ��ʱ���� cloud �ĵ����ƶ��������߼�
        StartCoroutine(MoveAndDestroyCloud(instance, cloud.moveSpeed));
    }
    private IEnumerator MoveAndDestroyCloud(GameObject cloudInstance, float moveSpeed)
    {
        while (cloudInstance != null)
        {
            // �����ƶ�
            cloudInstance.transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            // ����Ƿ񳬳��߽�
            if (cloudInstance.transform.position.y + GetBackgroundHeight(cloudInstance) / 2 < pointB.position.y)
            {
                Destroy(cloudInstance);
                break;
            }

            yield return null; // �ȴ���һ֡
        }
    }
}
