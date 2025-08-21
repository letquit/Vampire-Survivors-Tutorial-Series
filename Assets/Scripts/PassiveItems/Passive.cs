using UnityEngine;

/// <summary>
/// 被动技能类，继承自Item类。该类用于处理被动数据(PassiveData)，
/// 当玩家获得该被动技能时，会根据数据提升玩家的各项属性值。
/// </summary>
public class Passive : Item
{
    [SerializeField] CharacterData.Stats currentBoosts;

    /// <summary>
    /// 修饰符结构体，用于定义被动技能的属性加成
    /// </summary>
    [System.Serializable]
    public class Modifier : LevelData
    {
        /// <summary>
        /// 属性加成数值
        /// </summary>
        public CharacterData.Stats boosts;
    }

    /// <summary>
    /// 初始化被动技能，用于动态创建的被动技能实例的初始化设置
    /// </summary>
    /// <param name="data">被动技能数据，包含技能的基础属性和等级信息</param>
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    /// <summary>
    /// 获取当前被动技能提供的属性加成
    /// </summary>
    /// <returns>当前的属性加成数值</returns>
    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    /// <summary>
    /// 将被动技能等级提升1级，并重新计算对应的属性加成
    /// </summary>
    /// <returns>升级成功返回true，如果已达到最高等级则返回false</returns>
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // 如果已经到达最高等级，则阻止继续升级
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("无法将 {0} 升级到 {1} 级，已达到最高等级 {2}。", name, currentLevel, data.maxLevel));
            return false;
        }

        // 否则，将下一级的属性加成添加到当前被动技能上
        currentBoosts += ((Modifier)data.GetLevelData(++currentLevel)).boosts;
        return true;
    }
}
