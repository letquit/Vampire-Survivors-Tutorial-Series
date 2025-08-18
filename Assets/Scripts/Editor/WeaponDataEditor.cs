using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Editor
{
    /// <summary>
    /// 武器数据的自定义编辑器类，用于在Unity编辑器中自定义WeaponData对象的Inspector面板显示
    /// </summary>
    [CustomEditor(typeof(WeaponData))]
    public class WeaponDataEditor : UnityEditor.Editor
    {
        private WeaponData weaponData;
        private string[] weaponSubtypes;
        private int selectedWeaponSubtype;

        /// <summary>
        /// 当编辑器启用时调用，用于初始化武器数据和获取所有武器子类型
        /// </summary>
        private void OnEnable()
        {
            // 缓存武器数据值
            weaponData = (WeaponData)target;

            // 获取所有武器子类型并缓存
            System.Type baseType = typeof(Weapon);
            List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
                .ToList();

            // 在列表前面添加一个"无"选项
            List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
            subTypesString.Insert(0, "None");
            weaponSubtypes = subTypesString.ToArray();

            // 确保使用正确的武器子类型
            selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behaviour));
        }

        /// <summary>
        /// 重写Inspector GUI绘制方法，用于自定义WeaponData在Inspector面板中的显示
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 在Inspector中绘制下拉框
            selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

            if (selectedWeaponSubtype > 0)
            {
                // 更新行为字段
                weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();
                EditorUtility.SetDirty(weaponData); // 标记对象需要保存
                DrawDefaultInspector(); // 绘制默认的Inspector元素
            }
        }
    }
}
