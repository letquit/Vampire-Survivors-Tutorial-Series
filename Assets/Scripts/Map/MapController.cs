using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// 地图控制器，用于管理地形区块的生成与优化。
/// </summary>
public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private Vector3 playerLastPosition;
    // private PlayerMovement pm;

    [Header("Optimization")] 
    public List<GameObject> spawnedChunks;
    private GameObject latestChunk;
    public float maxOpDist;
    private float opDist;
    private float optimizerCooldown;
    public float optimizerCooldownDur;

    /// <summary>
    /// 初始化组件引用并生成初始道具。
    /// </summary>
    private void Start()
    {
        playerLastPosition = player.transform.position;
        
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
    
        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        // 只在有明显移动时才检查，但检查所有九个方向
        if (moveDir.magnitude >= 0.1f) 
        {
            // 检查九宫格内的所有方向
            CheckAndSpawnChunk("Right");
            CheckAndSpawnChunk("Left");
            CheckAndSpawnChunk("Up");
            CheckAndSpawnChunk("Down");
            CheckAndSpawnChunk("Right Up");
            CheckAndSpawnChunk("Right Down");
            CheckAndSpawnChunk("Left Up");
            CheckAndSpawnChunk("Left Down");
        }
    }

    /// <summary>
    /// 检查指定方向是否存在地形区块，若不存在则生成新地块。
    /// </summary>
    /// <param name="direction">要检查的方向名称（如 "Right", "Up" 等）</param>
    private void CheckAndSpawnChunk(string direction)
    {
        if (string.IsNullOrEmpty(direction)) return;
    
        Transform directionTransform = currentChunk.transform.Find(direction);
        if (directionTransform == null) return;
    
        if (!Physics2D.OverlapCircle(directionTransform.position, checkerRadius, terrainMask))
        {
            SpawnChunk(directionTransform.position);
        }
    }

    /// <summary>
    /// 根据给定的方向向量获取对应的方向名称。
    /// </summary>
    /// <param name="direction">表示移动方向的向量</param>
    /// <returns>返回方向名称字符串，例如 "Right Up"、"Left" 等</returns>
    private string GetDirectionName(Vector3 direction)
    {
        if (direction == Vector3.zero) return "";
    
        direction = direction.normalized;
    
        bool right = direction.x > 0.3f;
        bool left = direction.x < -0.3f;
        bool up = direction.y > 0.3f;
        bool down = direction.y < -0.3f;
    
        if (right && up) return "Right Up";
        if (right && down) return "Right Down";
        if (left && up) return "Left Up";
        if (left && down) return "Left Down";
        if (right) return "Right";
        if (left) return "Left";
        if (up) return "Up";
        if (down) return "Down";
    
        return "";
    }

    /// <summary>
    /// 在指定位置生成一个随机地形区块，并为其生成道具。
    /// </summary>
    /// <param name="spawnPosition">新地形区块的生成位置</param>
    private void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        
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
