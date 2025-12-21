using UnityEngine;

namespace InventorySystemVideo
{
    /// <summary>
    /// 物品类别，用于表示游戏中的各种物品
    /// </summary>
    public class IItem
    {
        private string name;
        private ItemType itemType;
        private Sprite sprite;

        /// <summary>
        /// 初始化一个新的物品实例
        /// </summary>
        /// <param name="name">物品的名称</param>
        /// <param name="itemType">物品的类型</param>
        /// <param name="sprite">物品的精灵图像</param>
        public IItem(string name, ItemType itemType, Sprite sprite)
        {
            Name = name;
            ItemType = itemType;
            Sprite = sprite;
        }
        
        /// <summary>
        /// 获取或设置物品的名称
        /// </summary>
        public string Name { get => name; set => name = value; }
        
        /// <summary>
        /// 获取或设置物品的类型
        /// </summary>
        public ItemType ItemType { get => itemType; set => itemType = value; }
        
        /// <summary>
        /// 获取或设置物品的精灵图像
        /// </summary>
        public Sprite Sprite { get => sprite; set => sprite = value; }
    }
    
    /// <summary>
    /// 物品类型的枚举，定义了游戏中物品的分类
    /// </summary>
    public enum ItemType
    {
        WEAPON,
        FOOD,
        ARMOR
    }
}