using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Reflection;

/// <summary>
/// 等级选择界面控制器，用于管理关卡选择、显示和加载逻辑。
/// </summary>
public class UILevelSelector : MonoBehaviour
{
    /// <summary>
    /// 用于显示当前选中等级信息的 UI 显示组件。
    /// </summary>
    public UISceneDataDisplay statsUI;

    /// <summary>
    /// 当前选中的等级索引。-1 表示未选择任何等级。
    /// </summary>
    public static int selectedLevel = -1;

    /// <summary>
    /// 当前选中的等级数据。
    /// </summary>
    public static SceneData currentLevel;

    /// <summary>
    /// 所有可用的等级数据列表。
    /// </summary>
    public List<SceneData> levels = new List<SceneData>();

    [Header("Template")]
    /// <summary>
    /// 用于动态生成等级选择项的 Toggle 模板。
    /// </summary>
    public Toggle toggleTemplate;

    /// <summary>
    /// 在 Toggle 中查找“等级名称”文本组件的路径。
    /// </summary>
    public string LevelNamePath = "Level Name";

    /// <summary>
    /// 在 Toggle 中查找“等级编号”文本组件的路径。
    /// </summary>
    public string LevelNumberPath = "Level Number";

    /// <summary>
    /// 在 Toggle 中查找“等级描述”文本组件的路径。
    /// </summary>
    public string LevelDescriptionPath = "Level Description";

    /// <summary>
    /// 在 Toggle 中查找“等级图像”图像组件的路径。
    /// </summary>
    public string LevelImagePath = "Level Image";

    /// <summary>
    /// 存储所有动态生成的可选 Toggle 组件。
    /// </summary>
    public List<Toggle> selectableToggles = new List<Toggle>();

    /// <summary>
    /// 全局增益数据，将应用于玩家和敌人。
    /// </summary>
    public static BuffData globalBuff;

    /// <summary>
    /// 标记全局增益是否对玩家产生影响。
    /// </summary>
    public static bool globalBuffAffectsPlayer = false;

    /// <summary>
    /// 标记全局增益是否对敌人产生影响。
    /// </summary>
    public static bool globalBuffAffectsEnemies = false;

    /// <summary>
    /// 用于匹配等级地图名称的正则表达式格式。
    /// </summary>
    public const string MAP_NAME_FORMAT = "^(Level .*) ?- ?(.*$)";

    /// <summary>
    /// 场景数据类，用于存储每个等级的相关信息。
    /// </summary>
    [Serializable]
    public class SceneData
    {
        /// <summary>
        /// 场景名称。
        /// </summary>
        public string sceneName;

        [Header("UI 显示")]
        /// <summary>
        /// 显示名称。
        /// </summary>
        public string displayName;

        /// <summary>
        /// 标签。
        /// </summary>
        public string label;

        /// <summary>
        /// 等级描述。
        /// </summary>
        [TextArea] public string description;

        /// <summary>
        /// 等级图标。
        /// </summary>
        public Sprite icon;

        [Header("修改器")]
        /// <summary>
        /// 玩家属性修改器。
        /// </summary>
        public CharacterData.Stats playerModifier;

        /// <summary>
        /// 敌人属性修改器。
        /// </summary>
        public EnemyStats.Stats enemyModifier;

        /// <summary>
        /// 时间限制（秒）。-1 表示无限制。
        /// </summary>
        [Min(-1)] public float timeLimit = 0f;

        /// <summary>
        /// 游戏时钟速度。
        /// </summary>
        public float clockSpeed = 1f;

        /// <summary>
        /// 额外备注信息。
        /// </summary>
        [TextArea] public string extraNotes = "--";
    }

    #if UNITY_EDITOR
    /// <summary>
    /// 获取所有符合命名规则的场景资源。
    /// </summary>
    /// <returns>所有匹配的场景资源数组。</returns>
    public static SceneAsset[] GetAllMaps()
    {
        List<SceneAsset> maps = new List<SceneAsset>();

        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".unity"))
            {
                SceneAsset map = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                if (map != null && Regex.IsMatch(map.name, MAP_NAME_FORMAT))
                {
                    maps.Add(map);
                }
            }
        }
        maps.Reverse();
        return maps.ToArray();
    }
    #endif

    /// <summary>
    /// 切换到指定名称的场景。
    /// </summary>
    /// <param name="name">要加载的场景名称。</param>
    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 加载当前选中的等级场景。
    /// </summary>
    public void LoadSelectedLevel()
    {
        if (selectedLevel >= 0 && selectedLevel < levels.Count)
        {
            SceneManager.LoadScene(levels[selectedLevel].sceneName);
            currentLevel = levels[selectedLevel];
            selectedLevel = -1;
            Time.timeScale = 1f;
        }
        else
        {
            Debug.LogWarning("没有选择等级！");
        }
    }

    /// <summary>
    /// 选择指定索引的等级，并更新 UI 和全局增益数据。
    /// </summary>
    /// <param name="sceneIndex">要选择的等级索引。</param>
    public void Select(int sceneIndex)
    {
        selectedLevel = sceneIndex;
        statsUI.UpdateFields();
        globalBuff = GenerateGlobalBuffData();
        globalBuffAffectsPlayer = globalBuff && !IsModifierEmpty(globalBuff.variations[0].playerModifier);
        globalBuffAffectsEnemies = globalBuff && !IsModifierEmpty(globalBuff.variations[0].enemyModifier);
    }

    /// <summary>
    /// 生成一个包含当前等级修改器的全局增益数据对象。
    /// </summary>
    /// <returns>生成的 BuffData 对象。</returns>
    public BuffData GenerateGlobalBuffData()
    {
        BuffData bd = ScriptableObject.CreateInstance<BuffData>();
        bd.name = "全局等级增益";
        bd.variations[0].damagePerSecond = 0;
        bd.variations[0].duration = 0;
        bd.variations[0].playerModifier = levels[selectedLevel].playerModifier;
        bd.variations[0].enemyModifier = levels[selectedLevel].enemyModifier;
        return bd;
    }

    /// <summary>
    /// 检查指定对象的所有字段值之和是否为零，用于判断修改器是否为空。
    /// </summary>
    /// <param name="obj">要检查的对象。</param>
    /// <returns>如果所有字段值之和为零，则返回 true；否则返回 false。</returns>
    private static bool IsModifierEmpty(object obj)
    {
        Type type = obj.GetType();
        FieldInfo[] fields = type.GetFields();
        float sum = 0;
        foreach (FieldInfo f in fields)
        {
            object val = f.GetValue(obj);
            if (val is int) sum += (int)val;
            else if (val is float) sum += (float)val;
        }

        return Mathf.Approximately(sum, 0);
    }
}
