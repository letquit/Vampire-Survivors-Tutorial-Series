using System;
using UnityEngine;

/// <summary>
/// ChunkTrigger类用于检测玩家进入特定区域，并通知地图控制器当前所在的区块
/// </summary>
public class ChunkTrigger : MonoBehaviour
{
    private MapController mc;
    public GameObject targetMap;

    /// <summary>
    /// Start方法在游戏对象启用时调用，用于初始化组件引用
    /// </summary>
    private void Start()
    {
        mc = FindObjectOfType<MapController>();
    }

    /// <summary>
    /// OnTriggerStay2D方法在碰撞体持续接触时每帧调用，用于检测玩家是否停留在触发区域内
    /// </summary>
    /// <param name="other">与触发器发生接触的碰撞体对象</param>
    private void OnTriggerStay2D(Collider2D other)
    {
        // 当接触对象是玩家时，更新地图控制器的当前区块
        if (other.CompareTag("Player"))
        {
            mc.currentChunk = targetMap;
        }
    }

    /// <summary>
    /// OnTriggerExit2D方法在碰撞体离开触发区域时调用，用于处理玩家离开触发区域的逻辑
    /// </summary>
    /// <param name="other">离开触发区域的碰撞体对象</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        // 当离开的对象是玩家且当前区块等于目标地图时，清空当前区块引用
        if (other.CompareTag("Player"))
        {
            if (mc.currentChunk == targetMap)
            {
                mc.currentChunk = null;
            }
        }
    }
}

