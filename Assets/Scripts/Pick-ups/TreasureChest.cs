
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 宝箱类，用于处理玩家与宝箱的交互，随机给予玩家武器进化奖励
/// </summary>
public class TreasureChest : MonoBehaviour
{
    private InventoryManager inventory;

    /// <summary>
    /// 初始化宝箱，获取游戏中的库存管理器实例
    /// </summary>
    private void Start()
    {
        inventory = FindFirstObjectByType<InventoryManager>();
    }
    
    /// <summary>
    /// 当碰撞体触发时调用，检测是否与玩家发生碰撞
    /// </summary>
    /// <param name="collision">触发碰撞的碰撞体对象</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否为玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            OpenTreasureChest();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 打开宝箱，随机选择一个可进化的武器并执行进化
    /// </summary>
    private void OpenTreasureChest()
    {
        // 检查是否有可用的武器进化选项
        if (inventory.GetPossibleEvolutions().Count <= 0)
        {
            Debug.LogWarning("No Available Evolutions");
            return;
        }
        
        // 从可进化列表中随机选择一个武器进化蓝图
        WeaponEvolutionBlueprint toEvolve =
            inventory.GetPossibleEvolutions()[Random.Range(0, inventory.GetPossibleEvolutions().Count)];
        inventory.EvolveWeapon(toEvolve);
    }
}

