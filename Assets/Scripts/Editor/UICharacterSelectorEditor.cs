using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    /// <summary>
    /// 自定义编辑器类，用于在Unity编辑器中扩展 UICharacterSelector 组件的检查器界面。
    /// 提供一个按钮，用于根据项目中的角色数据自动生成可选择角色的Toggle控件。
    /// </summary>
    [DisallowMultipleComponent]
    [CustomEditor(typeof(UICharacterSelector))]
    public class UICharacterSelectorEditor : UnityEditor.Editor
    {
        UICharacterSelector selector;

        /// <summary>
        /// 当编辑器启用时调用。获取当前正在编辑的目标对象（UICharacterSelector）的引用。
        /// </summary>
        void OnEnable()
        {
            selector = target as UICharacterSelector;
        }

        /// <summary>
        /// 重写Inspector界面的绘制方法。显示默认的Inspector内容，并添加一个按钮用于生成角色切换控件。
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (selector != null && GUILayout.Button("生成可选择的角色"))
            {
                CreateTogglesForCharacterData();
            }
        }
        
        /// <summary>
        /// 根据项目中的所有角色数据资产，为UICharacterSelector生成对应的Toggle控件。
        /// 每个Toggle将显示角色名称、图标和初始武器图标，并绑定到角色选择逻辑。
        /// </summary>
        public void CreateTogglesForCharacterData()
        {
            // 如果切换模板未分配，请留下警告并中止。
            if (!selector.toggleTemplate)
            {
                Debug.LogWarning("请首先为UI角色选择器分配一个切换模板。");
                return;
            }

            // 遍历切换模板父级的所有子项，
            // 删除其下的所有内容，除了模板。
            for (int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
            {
                Toggle tog = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
                if (tog == selector.toggleTemplate) continue;
                Undo.DestroyObjectImmediate(tog.gameObject); // 记录操作以便我们可以撤销。
            }

            // 记录对UICharacterSelector组件的更改作为可撤销的，并清除切换列表。
            Undo.RecordObject(selector, "更新UICharacterSelector。");
            selector.selectableToggles.Clear();
            CharacterData[] characters = UICharacterSelector.GetAllCharacterDataAssets();

            // 对于项目中的每个角色数据资产，我们在角色选择器中为它们创建一个切换。
            for (int i = 0; i < characters.Length; i++)
            {
                Toggle tog;
                if (i == 0)
                {
                    tog = selector.toggleTemplate;
                    Undo.RecordObject(tog, "修改模板。");
                }
                else
                {
                    tog = Instantiate(selector.toggleTemplate, selector.toggleTemplate.transform.parent); // 创建当前角色的切换作为原始模板父级的子项。
                    Undo.RegisterCreatedObjectUndo(tog.gameObject, "创建了一个新的切换。");
                }

                // 查找要分配的角色名称、图标和武器图标。
                Transform characterName = tog.transform.Find(selector.characterNamePath);
                if (characterName && characterName.TryGetComponent(out TextMeshProUGUI tmp))
                {
                    tmp.text = tog.gameObject.name = characters[i].Name;
                }

                Transform characterIcon = tog.transform.Find(selector.characterIconPath);
                if (characterIcon && characterIcon.TryGetComponent(out Image chrIcon))
                    chrIcon.sprite = characters[i].Icon;

                Transform weaponIcon = tog.transform.Find(selector.weaponIconPath);
                if (weaponIcon && weaponIcon.TryGetComponent(out Image wpnIcon))
                    wpnIcon.sprite = characters[i].StartingWeapon.icon;

                selector.selectableToggles.Add(tog);

                // 移除所有选择事件并添加我们自己的事件，以检查点击了哪个角色切换。
                for (int j = 0; j < tog.onValueChanged.GetPersistentEventCount(); j++)
                {
                    if (tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                    {
                        UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                    }
                }
                UnityEventTools.AddObjectPersistentListener(tog.onValueChanged, selector.Select, characters[i]);
            }
            
            // 注册更改以便在完成后保存。
            EditorUtility.SetDirty(selector);
        }
    }
}
