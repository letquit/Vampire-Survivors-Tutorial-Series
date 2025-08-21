using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// 自定义编辑器类，用于在Unity编辑器中扩展UIInventoryIconsDisplay组件的Inspector面板。
    /// 提供了选择物品列表和重新生成图标的功能。
    /// </summary>
    [CustomEditor(typeof(UIInventoryIconsDisplay))]
    public class UIInventoryIconsDisplayEditor : UnityEditor.Editor
    {
        UIInventoryIconsDisplay display;
        int targetedItemListIndex = 0;
        string[] itemListOptions;

        /// <summary>
        /// 当启用该编辑器时被调用。初始化目标对象，并扫描PlayerInventory类中所有类型为List<PlayerInventory.Slot>的字段，
        /// 用于填充下拉菜单选项。
        /// </summary>
        private void OnEnable()
        {
            // 获取对组件的访问权限，因为我们需要设置targetedItemList变量。
            display = target as UIInventoryIconsDisplay;

            // 获取PlayerInventory类的Type对象
            Type playerInventoryType = typeof(PlayerInventory);

            // 获取PlayerInventory类的所有字段
            FieldInfo[] fields = playerInventoryType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // 存储类型为List<PlayerInventory.Slot>的变量列表
            // 使用LINQ过滤类型为List<PlayerInventory.Slot>的字段并选择它们的名称
            List<string> slotListNames = fields
                .Where(field => field.FieldType.IsGenericType &&
                                field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
                                field.FieldType.GetGenericArguments()[0] == typeof(PlayerInventory.Slot))
                .Select(field => field.Name)
                .ToList();

            slotListNames.Insert(0, "None");
            itemListOptions = slotListNames.ToArray();

            // 确保我们使用正确的武器子类型。
            targetedItemListIndex = Math.Max(0, Array.IndexOf(itemListOptions, display.targetedItemList));
        }

        /// <summary>
        /// 绘制自定义的Inspector界面，包括一个下拉菜单和一个“生成图标”按钮。
        /// 用户可以通过下拉菜单选择目标物品列表，并通过按钮触发图标重新生成操作。
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck(); // 开始检查更改
        
            // 在检查器中绘制下拉菜单
            targetedItemListIndex = EditorGUILayout.Popup("Targeted Item List", Mathf.Max(0, targetedItemListIndex), itemListOptions);

            if (EditorGUI.EndChangeCheck())
            {
                display.targetedItemList = itemListOptions[targetedItemListIndex].ToString();
                EditorUtility.SetDirty(display); // 标记对象以保存。
            }

            if (GUILayout.Button("生成图标")) RegenerateIcons();
        }
    
        /// <summary>
        /// 根据插槽模板重新生成所有图标子对象。
        /// 销毁当前所有非模板的子对象，并根据maxSlots数量重新实例化新的插槽对象。
        /// 此方法在点击“生成图标”按钮时被调用。
        /// </summary>
        void RegenerateIcons()
        {
            display = target as UIInventoryIconsDisplay;

            // 将整个函数调用注册为可撤销操作
            Undo.RegisterCompleteObjectUndo(display, "重新生成图标");

            if (display.slots.Length > 0)
            {
                // 销毁之前插槽中的所有子对象。
                foreach (GameObject g in display.slots)
                {
                    if (!g) continue; // 如果插槽为空，忽略它。

                    // 否则销毁它并记录为可撤销操作。
                    if (g != display.slotTemplate)
                        Undo.DestroyObjectImmediate(g);
                }
            }

            // 销毁除插槽模板外的所有其他子对象。
            for (int i = 0; i < display.transform.childCount; i++)
            {
                if (display.transform.GetChild(i).gameObject == display.slotTemplate) continue;
                Undo.DestroyObjectImmediate(display.transform.GetChild(i).gameObject);
                i--;
            }

            if (display.maxSlots <= 0) return; // 如果没有插槽，则终止。
        
            // 创建所有新的子对象。
            display.slots = new GameObject[display.maxSlots];
            display.slots[0] = display.slotTemplate;
            for (int i = 1; i < display.slots.Length; i++)
            {
                display.slots[i] = Instantiate(display.slotTemplate, display.transform);
                display.slots[i].name = display.slotTemplate.name;
            }
        }
    }
}
