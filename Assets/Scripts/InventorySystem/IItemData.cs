using UnityEngine;

namespace InventorySystemVideo
{
    /// <summary>
    /// 物品数据类，用于定义游戏中的物品基础信息
    /// 继承自ScriptableObject，可以在Unity编辑器中创建独立的资产文件
    /// </summary>
    [CreateAssetMenu(menuName = "Inventory/Item Data")]
    public class IItemData : ScriptableObject
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        public string itemName;
        
        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType itemType;
        
        /// <summary>
        /// 物品图标
        /// </summary>
        public Sprite icon;
    }   
}