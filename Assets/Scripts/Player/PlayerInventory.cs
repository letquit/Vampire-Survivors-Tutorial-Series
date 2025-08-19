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

    /// <summary>
    /// 升级UI类，用于表示升级选项的UI元素。
    /// </summary>
    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;         // 升级名称显示文本
        public TMP_Text upgradeDescriptionDisplay;  // 升级描述显示文本
        public Image upgradeIcon;                   // 升级图标
        public Button upgradeButton;                // 升级按钮
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();      // 可用的武器升级选项
    public List<PassiveData> availablePassives = new List<PassiveData>();   // 可用的被动道具升级选项
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();        // 场景中的升级UI选项列表

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
            if (s.item != null)  // 添加空值检查
            {
                Passive p = s.item as Passive;
                if (p != null && p.data == type)
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
            if (s.item != null)  // 添加空值检查
            {
                Weapon w = s.item as Weapon;
                if (w != null && w.data == type)
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
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform); // 设置武器为玩家的子对象
            spawnedWeapon.transform.localPosition = Vector2.zero;
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
    /// 升级指定槽位的武器。
    /// </summary>
    /// <param name="slotIndex">武器槽位索引</param>
    /// <param name="upgradeIndex">升级选项索引（未使用）</param>
    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            // 如果武器已达到最高等级，则不升级
            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format(
                    "Failed to level up {0}.",
                    weapon.name
                ));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    
    /// <summary>
    /// 升级指定槽位的被动道具。
    /// </summary>
    /// <param name="slotIndex">被动道具槽位索引</param>
    /// <param name="upgradeIndex">升级选项索引（未使用）</param>
    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;
            if (!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format(
                    "Failed to level up {0}.",
                    p.name
                ));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }
    
    /// <summary>
    /// 应用升级选项，确定应该显示哪些升级选项。
    /// </summary>
    private void ApplyUpgradeOptions()
    {
        // 复制可用的武器/被动道具升级列表，以便在函数中迭代
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveItemUpgrades = new List<PassiveData>(availablePassives);

        // 遍历每个升级UI槽位
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // 如果没有更多可用升级，则退出
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
                return;

            // 确定此升级应该是被动道具还是主动武器
            int upgradeType;
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                // 随机生成1到2之间的数字
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            // 生成主动武器升级
            if (upgradeType == 1)
            {
                // 选择一个武器升级，然后移除它以避免重复选择
                WeaponData chosenWeaponUpgrade =
                    availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                // 确保选择的武器数据有效
                if (chosenWeaponUpgrade != null)
                {
                    // 启用UI槽位
                    EnableUpgradeUI(upgradeOption);

                    // 遍历所有现有武器。如果找到匹配项，我们将
                    // 在按钮上挂钩事件监听器，当点击此升级选项时升级武器
                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            // 如果武器已达到最高等级，则不允许升级
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            // 设置事件监听器，物品和等级描述为下一级的数据
                            upgradeOption.upgradeButton.onClick.AddListener(() =>
                                LevelUpWeapon(i, i)); // 应用按钮功能
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    // 如果执行到这里，意味着我们将添加新武器，而不是升级现有武器
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() =>
                            Add(chosenWeaponUpgrade)); // 应用按钮功能
                        upgradeOption.upgradeDescriptionDisplay.text =
                            chosenWeaponUpgrade.baseStats.description; // 应用初始描述
                        upgradeOption.upgradeNameDisplay.text =
                            chosenWeaponUpgrade.baseStats.name; // 应用初始名称
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if (upgradeType == 2)
            {
                // 注意：我们必须重新编码此系统，因为现在如果遇到已达到最高等级的武器，
                // 它会禁用升级槽位
                PassiveData chosenPassiveUpgrade =
                    availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    // 启用UI槽位
                    EnableUpgradeUI(upgradeOption);

                    // 遍历所有现有被动道具。如果找到匹配项，我们将
                    // 在按钮上挂钩事件监听器，当点击此升级选项时升级道具
                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            // 如果被动道具已达到最高等级，则不允许升级
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() =>
                                LevelUpPassiveItem(i, i)); // 应用按钮功能
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }
                    
                    if (!isLevelUp) // 生成新的被动道具
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade)); // 应用按钮功能
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description; // 应用初始描述
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name; // 应用初始名称
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 移除所有升级选项的事件监听器并禁用UI。
    /// </summary>
    private void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); // 调用DisableUpgradeUI方法禁用所有UI选项
        }
    }

    /// <summary>
    /// 移除并重新应用升级选项。
    /// </summary>
    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    /// <summary>
    /// 禁用指定的升级UI。
    /// </summary>
    /// <param name="ui">要禁用的升级UI</param>
    private void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 启用指定的升级UI。
    /// </summary>
    /// <param name="ui">要启用的升级UI</param>
    private void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
