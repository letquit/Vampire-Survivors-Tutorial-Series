using System;
using UnityEngine;

namespace InventorySystemVideo
{
    /// <summary>
    /// 物品游戏对象类，用于在场景中表示可拾取的物品
    /// </summary>
    public class IItemGameObject : MonoBehaviour
    {
        [SerializeField] private IItemData itemData;

        /// <summary>
        /// 在对象唤醒时初始化组件
        /// 设置精灵渲染器的图标为物品数据中的图标
        /// </summary>
        private void Awake()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = itemData.icon;
        }

        /// <summary>
        /// 当触发器碰撞发生时调用此方法
        /// 检测是否与玩家发生碰撞，如果是则将物品添加到玩家背包中
        /// </summary>
        /// <param name="collider">与当前对象发生碰撞的碰撞器</param>
        private void OnTriggerEnter2D(Collider2D collider)
        {
            // 检查碰撞的对象是否为玩家组件
            if (collider.TryGetComponent(out IPlayer player))
            {
                // 获取物品的基本信息
                string itemName = itemData.name;
                Sprite sprite = itemData.icon;
                ItemType itemType = itemData.itemType;
                
                // 创建新的物品实例并添加到玩家背包
                IItem item = new IItem(itemName, itemType, sprite);
                player.AddItem(item);
                
                // 隐藏当前物品对象
                gameObject.SetActive(false);
            }
        }
    }   
}