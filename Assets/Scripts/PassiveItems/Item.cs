using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品类的基类，用于武器（Weapon）和被动技能（Passive）的共同逻辑处理。
/// 主要用于处理武器或被动技能的进化机制，因为两者都需要支持进化功能。
/// </summary>
public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventory inventory;
    protected PlayerStats owner;

    /// <summary>
    /// 初始化物品数据。
    /// </summary>
    /// <param name="data">物品的数据对象，包含最大等级和进化信息。</param>
    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;
        
        // 存储进化数据，用于后续判断是否满足进化条件。
        evolutionData = data.evolutionData;

        // 当前通过查找方式获取玩家背包和角色属性组件，未来应优化引用方式。
        inventory = FindFirstObjectByType<PlayerInventory>();
        owner = FindFirstObjectByType<PlayerStats>();
    }

    /// <summary>
    /// 获取当前可以进化的所有进化选项。
    /// </summary>
    /// <returns>返回一个数组，包含当前可以进行的所有进化选项。</returns>
    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        // 遍历所有进化选项，检查是否满足进化条件。
        foreach (ItemData.Evolution e in evolutionData)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }

        return possibleEvolutions.ToArray();
    }
    
    /// <summary>
    /// 检查指定的进化选项是否可以执行。
    /// </summary>
    /// <param name="evolution">要检查的进化选项。</param>
    /// <param name="levelUpAmount">当前物品将要提升的等级数，默认为1。</param>
    /// <returns>如果满足进化条件返回true，否则返回false。</returns>
    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
    {
        // 如果当前等级未达到进化所需等级，则无法进化。
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning(string.Format("进化失败。当前等级 {0}，进化所需等级 {1}", currentLevel, evolution.evolutionLevel));
            return false;
        }

        // 检查所有催化剂是否都在背包中且满足等级要求。
        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level)
            {
                Debug.LogWarning(string.Format("进化失败。缺少 {0}", c.itemType.name));
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 尝试执行指定的进化操作。
    /// </summary>
    /// <param name="evolutionData">要执行的进化选项数据。</param>
    /// <param name="levelUpAmount">当前物品将要提升的等级数，默认为1。</param>
    /// <returns>如果成功执行进化返回true，否则返回false。</returns>
    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolutionData, levelUpAmount))
            return false;

        // 判断是否需要消耗被动技能或武器。
        bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passives) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

        // 遍历所有催化剂，根据配置决定是否消耗。
        foreach (ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if (c.itemType is PassiveData && consumePassives) inventory.Remove(c.itemType, true);
            if (c.itemType is WeaponData && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        // 根据类型判断是否需要消耗自身。
        if (this is Passive && consumePassives) inventory.Remove((this as Passive).data, true);
        else if (this is Weapon && consumeWeapons) inventory.Remove((this as Weapon).data, true);

        // 将进化结果添加到背包中。
        inventory.Add(evolutionData.outcome.itemType);

        return true;
    }
    
    /// <summary>
    /// 检查当前物品是否可以升级。
    /// </summary>
    /// <returns>如果当前等级未达到最大等级返回true，否则返回false。</returns>
    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    /// <summary>
    /// 执行物品升级操作，并尝试触发自动进化。
    /// </summary>
    /// <returns>始终返回true。</returns>
    public virtual bool DoLevelUp()
    {
        if (evolutionData == null) return true;

        // 遍历所有进化选项，如果是自动触发类型，则尝试执行进化。
        foreach (ItemData.Evolution e in evolutionData)
        {
            if (e.condition == ItemData.Evolution.Condition.auto)
                AttemptEvolution(e);
        }
        return true;
    }

    /// <summary>
    /// 当物品被装备时调用，用于处理装备时的效果。
    /// </summary>
    public virtual void OnEquip() { }

    /// <summary>
    /// 当物品被卸下时调用，用于移除装备时的效果。
    /// </summary>
    public virtual void OnUnequip() { }
}
