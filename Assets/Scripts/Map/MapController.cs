using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 地图控制器，用于管理地形区块的生成与优化。
/// </summary>
public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    private Vector3 noTerrainPosition;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private PlayerMovement pm;

    [Header("Optimization")] 
    public List<GameObject> spawnedChunks;
    private GameObject latestChunk;
    public float maxOpDist;
    private float opDist;
    private float optimizerCooldown;
    public float optimizerCooldownDur;
    
    // 九宫格方向偏移
    private Vector3[] directionOffsets = {
        new Vector3(0, 0, 0),      // 中心(当前区块)
        new Vector3(20, 0, 0),     // 右
        new Vector3(-20, 0, 0),    // 左
        new Vector3(0, 20, 0),     // 上
        new Vector3(0, -20, 0),    // 下
        new Vector3(20, 20, 0),    // 右上
        new Vector3(20, -20, 0),   // 右下
        new Vector3(-20, 20, 0),   // 左上
        new Vector3(-20, -20, 0)   // 左下
    };
    
    private string[] directionNames = {
        "", "Right", "Left", "Up", "Down", 
        "Right Up", "Right Down", "Left Up", "Left Down"
    };

    /// <summary>
    /// 初始化组件引用并生成初始道具。
    /// </summary>
    private void Start()
    {
        pm = FindFirstObjectByType<PlayerMovement>();
        
        // 生成初始地块的道具
        PropRandomizer propRandomizer = FindFirstObjectByType<PropRandomizer>();
        if (propRandomizer != null)
        {
            propRandomizer.SpawnInitialProps();
        }
    }

    /// <summary>
    /// 每帧更新地图区块检查和优化逻辑。
    /// </summary>
    private void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    /// <summary>
    /// 检查当前区块周围是否需要生成新的地形区块。
    /// </summary>
    private void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }
        
        for (int i = 1; i < directionOffsets.Length; i++)
        {
            Vector3 checkPosition;
            
            Transform directionTransform = currentChunk.transform.Find(directionNames[i]);
            if (directionTransform != null)
            {
                checkPosition = directionTransform.position;
            }
            else
            {
                checkPosition = currentChunk.transform.position + directionOffsets[i];
            }
            
            if (!Physics2D.OverlapCircle(checkPosition, checkerRadius, terrainMask))
            {
                noTerrainPosition = checkPosition;
                SpawnChunk();
            }
        }
    }

    /// <summary>
    /// 在指定位置生成一个随机地形区块，并为其生成道具。
    /// </summary>
    private void SpawnChunk()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion.identity);
        
        PropRandomizer propRandomizer = FindFirstObjectByType<PropRandomizer>();
        if (propRandomizer != null)
        {
            propRandomizer.SpawnPropsOnChunk(latestChunk);
        }
        
        spawnedChunks.Add(latestChunk);
    }

    /// <summary>
    /// 定期优化已生成的区块，超出距离的区块将被禁用以节省性能。
    /// </summary>
    private void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }
        
        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
