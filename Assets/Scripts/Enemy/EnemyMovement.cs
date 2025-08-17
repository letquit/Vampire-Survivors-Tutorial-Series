using System;
using UnityEngine;

/// <summary>
/// 敌人移动控制类，用于控制敌人向玩家移动的行为
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    private EnemyStats enemy;
    private Transform player;

    private Vector2 knockbackVelocity;
    private float knockbackDuration;

    /// <summary>
    /// 初始化函数，在对象启用时执行一次
    /// 主要用于获取玩家对象的Transform组件引用
    /// </summary>
    private void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    /// <summary>
    /// 每帧更新函数，控制敌人向玩家位置移动
    /// 使用Vector2.MoveTowards方法实现平滑移动效果
    /// </summary>
    private void Update()
    {
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // 计算敌人向玩家移动的新位置
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;
        
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}

