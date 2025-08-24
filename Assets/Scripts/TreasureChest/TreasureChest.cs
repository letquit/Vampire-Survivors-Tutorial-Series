using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝箱类，用于处理玩家与宝箱的交互，提供掉落奖励。
/// </summary>
public class TreasureChest : MonoBehaviour
{
    /// <summary>
    /// 宝箱可能掉落的奖励类型，使用位标志表示。
    /// </summary>
    [System.Flags]
    public enum DropType
    {
        NewPassive = 1,     // 新的被动道具
        NewWeapon = 2,      // 新的武器
        UpgradePassive = 4, // 升级被动道具
        UpgradeWeapon = 8,  // 升级武器
        Evolution = 16      // 进化物品
    }

    /// <summary>
    /// 可能掉落的类型集合，默认为所有类型。
    /// </summary>
    public DropType possibleDrops = (DropType)~0;

    /// <summary>
    /// 掉落数量的类型：顺序或随机。
    /// </summary>
    public enum DropCountType { sequential, random }

    /// <summary>
    /// 当前使用的掉落数量策略。
    /// </summary>
    public DropCountType dropCountType = DropCountType.sequential;

    /// <summary>
    /// 宝箱使用的掉落配置文件数组。
    /// </summary>
    public TreasureChestDropProfile[] dropProfiles;

    /// <summary>
    /// 所有宝箱累计被拾取的次数。
    /// </summary>
    public static int totalPickups = 0;

    /// <summary>
    /// 当前使用的掉落配置文件索引。
    /// </summary>
    int currentDropProfileIndex = 0;

    /// <summary>
    /// 接收奖励的玩家库存。
    /// </summary>
    PlayerInventory recipient;

    /// <summary>
    /// 获取当前宝箱提供的奖励数量。
    /// </summary>
    /// <returns>奖励数量。</returns>
    private int GetRewardCount()
    {
        TreasureChestDropProfile dp = GetNextDropProfile();
        if(dp) return dp.noOfItems;
        return 1;
    }

    /// <summary>
    /// 获取当前使用的掉落配置文件。
    /// </summary>
    /// <returns>当前掉落配置文件。</returns>
    public TreasureChestDropProfile GetCurrentDropProfile()
    {
        return dropProfiles[currentDropProfileIndex];
    }

    /// <summary>
    /// 根据掉落策略获取下一个掉落配置文件。
    /// </summary>
    /// <returns>下一个掉落配置文件。</returns>
    public TreasureChestDropProfile GetNextDropProfile()
    {
        if (dropProfiles == null || dropProfiles.Length == 0)
        {
            Debug.LogWarning("掉落配置文件未设置。");
            return null;
        }

        switch (dropCountType)
        {
            case DropCountType.sequential:
                currentDropProfileIndex = Mathf.Clamp(
                    totalPickups, 0,
                    dropProfiles.Length - 1
                );
                break;

            case DropCountType.random:

                float playerLuck = recipient.GetComponentInChildren<PlayerStats>().Actual.luck;

                // 构建包含计算权重的配置文件列表
                List<(int index, TreasureChestDropProfile profile, float weight)> weightedProfiles = new List<(int, TreasureChestDropProfile, float)>();
                for (int i = 0; i < dropProfiles.Length; i++)
                {
                    float weight = dropProfiles[i].baseDropChance * (1 + dropProfiles[i].luckScaling * (playerLuck - 1));
                    weightedProfiles.Add((i, dropProfiles[i], weight));
                }

                // 按权重升序排序（从小到大）
                weightedProfiles.Sort((a, b) => a.weight.CompareTo(b.weight));

                // 计算总权重
                float totalWeight = 0f;
                foreach (var entry in weightedProfiles)
                    totalWeight += entry.weight;

                // 随机抽取并进行累积选择
                float r = Random.Range(0, totalWeight);
                float cumulative = 0f;
                foreach (var entry in weightedProfiles)
                {
                    cumulative += entry.weight;
                    if (r <= cumulative)
                    {
                        currentDropProfileIndex = entry.index;
                        return entry.profile;
                    }
                }
                break;
        }

        return GetCurrentDropProfile();
    }

