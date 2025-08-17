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

    /// <summary>
    /// 2D触发器进入事件处理函数
    /// 当武器碰撞体与敌人或可破坏道具接触时，对其造成伤害
    /// </summary>
    /// <param name="other">触发碰撞的另一个碰撞体对象</param>
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否与敌人碰撞且该敌人未被标记过
        if (other.CompareTag("Enemy") && !markedEnemies.Contains(other.gameObject))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrentDamage(), transform.position);
            
                markedEnemies.Add(other.gameObject);
            }
        }
        // 检查是否与可破坏道具碰撞且该道具未被标记过
        else if (other.CompareTag("Prop"))
        {
            if (other.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(other.gameObject))
            {
                breakable.TakeDamage(GetCurrentDamage());
                
                markedEnemies.Add(other.gameObject);
            }
        }
    }
}

