using UnityEngine;
    
namespace InventorySystemVideo
{
    /// <summary>
    /// 玩家类，负责管理玩家的物品库存系统
    /// 继承自Unity的MonoBehaviour组件
    /// </summary>
    public class IPlayer : MonoBehaviour
    {
        private IInventory inventory;

        /// <summary>
        /// 在对象唤醒时初始化库存系统
        /// 创建一个新的库存实例
        /// </summary>
        private void Awake()
        {
            inventory = new IInventory();
        }

        /// <summary>
        /// 向玩家库存中添加物品
        /// </summary>
        /// <param name="item">要添加的物品对象，类型为IItem接口</param>
        public void AddItem(IItem item)
        {
            // 检查物品是否为空，避免空引用异常
            if (item == null)
            {
                return;
            }
            inventory.Add(item);
        }

        /// <summary>
        /// 每帧更新检测玩家输入
        /// 当按下Tab键时，在控制台输出所有物品名称
        /// </summary>
        private void Update()
        {
            // 检测Tab键按下事件
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // 确保库存对象存在
                if (inventory != null)
                {
                    // 遍历库存中的所有物品并输出名称
                    foreach (var item in inventory)
                    {
                        Debug.Log(item.Name);
                    }
                }
            }
        }
    }
}