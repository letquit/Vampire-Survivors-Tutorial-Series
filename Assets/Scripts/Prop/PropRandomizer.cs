using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 道具随机生成器类，用于在指定位置随机生成道具
/// </summary>
public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;
    public List<GameObject> propPrefabs;
    private bool hasSpawnedInitialProps = false;

    private void Start()
    {
        
    }
    
    private void Update()
    {
        
    }

    /// <summary>
    /// 在预设的生成点位置随机生成道具
    /// 遍历所有生成点，在每个生成点附近随机位置实例化随机道具
    /// </summary>
    private void SpawnProps()
    {
        // 遍历所有道具生成点
        foreach (GameObject sp in propSpawnPoints)
        {
            // 生成随机坐标偏移量
            float randomX = Random.Range(-9.5f, 9.5f);
            float randomY = Random.Range(-9.5f, 9.5f);
            Vector3 randomPosition = new Vector3(randomX, randomY, 0f);

            // 随机选择一个道具预制体进行实例化
            int rand = Random.Range(0, propPrefabs.Count);
            // GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            GameObject prop = Instantiate(propPrefabs[rand], randomPosition, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }
    }
    
    /// <summary>
    /// 在指定区块内随机生成道具
    /// 遍历区块的所有子物体，在每个子物体附近随机位置生成随机道具
    /// </summary>
    /// <param name="chunk">要生成道具的目标区块游戏对象</param>
    public void SpawnPropsOnChunk(GameObject chunk)
    {
        // 遍历区块的所有子变换组件
        foreach (Transform child in chunk.transform)
        {
            // 生成随机坐标偏移量
            float randomX = Random.Range(-9.5f, 9.5f);
            float randomY = Random.Range(-9.5f, 9.5f);
            Vector3 randomPosition = new Vector3(randomX, randomY, 0f);
            
            // 计算世界坐标位置
            Vector3 worldPosition = chunk.transform.position + randomPosition;
            
            // 随机选择一个道具预制体进行实例化
            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prop = Instantiate(propPrefabs[rand], worldPosition, Quaternion.identity);
            prop.transform.parent = child;
        }
    }
    
    /// <summary>
    /// 生成初始道具，确保只执行一次
    /// 调用SpawnProps方法生成道具，并设置标记防止重复执行
    /// </summary>
    public void SpawnInitialProps()
    {
        // 检查是否已经生成过初始道具，避免重复生成
        if (!hasSpawnedInitialProps)
        {
            SpawnProps();
            hasSpawnedInitialProps = true;
        }
    }
}
