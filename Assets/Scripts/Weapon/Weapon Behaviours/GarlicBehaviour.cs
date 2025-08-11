using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大蒜武器行为类，继承自近战武器行为基类
/// 用于处理大蒜武器在游戏中的具体行为和逻辑
/// </summary>
public class GarlicBehaviour : MeleeWeaponBehaviour
{
    private List<GameObject> markedEnemies;
    /// <summary>
    /// 初始化函数，在对象创建时调用
    /// 负责初始化大蒜武器的初始状态和属性
    /// </summary>
    protected override void Start()
    {
        // 调用父类的Start方法，确保基类初始化逻辑得到执行
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemey") && !markedEnemies.Contains(other.gameObject))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
            
            markedEnemies.Add(other.gameObject);
        }
    }
}

