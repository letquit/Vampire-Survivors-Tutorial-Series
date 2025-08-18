using System;
using UnityEngine;

/// <summary>
/// 翅膀被动物品类，用于增加玩家移动速度
/// </summary>
[Obsolete]
public class WingsPassiveItem : PassiveItem
{
    /// <summary>
    /// 应用被动物品修饰符，增加玩家移动速度
    /// </summary>
    protected override void ApplyModifier()
    {
        // 根据被动物品数据中的倍数增加玩家当前移动速度
        player.CurrentMoveSpeed *= 1 + passiveItemData.multipler / 100f;
    }
}
