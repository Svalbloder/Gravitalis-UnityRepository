using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance; // ����ʵ��;
    [Header("��������")]
    public List<GameObject> prefabs; // Ԥ�����б������������
    public Transform spawnPoint;    // �̶�������λ��

    [Header("����ʱ������")]
    public float minSpawnTime = 1f; // ���ɼ������Сʱ�䣨�룩
    public float maxSpawnTime = 3f; // ���ɼ�������ʱ�䣨�룩

    [Header("�ƶ�����")]
    public float moveSpeed = 2f; // ����������ƶ��ٶ�

    [Header("��������")]
    public bool isSpawning = true; // ��������Э�̵Ŀ���
    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(SpawnCoroutine()); // ��ʼ����Э��
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            // ������عرգ�����ͣ����
            if (!isSpawning)
            {
                yield return null;
                continue;
            }

            // ����ȴ�ʱ��
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // �������Ԥ����
            if (prefabs.Count > 0)
            {
                int randomIndex = Random.Range(0, prefabs.Count);
                GameObject spawnedObject = Instantiate(prefabs[randomIndex], spawnPoint.position, Quaternion.identity);

                // Ϊ���ɵ���������ƶ��߼�
                if (spawnedObject.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.velocity = Vector2.down * moveSpeed; // �����������µ��ٶ�
                }
                else
                {
                    spawnedObject.AddComponent<Rigidbody2D>().velocity = Vector2.down * moveSpeed; // ���û��Rigidbody2D�������Ӳ������ٶ�
                }

                // ��ѡ���������뿪��Ұ��һ��ʱ�������
                Destroy(spawnedObject, 100f);
            }
        }
    }
    public void turnOff()
    {
        isSpawning = false;
    }
    public void turnOn()
    {
        isSpawning = true;
    }
}

