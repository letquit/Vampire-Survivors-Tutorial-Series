using System;
using UnityEngine;

namespace InventorySystemVideo.Test
{
    /// <summary>
    /// 库存系统测试类，用于演示库存系统的功能
    /// </summary>
    public class IInventoryTest : MonoBehaviour
    {
        [SerializeField] private string itemName1;
        [SerializeField] private ItemType itemType1;
        [SerializeField] private Sprite itemIcon1;
        
        [Space]
        
        [SerializeField] private string itemName2;
        [SerializeField] private ItemType itemType2;
        [SerializeField] private Sprite itemIcon2;
        
        [Space]
        
        [SerializeField] private string itemName3;
        [SerializeField] private ItemType itemType3;
        [SerializeField] private Sprite itemIcon3;
        
        private IInventory inventory;

        /// <summary>
        /// Unity生命周期方法，在游戏对象启动时执行
        /// 负责初始化库存系统并添加测试物品
        /// </summary>
        private void Start()
        {
            // 创建三个测试物品
            IItem item1 = new IItem(itemName1, itemType1, itemIcon1);
            IItem item2 = new IItem(itemName2, itemType2, itemIcon2);
            IItem item3 = new IItem(itemName3, itemType3, itemIcon3);
            
            // 初始化库存并添加物品
            inventory = new IInventory();
            inventory.Add(item1);
            inventory.Add(item2);
            inventory.Add(item3);

            // 遍历库存中的所有物品并打印名称
            foreach (IItem item in inventory)
            {
                print(item.Name);
            }
        }
    }
}