    /// <summary>
    /// 当玩家进入宝箱触发区域时，给予奖励并激活UI。
    /// </summary>
    /// <param name="col">触发器碰撞体。</param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out PlayerInventory p))
        {
            // 保存接收者并启动UI。
            recipient = p;

            // 先给予奖励。
            int rewardCount = GetRewardCount();
            for (int i = 0; i < rewardCount; i++)
            {
                Open(p);
            }
            gameObject.SetActive(false);

            UITreasureChest.Activate(p.GetComponentInChildren<PlayerCollector>(), this);

            // 先递增，然后在必要时回绕
            totalPickups = (totalPickups + 1) % (dropProfiles.Length + 1);
        }
    }

    /// <summary>
    /// 通知奖励发放完成，刷新UI。
    /// </summary>
    public void NotifyComplete() {
        recipient.weaponUI.Refresh();
        recipient.passiveUI.Refresh();
    }
    
    /// <summary>
    /// 根据可能掉落类型，依次尝试给予奖励。
    /// </summary>
    /// <param name="inventory">玩家库存。</param>
    void Open(PlayerInventory inventory)
    {
        if (inventory == null) return;

        if (possibleDrops.HasFlag(DropType.Evolution) && TryEvolve<Weapon>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.UpgradeWeapon) && TryUpgrade<Weapon>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.UpgradePassive) && TryUpgrade<Passive>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.NewWeapon) && TryGive<WeaponData>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.NewPassive)) TryGive<PassiveData>(inventory, false);
    }

    /// <summary>
    /// 尝试进化玩家库存中的物品。
    /// </summary>
    /// <typeparam name="T">物品类型。</typeparam>
    /// <param name="inventory">玩家库存。</param>
    /// <param name="updateUI">是否更新UI。</param>
    /// <returns>成功进化的物品，失败则返回null。</returns>
    T TryEvolve<T>(PlayerInventory inventory, bool updateUI = true) where T : Item
    {
        // 遍历每个可进化的物品。
        T[] evolvables = inventory.GetEvolvables<T>();
        foreach (Item i in evolvables)
        {
            // 获取所有可能的进化。
            ItemData.Evolution[] possibleEvolutions = i.CanEvolve(0);
            foreach (ItemData.Evolution e in possibleEvolutions)
            {
                // 尝试进化，如果成功则通知UI。
                if (i.AttemptEvolution(e, 0, updateUI))
                {
                    UITreasureChest.NotifyItemReceived(e.outcome.itemType.icon);
                    return i as T;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 尝试升级玩家库存中的物品。
    /// </summary>
    /// <typeparam name="T">物品类型。</typeparam>
    /// <param name="inventory">玩家库存。</param>
    /// <param name="updateUI">是否更新UI。</param>
    /// <returns>成功升级的物品，失败则返回null。</returns>
    T TryUpgrade<T>(PlayerInventory inventory, bool updateUI = true) where T : Item
    {
        // 获取库存中仍可升级的所有武器。
        T[] upgradables = inventory.GetUpgradables<T>();
        if (upgradables.Length == 0) return null; // 如果没有武器则终止。

        // 执行升级，并告诉宝箱哪个物品已升级。
        T t = upgradables[Random.Range(0, upgradables.Length)];
        inventory.LevelUp(t, updateUI);
        UITreasureChest.NotifyItemReceived(t.data.icon);
        return t;
    }

    /// <summary>
    /// 尝试向玩家库存中添加新物品。
    /// </summary>
    /// <typeparam name="T">物品数据类型。</typeparam>
    /// <param name="inventory">玩家库存。</param>
    /// <param name="updateUI">是否更新UI。</param>
    /// <returns>成功添加的物品，失败则返回null。</returns>
    T TryGive<T>(PlayerInventory inventory, bool updateUI = true) where T : ItemData
    {
        // 获取所有新的物品可能性。
        T[] possibilities = inventory.GetUnowned<T>();
        if (possibilities.Length == 0) return null;
        

        // 添加一个随机的可能性。
        T t = possibilities[Random.Range(0, possibilities.Length)];
        inventory.Add(t, updateUI);
        UITreasureChest.NotifyItemReceived(t.icon);
        return t;
    }
}
