// <summary>

using UnityEngine;

/// <summary>
/// WeaponScriptableObject类的替代品。我们的想法是希望将所有武器进化数据存储在一个单一对象中，
/// 而不是像继续使用WeaponScriptableObject那样需要多个对象来存储单个武器的数据。
/// </summary>
[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-down Rogue-like/Weapon Data")]
public class WeaponData : ItemData
{
    [HideInInspector] 
    public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;
    
    /// <summary>
    /// 获取指定等级的武器属性数据和描述信息
    /// </summary>
    /// <param name="level">武器等级</param>
    /// <returns>指定等级的武器属性数据</returns>
    public Weapon.Stats GetLevelData(int level)
    {
        // 从线性成长数组中获取下一级的属性数据
        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        // 如果线性成长数组没有数据，则从随机成长数组中随机选择一个属性数据
        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        // 如果都没有配置数据，则返回空值并输出警告信息
        Debug.LogWarning(string.Format("武器未配置第{0}级的升级属性！", level));
        return new Weapon.Stats();
    }
}
