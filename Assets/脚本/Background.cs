using System.Collections.Generic;
using UnityEngine;


public class Background : MonoBehaviour
{
    public List<BackgroundObject> backgroundObjects; // �����������������б�
    public Transform pointA;                         // A�㣨�ϱ�Ե��
    public Transform pointB;                         // B�㣨�±�Ե��

    private List<GameObject> activeBackgrounds = new List<GameObject>(); // ��ǰ��Ծ�ı��������б�

    void Start()
    {
        FillBackground();
    }

    void Update()
    {
        MoveBackgrounds();
        CheckAndRecycleBackgrounds();
    }

    // ����A��B֮��ľ��룬ȷ��û�п�϶��ÿ���������嶼�������
    private void FillBackground()
    {
        foreach (var bgObject in backgroundObjects)
        {
            float currentY = pointB.position.y;
            while (currentY < pointA.position.y)
            {
                GameObject prefab = bgObject.prefab;
                float height = GetBackgroundHeight(prefab);

                // ��֤����� x ��λ���� prefab һ��
                Vector3 spawnPosition = new Vector3(prefab.transform.position.x, currentY + height / 2, pointA.position.z);
                GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
                activeBackgrounds.Add(instance);

                currentY += height; // ����λ�ã�׼��������һ��ʵ��
            }
        }
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

    // ��鱳�������Ƿ񳬳�B�㣬�����ջ������µ�����
    private void CheckAndRecycleBackgrounds()
    {
        // ���ճ���B��ı�������
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

        // Ϊÿ�ֱ�������������ʵ��
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

                    // ��֤������� x ��λ���� prefab һ��
                    Vector3 spawnPosition = new Vector3(
                        newPrefab.transform.position.x,  // ʹ��Ԥ��� x ��λ��
                        lastBg.transform.position.y + lastBgHeight / 2 + newHeight / 2,
                        pointA.position.z
                    );

                    GameObject newInstance = Instantiate(newPrefab, spawnPosition, Quaternion.identity, transform);
                    activeBackgrounds.Add(newInstance);
                }
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

    // ��ȡ�뱳�������Ӧ��BackgroundObject
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
