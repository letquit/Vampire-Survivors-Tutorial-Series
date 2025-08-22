using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人移动控制类，用于控制敌人向玩家移动的行为
/// </summary>
public class EnemyMovement : Sortable
{
    protected EnemyStats stats;
    protected Transform player;
    protected Rigidbody2D rb;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    /// <summary>
    /// 定义敌人移出画面后的处理方式
    /// none: 不做任何处理
    /// respawnAtEdge: 在画面边缘重新生成
    /// despawn: 销毁敌人对象
    /// </summary>
    public enum OutOfFrameAction {none, respawnAtEdge, despawn}
    
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;
    
    [Flags]
    public enum KnockbackVariance { duration = 1, velocity = 2}
    public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;
    
    protected bool spawnedOutOfFrame;
    
    /// <summary>
    /// 初始化函数，在对象启用时执行一次
    /// 主要用于获取玩家对象的Transform组件引用
    /// </summary>
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
        stats = GetComponent<EnemyStats>();
        
        // 在屏幕上随机选择一个玩家，而不是总是选择第一个玩家。
        PlayerMovement[] allPlayers = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        player = allPlayers[Random.Range(0, allPlayers.Length)].transform;
    }

    /// <summary>
    /// 每帧更新函数，控制敌人向玩家位置移动
    /// 使用Vector2.MoveTowards方法实现平滑移动效果
    /// </summary>
    protected virtual void Update()
    {
        // 如果处于击退状态，则应用击退效果
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleOutOfFrameAction();
        }
    }

    /// <summary>
    /// 处理敌人移出画面边界的情况
    /// 根据outOfFrameAction枚举值执行相应操作
    /// </summary>
    protected virtual void HandleOutOfFrameAction()
    {
        // 当敌人移出画面时进行处理。
        if (!SpawnManager.IsWithinBoundaries(transform))
        {
            switch (outOfFrameAction)
            {
                case OutOfFrameAction.none: default:
                    break;
                case OutOfFrameAction.respawnAtEdge:
                    // 如果敌人移出相机画面，将其传送到画面边缘。
                    transform.position = SpawnManager.GeneratePosition();
                    break;
                case OutOfFrameAction.despawn:
                    // 如果敌人是在画面外生成的，则不销毁。
                    if (!spawnedOutOfFrame)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        } else spawnedOutOfFrame = false;
    }
    
    /// <summary>
    /// 应用击退效果
    /// </summary>
    /// <param name="velocity">击退速度向量</param>
    /// <param name="duration">击退持续时间（秒）</param>
    public virtual void Knockback(Vector2 velocity, float duration)
    {
        // 如果当前已有击退效果，则不重复应用
        if (knockbackDuration > 0) return;
        
        // 如果击退类型设置为无，则忽略击退。
        if (knockbackVariance == 0) return;
        
        // 检查stats对象是否存在
        if (stats == null) return;

        // 仅在乘数不是0或1时更改因子。
        float pow = 1;
        bool reducesVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0,
            reducesDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

        if (reducesVelocity && reducesDuration) pow = 0.5f;

        // 检查要影响的击退值。
        knockbackVelocity = velocity * (reducesVelocity ? Mathf.Pow(stats.Actual.knockbackMultiplier, pow) : 1);
        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(stats.Actual.knockbackMultiplier, pow) : 1);
    }

    
    /// <summary>
    /// 控制敌人向玩家移动
    /// 使用Vector2.MoveTowards实现平滑移动
    /// </summary>
    public virtual void Move()
    {
        // 如果存在刚体，则使用它来移动而不是直接移动位置。
        // 这样可以优化性能。
        if (rb)
        {
            rb.MovePosition(Vector2.MoveTowards(
                rb.position,
                player.transform.position,
                stats.Actual.moveSpeed * Time.deltaTime
            ));
        }
        else
        {
            // 持续将敌人向玩家移动
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                stats.Actual.moveSpeed * Time.deltaTime
            );
        }
    }
}
