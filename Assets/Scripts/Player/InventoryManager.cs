using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理玩家的武器和被动道具库存，包括添加、升级以及UI显示更新。
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

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
    }

    /// <summary>
    /// 将指定槽位的武器升级到下一级。
    /// 若无下一级预设则输出错误日志。
    /// </summary>
    /// <param name="slotIndex">要升级武器的槽位索引。</param>
    public void LevelUpWeapon(int slotIndex)
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
        }
    }
    
    /// <summary>
    /// 将指定槽位的被动道具升级到下一级。
    /// 若无下一级预设则输出错误日志。
    /// </summary>
    /// <param name="slotIndex">要升级被动道具的槽位索引。</param>
    public void LevelUpPassiveItem(int slotIndex)
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
        }
    }
}
