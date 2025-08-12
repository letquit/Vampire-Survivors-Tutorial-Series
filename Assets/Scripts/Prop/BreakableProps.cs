using UnityEngine;

/// <summary>
/// 可破坏道具类，用于处理游戏中可被破坏的物体
/// </summary>
public class BreakableProps : MonoBehaviour
{
    public float health;
    
    /// <summary>
    /// 承受伤害处理函数
    /// </summary>
    /// <param name="damage">受到的伤害值，将从当前生命值中扣除</param>
    public void TakeDamage(float damage)
    {
        // 扣除伤害值
        health -= damage;
        
        // 检查是否死亡
        if (health <= 0)
        {
            Kill();
        }
    }
    
    /// <summary>
    /// 销毁物体函数
    /// </summary>
    private void Kill()
    {
        Destroy(gameObject);
    }
}

