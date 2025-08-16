using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人生成器类，用于控制游戏中的敌人波次生成逻辑。
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Range Settings")]
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 8f;
    
    public List<Wave> waves;
    public int currentWaveCount;

    [Header("Spawner Attributes")]
    private float spawnTimer;
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval;

    private bool isWaveActive = false;
    
    private Transform player;

    /// <summary>
    /// 初始化敌人生成器，获取玩家对象并计算当前波次配额。
    /// </summary>
    private void Start()
    {
        player = FindFirstObjectByType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    /// <summary>
    /// 每帧更新敌人生成逻辑，包括波次切换和敌人生成。
    /// </summary>
    private void Update()
    {
        // 如果当前波次已结束，则开始下一波
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }
        
        spawnTimer += Time.deltaTime;

        // 判断是否到达生成间隔时间，执行敌人生成
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    /// <summary>
    /// 开始下一波敌人生成前的等待协程。
    /// </summary>
    /// <returns>IEnumerator，用于协程执行</returns>
    IEnumerator BeginNextWave()
    {
        isWaveActive = true;
        
        yield return new WaitForSeconds(waveInterval);

        // 切换到下一波并重新计算配额
        if (currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }
    
    /// <summary>
    /// 计算当前波次需要生成的敌人总数。
    /// </summary>
    private void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    /// <summary>
    /// 根据当前波次配置生成敌人。
    /// </summary>
    private void SpawnEnemies()
    {
        // 判断是否还能继续生成敌人
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // 判断该敌人群组是否还有未生成的敌人
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // 使用工具类生成随机位置
                    Vector3 randomOffset = EnemyUtilities.GetRandomEnemyPosition(minSpawnDistance, maxSpawnDistance);

                    Instantiate(enemyGroup.enemyPrefab,
                        player.position + randomOffset,
                        Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                    
                    
                    // 判断是否达到最大敌人数量限制
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 当敌人被击杀时调用，减少当前存活敌人数量。
    /// </summary>
    public void OnEnemyKilled()
    {
        enemiesAlive--;
        
        // 如果当前敌人数量未达上限，则重置最大敌人数量标记
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
    
    /// <summary>
    /// 波次配置类，用于定义每一波敌人的生成规则。
    /// </summary>
    [Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }
    
    /// <summary>
    /// 敌人群组配置类，用于定义每种敌人的生成数量和预制体。
    /// </summary>
    [Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;
    }
}
