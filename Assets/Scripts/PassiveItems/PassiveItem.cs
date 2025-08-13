using System;
using UnityEngine;

/// <summary>
/// 被动物品类，用于处理玩家的被动技能和属性加成
/// </summary>
public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    /// <summary>
    /// 初始化被动物品，获取玩家状态组件并应用修饰效果
    /// </summary>
    private void Start()
    {
        // 查找场景中的玩家状态组件
        player = FindFirstObjectByType<PlayerStats>();
        // 应用被动物品的修饰效果
        ApplyModifier();
    }

    /// <summary>
    /// 应用被动物品的属性修饰效果，子类需要重写此方法来实现具体的效果
    /// </summary>
    protected virtual void ApplyModifier()
    {
        
    }
}

