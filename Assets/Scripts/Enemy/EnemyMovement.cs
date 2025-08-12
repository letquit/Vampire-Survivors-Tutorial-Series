using System;
using UnityEngine;

/// <summary>
/// 敌人移动控制类，用于控制敌人向玩家移动的行为
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    private EnemyStats enemy;
    private Transform player;

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
        // 计算敌人向玩家移动的新位置
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
    }
}

