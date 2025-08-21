using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIInventoryIconsDisplay 类用于显示玩家库存中的物品图标和等级信息。
/// 该组件会根据指定的库存列表自动更新UI插槽中的图标和等级文本。
/// </summary>
[RequireComponent(typeof(LayoutGroup))]
public class UIInventoryIconsDisplay : MonoBehaviour
{
    /// <summary>
    /// 插槽的预制体模板，用于实例化多个插槽。
    /// </summary>
    public GameObject slotTemplate;

    /// <summary>
    /// 最大插槽数量，默认为6。
    /// </summary>
    public uint maxSlots = 6;

    /// <summary>
    /// 是否显示物品等级，默认为true。
    /// </summary>
    public bool showLevels = true;

    /// <summary>
    /// 关联的玩家库存对象。
    /// </summary>
    public PlayerInventory inventory;

    /// <summary>
    /// 所有插槽的游戏对象数组。
    /// </summary>
    public GameObject[] slots;

    /// <summary>
    /// 图标在插槽中的查找路径。
    /// </summary>
    [Header("Paths")] public string iconPath;

    /// <summary>
    /// 等级文本在插槽中的查找路径。
    /// </summary>
    public string levelTextPath;

    /// <summary>
    /// 目标库存列表字段名称（通过反射获取）。
    /// </summary>
    [HideInInspector] public string targetedItemList;

    /// <summary>
    /// 在Inspector中重置组件时调用，初始化slotTemplate和inventory引用。
    /// </summary>
    void Reset()
    {
        slotTemplate = transform.GetChild(0).gameObject;
        inventory = FindObjectOfType<PlayerInventory>();
    }

    /// <summary>
    /// 当组件启用时调用，刷新UI显示内容。
    /// </summary>
    void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// 刷新UI显示内容，读取库存并更新所有插槽的图标和等级。
    /// </summary>
    public void Refresh()
    {
        if (!inventory) Debug.LogWarning("没有附加到UI图标显示的库存。");

        // 确定要读取的库存字段
        Type t = typeof(PlayerInventory);
        FieldInfo field = t.GetField(targetedItemList, BindingFlags.Public | BindingFlags.Instance);

        // 如果字段未找到则警告并返回
        if (field == null)
        {
            Debug.LogWarning("库存中的列表未找到。");
            return;
        }

        // 获取库存插槽列表
        List<PlayerInventory.Slot> items = (List<PlayerInventory.Slot>)field.GetValue(inventory);

        // 遍历物品列表并填充UI插槽
        for (int i = 0; i < items.Count; i++)
        {
            // 检查是否有足够的UI插槽
            if (i >= slots.Length)
            {
                Debug.LogWarning(
                    string.Format(
                        "你有 {0} 个库存插槽，但UI上只有 {1} 个插槽。",
                        items.Count, slots.Length
                    )
                );
                break;
            }

            // 获取当前物品数据
            Item item = items[i].item;

            Transform iconObj = slots[i].transform.Find(iconPath);
            if (iconObj)
            {
                Image icon = iconObj.GetComponentInChildren<Image>();

                // 根据物品是否存在设置图标透明度
                if (!item) icon.color = new Color(1, 1, 1, 0);
                else
                {
                    // 显示图标并更新精灵
                    icon.color = new Color(1, 1, 1, 1);
                    if (icon) icon.sprite = item.data.icon;
                }
            }

            // 更新等级文本显示
            Transform levelObj = slots[i].transform.Find(levelTextPath);
            if (levelObj)
            {
                // 找到文本组件并设置等级值
                TextMeshProUGUI levelTxt = levelObj.GetComponentInChildren<TextMeshProUGUI>();
                if (levelTxt)
                {
                    if (!item || !showLevels) levelTxt.text = "";
                    else levelTxt.text = item.currentLevel.ToString();
                }
            }
        }
    }
}
