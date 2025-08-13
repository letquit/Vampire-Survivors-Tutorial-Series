using UnityEngine;

/// <summary>
/// 菠菜被动道具类，继承自PassiveItem基类
/// 用于增强玩家的攻击力属性
/// </summary>
public class SpinachPassiveItem : PassiveItem
{
    /// <summary>
    /// 应用道具修饰符效果
    /// 该方法会根据道具数据中的倍率来增强玩家当前的攻击力
    /// </summary>
    protected override void ApplyModifier()
    {
        // 根据百分比倍率增加玩家的当前攻击力
        player.currentMight *= 1 + passiveItemData.multipler / 100f;
    }
}
