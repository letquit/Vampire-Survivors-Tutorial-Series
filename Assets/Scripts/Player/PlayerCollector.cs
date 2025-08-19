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

    /// <summary>
    /// 初始化组件引用和玩家状态引用
    /// </summary>
    private void Start()
    {
        player = GetComponentInParent<PlayerStats>();
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

