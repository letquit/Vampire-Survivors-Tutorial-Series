using UnityEngine;

/// <summary>
/// 所有武器/被动物品的基类。使用基类是为了让WeaponData和PassiveItemData能够在需要时可以互换使用。
/// </summary>
public abstract class ItemData : ScriptableObject
{
    /// <summary>
    /// 物品图标
    /// </summary>
    public Sprite icon;
    
    /// <summary>
    /// 物品最大等级
    /// </summary>
    public int maxLevel;
    
    /// <summary>
    /// 进化配置结构体
    /// </summary>
    [System.Serializable]
    public struct Evolution
    {
        /// <summary>
        /// 进化名称
        /// </summary>
        public string name;
        
        /// <summary>
        /// 进化条件枚举
        /// </summary>
        public enum Condition { auto, treasureChest }
        
        /// <summary>
        /// 进化条件
        /// </summary>
        public Condition condition;

        /// <summary>
        /// 消耗类型标记枚举
        /// </summary>
        [System.Flags] public enum Consumption { passives = 1, weapons = 2 }
        
        /// <summary>
        /// 进化消耗的物品类型
        /// </summary>
        public Consumption consumes;

        /// <summary>
        /// 进化所需等级
        /// </summary>
        public int evolutionLevel;
        
        /// <summary>
        /// 进化催化剂配置数组
        /// </summary>
        public Config[] catalysts;
        
        /// <summary>
        /// 进化结果配置
        /// </summary>
        public Config outcome;

        /// <summary>
        /// 配置结构体
        /// </summary>
        [System.Serializable]
        public struct Config
        {
            /// <summary>
            /// 物品类型
            /// </summary>
            public ItemData itemType;
            
            /// <summary>
            /// 物品等级
            /// </summary>
            public int level;
        }
    }

    /// <summary>
    /// 进化数据数组
    /// </summary>
    public Evolution[] evolutionData;
    
    /// <summary>
    /// 获取指定等级的数据信息
    /// </summary>
    /// <param name="level">目标等级</param>
    /// <returns>对应等级的数据信息</returns>
    public abstract Item.LevelData GetLevelData(int level);
}
