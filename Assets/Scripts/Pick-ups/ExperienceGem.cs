using System;
using UnityEngine;

/// <summary>
/// 经验宝石类，继承自Pickup类并实现ICollectible接口
/// 用于玩家收集后获得经验值的游戏对象
/// </summary>
public class ExperienceGem : Pickup, ICollectible
{
    public int experienceGranted;
    
    /// <summary>
    /// 收集经验宝石的方法
    /// 查找玩家状态组件并增加相应经验值
    /// </summary>
    public void Collect()
    {
        // 查找场景中的玩家状态组件
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        // 为玩家增加经验奖励
        player.IncreaseExperience(experienceGranted);
    }

    
}
