using System;
using UnityEngine;

/// <summary>
/// 玩家收集器类，负责检测和收集可收集物体
/// </summary>
public class PlayerCollector : MonoBehaviour
{
    private PlayerStats player;
    private CircleCollider2D playerCollector;
    public float pullSpeed;

    /// <summary>
    /// 初始化组件引用和玩家状态引用
    /// </summary>
    private void Start()
    {
        player = FindFirstObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    /// <summary>
    /// 每帧更新收集器的碰撞体半径，使其与玩家当前磁力值同步
    /// </summary>
    private void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    /// <summary>
    /// 当其他物体进入触发器时的处理逻辑
    /// </summary>
    /// <param name="other">进入触发器的碰撞体对象</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞物体是否实现了ICollectible接口
        if (other.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // 计算从收集物体到玩家的力方向，并施加拉力
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - other.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);
            
            // 调用收集物体的收集方法
            collectible.Collect();
        }
    }
}

