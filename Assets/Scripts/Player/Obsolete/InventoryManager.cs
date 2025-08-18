using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 管理玩家的武器和被动道具库存，包括添加、升级以及UI显示更新。
/// </summary>
[Obsolete]
public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    /// <summary>
    /// 武器升级选项的数据结构。
    /// </summary>
    [Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }
    
    /// <summary>
    /// 被动道具升级选项的数据结构。
    /// </summary>
    [Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }
    
    /// <summary>
    /// 升级界面UI元素的数据结构。
    /// </summary>
    [Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }
    
    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    public List<WeaponEvolutionBlueprint> WeaponEvolutions = new List<WeaponEvolutionBlueprint>();
    
    private PlayerStats player;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    /// <summary>
    /// 在指定槽位添加武器，并更新对应的UI显示。
    /// </summary>
    /// <param name="slotIndex">要添加武器的槽位索引。</param>
    /// <param name="weapon">要添加的武器控制器实例。</param>
    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    /// <summary>
    /// 在指定槽位添加被动道具，并更新对应的UI显示。
    /// </summary>
    /// <param name="slotIndex">要添加被动道具的槽位索引。</param>
    /// <param name="passiveItem">要添加的被动道具实例。</param>
    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.level;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    /// <summary>
    /// 将指定槽位的武器升级到下一级。
    /// 若无下一级预设则输出错误日志。
    /// </summary>
    /// <param name="slotIndex">要升级武器的槽位索引。</param>
    /// <param name="upgradeIndex">用于更新升级选项数据的索引。</param>
    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        // 检查槽位是否有效
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];

            // 检查是否存在下一级武器预制体
            if (!weapon.weaponData.nextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL FOR " + weapon.name);
                return;
            }

            // 实例化并替换为升级后的武器
            GameObject upgradedWeapon =
                Instantiate(weapon.weaponData.nextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.level;

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;
            
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }
    
    /// <summary>
    /// 将指定槽位的被动道具升级到下一级。
    /// 若无下一级预设则输出错误日志。
    /// </summary>
    /// <param name="slotIndex">要升级被动道具的槽位索引。</param>
    /// <param name="upgradeIndex">用于更新升级选项数据的索引。</param>
    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        // 检查槽位是否有效
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];

            // 检查是否存在下一级被动道具预制体
            if (!passiveItem.passiveItemData.nextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL FOR " + passiveItem.name);
                return;
            }

            // 实例化并替换为升级后的被动道具
            GameObject upgradedPassiveItem =
                Instantiate(passiveItem.passiveItemData.nextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.level;

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData =
                upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;
            
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    /// <summary>
    /// 根据当前拥有的武器和被动道具，随机生成可选的升级选项，并绑定到UI上。
    /// </summary>
    private void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);
        
        foreach (var upgradeOption in upgradeUIOptions)
        {
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

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
                upgradeType = Random.Range(1, 3);
            }
            
            if (upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];

                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    
                    bool newWeapon = false;
                    
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {
                                if (!chosenWeaponUpgrade.weaponData.nextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }
                                
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex));
                                
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData
                                    .nextLevelPrefab.GetComponent<WeaponController>().weaponData.description;
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData
                                    .nextLevelPrefab.GetComponent<WeaponController>().weaponData.name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }

                    if (newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon));
                        
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.icon;
                }
            }
            else if (upgradeType == 2)
            { 
                PassiveItemUpgrade chosenPassiveItemUpgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)];

                availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade);
                
                if (chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;
                            
                            if (!newPassiveItem)
                            {
                                if (!chosenPassiveItemUpgrade.passiveItemData.nextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }
                                
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex));
                                
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData
                                    .nextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.description;
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData
                                    .nextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.name;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }

                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem));
                        
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.icon;
                }
            }
        }
    }
    
    /// <summary>
    /// 清除所有升级选项UI上的事件监听器，并隐藏UI。
    /// </summary>
    private void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    /// <summary>
    /// 先清除旧的升级选项，再重新生成新的升级选项。
    /// </summary>
    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    /// <summary>
    /// 隐藏指定的升级UI。
    /// </summary>
    /// <param name="ui">要隐藏的升级UI对象。</param>
    private void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 显示指定的升级UI。
    /// </summary>
    /// <param name="ui">要显示的升级UI对象。</param>
    private void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

    /// <summary>
    /// 获取当前可能触发的武器进化蓝图列表。
    /// 匹配条件包括：武器类型、催化剂类型、武器等级精确匹配、催化剂等级大于等于要求。
    /// </summary>
    /// <returns>满足条件的武器进化蓝图列表。</returns>
    public List<WeaponEvolutionBlueprint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBlueprint> possibleEvolutions = new List<WeaponEvolutionBlueprint>();

        foreach (WeaponController weapon in weaponSlots)
        {
            if (weapon != null)
            {
                foreach (PassiveItem catalyst in passiveItemSlots)
                {
                    if (catalyst != null)
                    {
                        foreach (WeaponEvolutionBlueprint evolution in WeaponEvolutions)
                        {
                            // 1. 检查武器的基础类型是否匹配
                            bool weaponTypeMatch = weapon.weaponData.baseWeaponData == evolution.baseWeaponData.baseWeaponData;
                            
                            // 2. 检查被动物品的基础类型是否匹配
                            bool catalystTypeMatch = catalyst.passiveItemData.basePassiveItemData == evolution.catalystPassiveItemData;
                            
                            // 3. 武器等级必须精确匹配蓝图要求（不是大于等于）
                            bool weaponLevelMatch = weapon.weaponData.level == evolution.baseWeaponData.level;
                            
                            // 4. 被动物品等级仍然保持大于等于的判断
                            bool catalystLevelMatch = catalyst.passiveItemData.level >= evolution.catalystPassiveItemData.level;

                            if (weaponTypeMatch && catalystTypeMatch && weaponLevelMatch && catalystLevelMatch)
                            {
                                possibleEvolutions.Add(evolution);
                                Debug.Log($"Found evolution match: Weapon({weapon.weaponData.name} Level {weapon.weaponData.level}) + Catalyst({catalyst.passiveItemData.name} Level {catalyst.passiveItemData.level})");
                            }
                        }
                    }
                }
            }
        }

        return possibleEvolutions;
    }

    /// <summary>
    /// 执行一次武器进化操作。
    /// 查找符合条件的武器与催化剂组合，将其替换为进化后的武器。
    /// </summary>
    /// <param name="evolution">要执行的武器进化蓝图。</param>
    public void EvolveWeapon(WeaponEvolutionBlueprint evolution)
    {
        for (int weaponSlotIndex = 0; weaponSlotIndex < weaponSlots.Count; weaponSlotIndex++)
        {
            WeaponController weapon = weaponSlots[weaponSlotIndex];
            if (!weapon) continue;

            for (int catalystSlotIndex = 0; catalystSlotIndex < passiveItemSlots.Count; catalystSlotIndex++)
            {
                PassiveItem catalyst = passiveItemSlots[catalystSlotIndex];
                if (!catalyst) continue;
                
                bool weaponTypeMatch = weapon.weaponData.baseWeaponData == evolution.baseWeaponData.baseWeaponData;
                bool catalystTypeMatch = catalyst.passiveItemData.basePassiveItemData == evolution.catalystPassiveItemData;
                bool weaponLevelMatch = weapon.weaponData.level == evolution.baseWeaponData.level;
                bool catalystLevelMatch = catalyst.passiveItemData.level >= evolution.catalystPassiveItemData.level;

                if (weaponTypeMatch && catalystTypeMatch && weaponLevelMatch && catalystLevelMatch)
                {
                    GameObject evolvedWeapon = Instantiate(evolution.evolvedWeapon, transform.position, Quaternion.identity);
                    WeaponController evolvedWeaponController = evolvedWeapon.GetComponent<WeaponController>();
                    
                    evolvedWeapon.transform.SetParent(transform);
                    AddWeapon(weaponSlotIndex, evolvedWeaponController);
                    Destroy(weapon.gameObject);

                    weaponLevels[weaponSlotIndex] = evolvedWeaponController.weaponData.level;
                    weaponUISlots[weaponSlotIndex].sprite = evolvedWeaponController.weaponData.icon;
                    
                    weaponUpgradeOptions.RemoveAt(evolvedWeaponController.weaponData.evolvedUpgradeToRemove);
                    
                    Debug.Log($"Successfully evolved {weapon.weaponData.name} (Level {weapon.weaponData.level}) with {catalyst.passiveItemData.name} (Level {catalyst.passiveItemData.level})!");
                    
                    return;
                }
            }
        }
    }
}
