using System;
using UnityEngine;

/// <summary>
/// 敌人属性管理类，用于控制敌人的移动速度、生命值和伤害等属性
/// </summary>
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    private float currentMoveSpeed;
    private float currentHealth;
    private float currentDamage;

    /// <summary>
    /// 在对象唤醒时初始化敌人的各项属性值
    /// 从enemyData中获取初始的移动速度、最大生命值和伤害值
    /// </summary>
    private void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }
    
    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    /// <param name="damage">造成的伤害值</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // 检查敌人是否死亡
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    /// <summary>
    /// 销毁敌人游戏对象
    /// </summary>
    private void Kill()
    {
        Destroy(gameObject);
    }
}

