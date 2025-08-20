using UnityEngine;

/// <summary>
/// 被动物品数据类，用于替代PassiveItemScriptableObject类。
/// 设计理念是将所有被动物品的等级数据存储在单个对象中，
/// 而不是为每个被动物品创建多个对象（这是继续使用PassiveItemScriptableObject时必须做的）。
/// </summary>
[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-down Rogue-like/Passive Data")]
public class PassiveData : ItemData
{
    /// <summary>
    /// 被动物品的基础属性数据
    /// </summary>
    public Passive.Modifier baseStats;

    /// <summary>
    /// 被动物品的等级成长数据数组，每个元素代表对应等级的属性加成
    /// </summary>
    public Passive.Modifier[] growth;

    /// <summary>
    /// 获取指定等级的被动物品数据
    /// </summary>
    /// <param name="level">要获取数据的等级</param>
    /// <returns>指定等级的属性修饰器数据</returns>
    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        // 从成长数组中获取下一等级的属性数据
        if (level - 2 < growth.Length)
            return growth[level - 2];

        // 返回空值并输出警告信息
        Debug.LogWarning(string.Format("被动物品未配置{0}级的升级属性！", level));
        return new Passive.Modifier();
    }
}
