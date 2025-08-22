using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 宝箱类，用于处理玩家与宝箱的交互，随机给予玩家武器进化奖励
/// </summary>
public class TreasureChest : MonoBehaviour
{
    /// <summary>
    /// 当碰撞体进入触发器时调用此方法
    /// </summary>
    /// <param name="col">进入触发器的碰撞体</param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 获取碰撞对象的玩家背包组件
        PlayerInventory p = col.GetComponent<PlayerInventory>();
        if (p)
        {
            // 随机生成布尔值决定是否给予高级奖励
            bool randomBool = Random.Range(0, 2) == 0;

            // 打开宝箱并销毁宝箱对象
            OpenTreasureChest(p, randomBool);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 打开宝箱，尝试进化玩家武器
    /// </summary>
    /// <param name="inventory">玩家背包，用于获取可进化的武器</param>
    /// <param name="isHigherTier">是否为高级奖励，决定进化概率</param>
    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        // 遍历所有武器槽位，检查武器是否可以进化
        foreach (PlayerInventory.Slot s in inventory.weaponSlots)
        {
            // 检查槽位是否为空
            if (s.IsEmpty()) continue;
            
            Weapon w = s.item as Weapon;
            // 检查武器和武器数据是否存在
            if (w == null || w.data == null || w.data.evolutionData == null) continue; // 如果武器无法进化则跳过

            // 遍历该武器的所有可能进化路径
            foreach (ItemData.Evolution e in w.data.evolutionData)
            {
                // 只尝试通过宝箱条件进化的武器
                if (e.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    // 尝试进化武器，使用0作为基础概率参数
                    bool attempt = w.AttemptEvolution(e, 0);
                    if (attempt) return; // 如果进化成功则停止处理
                }
            }
        }
    }
}
