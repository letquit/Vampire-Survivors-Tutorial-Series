using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystemVideo
{
    /// <summary>
    /// 库存系统类，用于管理物品集合
    /// 实现IEnumerable接口，支持遍历操作
    /// </summary>
    public class IInventory : IEnumerable<IItem>
    {
        /// <summary>
        /// 存储物品的集合
        /// </summary>
        private ICollection<IItem> Items { get; set; }
        
        /// <summary>
        /// 初始化库存实例，创建空的物品列表
        /// </summary>
        public IInventory()
        {
            Items = new List<IItem>();
        }

        /// <summary>
        /// 向库存中添加物品
        /// </summary>
        /// <param name="item">要添加的物品对象</param>
        public void Add(IItem item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// 获取泛型枚举器，用于遍历库存中的物品
        /// </summary>
        /// <returns>返回IItem类型的枚举器</returns>
        public IEnumerator<IItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// 获取非泛型枚举器，用于遍历库存中的物品
        /// </summary>
        /// <returns>返回IEnumerable类型的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }   
}