using System;
using UnityEngine;

/// <summary>
/// 玩家收集器类，负责检测和收集可收集物体
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    private PlayerStats player;
    private CircleCollider2D detector;
    public float pullSpeed;

    public UICoinDisplay ui;

    private float coins;
    
    /// <summary>
    /// 初始化组件引用和玩家状态引用
    /// </summary>
    private void Start()
    {
        player = GetComponentInParent<PlayerStats>();
        coins = 0;
    }

    /// <summary>
    /// 设置检测器的半径大小
    /// </summary>
    /// <param name="r">要设置的半径值</param>
    public void SetRadius(float r)
    {
        // 获取检测器组件（如果尚未获取）
        if (!detector) detector = GetComponent<CircleCollider2D>();
        detector.radius = r;
    }
    
    /// <summary>
    /// 获取当前收集到的硬币数量
    /// </summary>
    /// <returns>当前收集到的硬币总数</returns>
    public float GetCoins() { return coins; }

    /// <summary>
    /// 增加指定数量的硬币，并更新UI显示
    /// </summary>
    /// <param name="amount">要增加的硬币数量</param>
    /// <returns>增加后的硬币总数量</returns>
    public float AddCoins(float amount)
    {
        coins += amount;
        ui.UpdateDisplay();
        return coins;
    }

    /// <summary>
    /// 将当前收集的硬币保存到游戏存档中，并重置当前硬币计数
    /// </summary>
    public void SaveCoinsToStash()
    {
        SaveManager.LastLoadedGameData.coins += coins;
        coins = 0;
        SaveManager.Save();
    }

    /// <summary>
    /// 当其他物体进入触发器时的处理逻辑
    /// </summary>
    /// <param name="other">进入触发器的碰撞体对象</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞体是否为可收集物品，如果是则执行收集逻辑
        if (other.TryGetComponent(out Pickup p))
        {
            p.Collect(player, pullSpeed);
        }
    }
}

