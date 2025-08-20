using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 玩家物品栏管理类，用于管理武器和被动道具的添加、移除、升级等操作。
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    /// <summary>
    /// 物品槽位类，用于表示一个物品槽位，包含物品和对应的UI图像。
    /// </summary>
    [System.Serializable]
    public class Slot
    {
        public Item item;       // 当前槽位中的物品
        public Image image;     // 槽位对应的UI图像

        /// <summary>
        /// 将指定物品分配到该槽位，并更新UI图像。
        /// </summary>
        /// <param name="assignedItem">要分配的物品</param>
        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player.", item.name));
        }

        /// <summary>
        /// 清空该槽位，移除物品并隐藏UI图像。
        /// </summary>
        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }
        
        /// <summary>
        /// 判断该槽位是否为空。
        /// </summary>
        /// <returns>如果槽位为空则返回true，否则返回false</returns>
        public bool IsEmpty() { return item == null; }
    }
    
    public List<Slot> weaponSlots = new List<Slot>(6);      // 武器槽位列表
    public List<Slot> passiveSlots = new List<Slot>(6);     // 被动道具槽位列表

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();      // 可用的武器升级选项
    public List<PassiveData> availablePassives = new List<PassiveData>();   // 可用的被动道具升级选项
    public UIUpgradeWindow upgradeWindow;

    private PlayerStats player; // 玩家状态组件引用

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    /// <summary>
    /// 检查物品栏中是否包含指定类型的物品。
    /// </summary>
    /// <param name="type">要检查的物品数据类型</param>
    /// <returns>如果包含该类型物品则返回true，否则返回false</returns>
    public bool Has(ItemData type) { return Get(type); }
    
    /// <summary>
    /// 根据物品数据类型获取对应的物品实例。
    /// </summary>
    /// <param name="type">物品数据类型</param>
    /// <returns>找到的物品实例，未找到则返回null</returns>
    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    /// <summary>
    /// 在物品栏中查找指定类型的被动道具。
    /// </summary>
    /// <param name="type">被动道具数据类型</param>
    /// <returns>找到的被动道具实例，未找到则返回null</returns>
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            if (s.item != null)
            {
                Passive p = s.item as Passive;
                if (p && p.data == type)
                    return p;
            }
        }
        return null;
    }

    /// <summary>
    /// 在物品栏中查找指定类型的武器。
    /// </summary>
    /// <param name="type">武器数据类型</param>
    /// <returns>找到的武器实例，未找到则返回null</returns>
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            if (s.item != null)
            {
                Weapon w = s.item as Weapon;
                if (w && w.data == type)
                    return w;
            }
        }
        return null;
    }


    /// <summary>
    /// 从物品栏中移除指定类型的武器。
    /// </summary>
    /// <param name="data">要移除的武器数据</param>
    /// <param name="removeUpgradeAvailability">是否同时从可升级列表中移除</param>
    /// <returns>移除成功返回true，否则返回false</returns>
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        // 从可升级池中移除该武器
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 从物品栏中移除指定类型的被动道具。
    /// </summary>
    /// <param name="data">要移除的被动道具数据</param>
    /// <param name="removeUpgradeAvailability">是否同时从可升级列表中移除</param>
    /// <returns>移除成功返回true，否则返回false</returns>
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // 从可升级池中移除该被动道具
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < passiveSlots.Count; i++) // 修正为passiveSlots
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear(); // 修正为passiveSlots
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// 根据物品数据类型移除对应的物品。
    /// </summary>
    /// <param name="data">要移除的物品数据</param>
    /// <param name="removeUpgradeAvailability">是否同时从可升级列表中移除</param>
    /// <returns>移除成功返回true，否则返回false</returns>
    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        return false;
    }

    /// <summary>
    /// 向物品栏中添加指定类型的武器，返回添加的槽位索引。
    /// </summary>
    /// <param name="data">要添加的武器数据</param>
    /// <returns>添加成功的槽位索引，失败返回-1</returns>
    public int Add(WeaponData data)
    {
        int slotNum = -1;

        // 尝试找到一个空槽位
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // 如果没有空槽位，退出
        if (slotNum < 0) return slotNum;

        // 否则在槽位中创建武器
        // 获取要生成的武器类型
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            // 生成武器游戏对象
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(transform); // 设置武器为玩家的子对象
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            // 将武器分配到槽位
            weaponSlots[slotNum].Assign(spawnedWeapon);

            // 如果正在选择升级，关闭升级UI
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format(
                "Invalid weapon type specified for {0}.",
                data.name
            ));
        }

        return -1;
    }
    
    /// <summary>
    /// 向物品栏中添加指定类型的被动道具，返回添加的槽位索引。
    /// </summary>
    /// <param name="data">要添加的被动道具数据</param>
    /// <returns>添加成功的槽位索引，失败返回-1</returns>
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // 尝试找到一个空槽位
        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // 如果没有空槽位，退出
        if (slotNum < 0) return slotNum;

        // 否则在槽位中创建被动道具
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); // 设置被动道具为玩家的子对象
        p.transform.localPosition = Vector2.zero;

        // 将被动道具分配到槽位
        passiveSlots[slotNum].Assign(p);

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }
    
    /// <summary>
    /// 根据物品数据类型添加对应的物品。
    /// </summary>
    /// <param name="data">要添加的物品数据</param>
    /// <returns>添加成功的槽位索引，失败返回-1</returns>
    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        return -1;
    }
    
    
    /// <summary>
    /// 应用升级选项，根据当前库存情况筛选出可用的升级项并显示升级窗口。
    /// </summary>
    void ApplyUpgradeOptions()
    {
        // <availableUpgrades> 是一个空列表，将从<allUpgrades>中过滤出来，
        // <allUpgrades>是PlayerInventory中所有升级的列表。
        // 并非所有升级都可以应用，因为有些可能已经达到了玩家的最大值，
        // 或者玩家没有足够的库存槽位。
        List<ItemData> availableUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        // 我们需要知道还剩下多少武器/被动槽位。
        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        // 过滤可用的武器和被动，并添加那些
        // 可能成为选项的。
        foreach (ItemData data in allUpgrades)
        {
            // 如果存在这种类型的武器，则允许升级如果
            // 武器的等级尚未达到最大值。
            Item obj = Get(data);
            if (obj)
            {
                if (obj.currentLevel < data.maxLevel) availableUpgrades.Add(data);
            }
            else
            {
                // 如果我们还没有这个物品在库存中，检查是否
                // 我们仍然有足够的槽位来接受新物品。
                if (data is WeaponData && weaponSlotsLeft > 0) availableUpgrades.Add(data);
                else if (data is PassiveData && passiveSlotsLeft > 0) availableUpgrades.Add(data);
            }
        }
        
        // 如果我们还有可用的升级，则显示UI升级窗口。
        int availUpgradeCount = availableUpgrades.Count;
        if (availUpgradeCount > 0)
        {
            bool getExtraItem = 1f - 1f / player.Stats.luck > UnityEngine.Random.value;
            if (getExtraItem || availUpgradeCount < 4) upgradeWindow.SetUpgrades(this, availableUpgrades, 4);
            else upgradeWindow.SetUpgrades(this, availableUpgrades, 3, "提高你的幸运属性以有机会获得4个物品！");
        }
        else if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    
    /// <summary>
    /// 检查槽位列表，查看此列表中是否有任何空的物品槽位。
    /// </summary>
    /// <param name="slots">要检查的槽位列表</param>
    /// <returns>空槽位的数量</returns>
    private int GetSlotsLeft(List<Slot> slots)
    {
        int count = 0;
        foreach (Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    /// <summary>
    /// 在玩家库存中升级选定的物品。
    /// </summary>
    /// <param name="item">要升级的物品实例</param>
    /// <returns>升级成功返回true，否则返回false</returns>
    public bool LevelUp(Item item)
    {
        // 尝试升级物品。
        if (!item.DoLevelUp())
        {
            Debug.LogWarning(string.Format(
                "Failed to level up {0}.",
                item.name
            ));
            return false;
        }

        // 随后关闭升级屏幕。
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        // 如果是被动技能，重新计算玩家属性。
        if (item is Passive) player.RecalculateStats();

        return true;
    }
    
    /// <summary>
    /// 根据物品数据类型升级库存中的物品。
    /// </summary>
    /// <param name="data">要升级的物品数据</param>
    /// <returns>升级成功返回true，否则返回false</returns>
    public bool LevelUp(ItemData data)
    {
        Item item = Get(data);
        if (item) return LevelUp(item);
        return false;
    }

    /// <summary>
    /// 移除并重新应用升级选项。
    /// </summary>
    public void RemoveAndApplyUpgrades()
    {
        ApplyUpgradeOptions();
    }
}
