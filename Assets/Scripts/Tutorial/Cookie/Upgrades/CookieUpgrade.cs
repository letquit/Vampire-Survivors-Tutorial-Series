using System;
using UnityEngine;

/// <summary>
/// 抽象类，表示Cookie游戏中的升级项
/// 继承自ScriptableObject，用于创建可配置的游戏升级数据
/// </summary>
public abstract class CookieUpgrade : ScriptableObject
{
    /// <summary>
    /// 升级效果的数值量
    /// </summary>
    public float upgradeAmount = 1f;

    /// <summary>
    /// 升级的初始成本
    /// </summary>
    public double originalUpgradeCost = 100f;
    
    /// <summary>
    /// 升级的当前成本（会随着购买次数增加）
    /// </summary>
    public double currentUpgradeCost = 100f;
    
    /// <summary>
    /// 每次购买后成本增加的倍数
    /// </summary>
    public double costIncreaseMultiplierPerPurchase = 0.05f;

    /// <summary>
    /// 升级按钮上显示的文本
    /// </summary>
    public string upgradeButtonText;
    
    /// <summary>
    /// 升级按钮的描述文本，使用TextArea属性支持多行编辑
    /// </summary>
    [TextArea(3, 10)] 
    public string upgradeButtonDescription;

    /// <summary>
    /// 应用升级效果的抽象方法
    /// 子类必须实现具体的升级逻辑
    /// </summary>
    public abstract void ApplyUpgrade();

    /// <summary>
    /// Unity编辑器回调方法，在脚本验证时重置当前升级成本为初始成本
    /// </summary>
    private void OnValidate()
    {
        currentUpgradeCost = originalUpgradeCost;
    }
}