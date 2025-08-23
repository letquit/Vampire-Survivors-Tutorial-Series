using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    /// <summary>
    /// 自定义编辑器类，用于在Unity编辑器中扩展UILevelSelector组件的检查器界面。
    /// 提供按钮以自动填充场景列表并生成对应的UI切换器。
    /// </summary>
    [DisallowMultipleComponent]
    [CustomEditor(typeof(UILevelSelector))]
    public class UILevelSelectorEditor : UnityEditor.Editor
    {
        UILevelSelector selector;

        /// <summary>
        /// 当编辑器启用时调用。获取目标对象的引用以便后续访问其变量。
        /// </summary>
        void OnEnable()
        {
            selector = target as UILevelSelector;
        }

        /// <summary>
        /// 重写Inspector GUI绘制方法，在检查器中显示自定义界面。
        /// 包括一个按钮来触发等级填充和UI切换器创建功能。
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!selector.toggleTemplate)
                EditorGUILayout.HelpBox(
                    "你需要为下面的按钮分配一个Toggle模板才能正常工作。",
                    MessageType.Warning
                );

            if (GUILayout.Button("查找并填充等级"))
            {
                PopulateLevelsList();
                CreateLevelSelectToggles();
            }
        }
    
        /// <summary>
        /// 查找项目中的所有场景文件，并将它们解析后添加到UILevelSelector的levels列表中。
        /// 使用正则表达式从场景名称中提取标签和显示名称信息。
        /// </summary>
        public void PopulateLevelsList()
        {
            Undo.RecordObject(selector, "创建新的SceneData结构");
            SceneAsset[] maps = UILevelSelector.GetAllMaps();

            selector.levels.RemoveAll(levels => string.IsNullOrEmpty(levels.sceneName));

            foreach (SceneAsset map in maps)
            {
                if (!selector.levels.Any(sceneData => sceneData.sceneName == map.name))
                {
                    Match m = Regex.Match(map.name, UILevelSelector.MAP_NAME_FORMAT, RegexOptions.IgnoreCase);
                    string mapLabel = "Level", mapName = "New Map";
                    if (m.Success)
                    {
                        if (m.Groups.Count > 1) mapLabel = m.Groups[1].Value;
                        if (m.Groups.Count > 2) mapName = m.Groups[2].Value;
                    }

                    selector.levels.Add(new UILevelSelector.SceneData
                    {
                        sceneName = map.name,
                        label = mapLabel,
                        displayName = mapName
                    });
                }
            }
        }
    
        /// <summary>
        /// 根据已填充的等级数据列表，使用指定的Toggle模板创建对应的UI切换器。
        /// 每个切换器会绑定相应的场景信息（如名称、编号、描述和图标）。
        /// </summary>
        public void CreateLevelSelectToggles()
        {
            if (!selector.toggleTemplate)
            {
                Debug.LogWarning("未能创建用于选择等级的切换器。请分配切换模板。");
                return;
            }

            for (int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
            {
                Toggle tog = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
                if (tog == selector.toggleTemplate) continue;
                Undo.DestroyObjectImmediate(tog.gameObject);
            }

            Undo.RecordObject(selector, "更新UILevelSelector。");
            selector.selectableToggles.Clear();

            for (int i = 0; i < selector.levels.Count; i++)
            {
                Toggle tog;
                if (i == 0)
                {
                    tog = selector.toggleTemplate;
                    Undo.RecordObject(tog, "修改模板。");
                }
                else
                {
                    tog = Instantiate(selector.toggleTemplate, selector.toggleTemplate.transform.parent);
                    Undo.RegisterCreatedObjectUndo(tog.gameObject, "创建了一个新的切换器。");
                }

                tog.gameObject.name = selector.levels[i].sceneName;

                Transform levelName = tog.transform.Find(selector.LevelImagePath).Find("Name Holder").Find(selector.LevelNamePath);
                if (levelName && levelName.TryGetComponent(out TextMeshProUGUI lvlName))
                {
                    lvlName.text = selector.levels[i].displayName;
                }

                Transform levelNumber = tog.transform.Find(selector.LevelImagePath).Find(selector.LevelNumberPath);
                if (levelNumber && levelNumber.TryGetComponent(out TextMeshProUGUI lvlNumber))
                {
                    lvlNumber.text = selector.levels[i].label;
                }
            
                Transform levelDescription = tog.transform.Find(selector.LevelDescriptionPath);
                if (levelDescription && levelDescription.TryGetComponent(out TextMeshProUGUI lvlDescription))
                {
                    lvlDescription.text = selector.levels[i].description;
                }

                Transform levelImage = tog.transform.Find(selector.LevelImagePath);
                if (levelImage && levelImage.TryGetComponent(out Image lvlImage))
                    lvlImage.sprite = selector.levels[i].icon;

                selector.selectableToggles.Add(tog);

                for (int j = 0; j < tog.onValueChanged.GetPersistentEventCount(); j++)
                {
                    if (tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                    {
                        UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                    }
                }
                UnityEventTools.AddIntPersistentListener(tog.onValueChanged, selector.Select, i);
            }
        
            EditorUtility.SetDirty(selector);
        }
    }
}
