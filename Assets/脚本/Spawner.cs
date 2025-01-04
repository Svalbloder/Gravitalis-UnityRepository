using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance; // 单例实例;
    [Header("生成设置")]
    public List<GameObject> prefabs; // 预制体列表，用于随机生成
    public Transform spawnPoint;    // 固定的生成位置

    [Header("生成时间设置")]
    public float minSpawnTime = 1f; // 生成间隔的最小时间（秒）
    public float maxSpawnTime = 3f; // 生成间隔的最大时间（秒）

    [Header("移动设置")]
    public float moveSpeed = 2f; // 生成物体的移动速度

    [Header("控制设置")]
    public bool isSpawning = true; // 控制生成协程的开关
    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(SpawnCoroutine()); // 开始生成协程
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            // 如果开关关闭，则暂停生成
            if (!isSpawning)
            {
                yield return null;
                continue;
            }

            // 随机等待时间
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // 随机生成预制体
            if (prefabs.Count > 0)
            {
                int randomIndex = Random.Range(0, prefabs.Count);
                GameObject spawnedObject = Instantiate(prefabs[randomIndex], spawnPoint.position, Quaternion.identity);

                // 为生成的物体添加移动逻辑
                if (spawnedObject.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.velocity = Vector2.down * moveSpeed; // 设置物体向下的速度
                }
                else
                {
                    spawnedObject.AddComponent<Rigidbody2D>().velocity = Vector2.down * moveSpeed; // 如果没有Rigidbody2D组件，添加并设置速度
                }

                // 可选：在物体离开视野或一段时间后销毁
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

