using UnityEngine;

/// <summary>
/// HealthPotion类继承自Pickup类并实现ICollectible接口，用于表示可以恢复玩家生命值的道具
/// </summary>
public class HealthPotion : Pickup
{
    public int healthToRestore;

    /// <summary>
    /// 收集道具时调用的方法，用于恢复玩家的生命值
    /// </summary>
    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }
        
        // 查找场景中的PlayerStats组件
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        // 调用玩家状态组件的恢复生命值方法
        player.RestoreHealth(healthToRestore);
    }
}

