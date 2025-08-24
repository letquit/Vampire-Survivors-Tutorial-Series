using System.Text;
using System;
using System.Reflection;
using UnityEngine;
using TMPro;

/// <summary>
/// UISceneDataDisplay 类用于显示场景数据的 UI 属性。
/// 它继承自 UIPropertyDisplay，并扩展了对玩家和敌人修改器中属性的处理。
/// </summary>
public class UISceneDataDisplay : UIPropertyDisplay
{
    /// <summary>
    /// 关卡选择器引用，用于获取当前选中的关卡数据。
    /// </summary>
    public UILevelSelector levelSelector;

    /// <summary>
    /// 额外关卡信息的文本组件，用于显示额外说明信息。
    /// </summary>
    TextMeshProUGUI extraStageInfo;

    /// <summary>
    /// 获取要读取的对象实例。
    /// 如果 levelSelector 存在且有选中关卡，则返回该关卡的数据；
    /// 否则返回一个新的 UILevelSelector.SceneData 实例。
    /// </summary>
    /// <returns>当前选中关卡的数据对象或默认 SceneData 实例。</returns>
    public override object GetReadObject()
    {
        if (levelSelector && UILevelSelector.selectedLevel >= 0)
            return levelSelector.levels[UILevelSelector.selectedLevel];
        return new UILevelSelector.SceneData();
    }

    /// <summary>
    /// 更新 UI 显示字段内容。
    /// 包括基础属性以及从 playerModifier 和 enemyModifier 中提取的附加属性。
    /// </summary>
    public override void UpdateFields()
    {
        // 获取两个Text对象的引用以渲染统计名称和统计值。
        if (!propertyNames) propertyNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!propertyValues) propertyValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (!extraStageInfo) extraStageInfo = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        
        // 获取所有属性的字符串。
        StringBuilder[] allStats = GetProperties(
            BindingFlags.Public | BindingFlags.Instance,
            "UILevelSelector+SceneData"
        );

        // 提取用户和敌人统计修改器。
        // 我们希望将其中一些值添加到此统计显示中。
        UILevelSelector.SceneData dat = (UILevelSelector.SceneData)GetReadObject();

        allStats[0].AppendLine("Move Speed").AppendLine("Gold Bonus").AppendLine("Luck Bonus").AppendLine("XP Bonus").AppendLine("Enemy Health");

        // 添加其他奖励。
        Type characterDataStats = typeof(CharacterData.Stats);
        ProcessValue(dat.playerModifier.moveSpeed, allStats[1], characterDataStats.GetField("moveSpeed"));
        ProcessValue(dat.playerModifier.greed, allStats[1], characterDataStats.GetField("greed"));
        ProcessValue(dat.playerModifier.luck, allStats[1], characterDataStats.GetField("luck"));
        ProcessValue(dat.playerModifier.growth, allStats[1], characterDataStats.GetField("growth"));

        Type enemyStats = typeof(EnemyStats.Stats);
        ProcessValue(dat.enemyModifier.maxHealth, allStats[1], enemyStats.GetField("maxHealth"));

        // 使用我们构建的字符串更新字段。
        if (propertyNames) propertyNames.text = allStats[0].ToString();
        if (propertyValues) propertyValues.text = allStats[1].ToString();
    }
    
    /// <summary>
    /// 判断某个字段是否应该在 UI 中显示。
    /// </summary>
    /// <param name="field">要判断的字段信息。</param>
    /// <returns>如果字段应显示则返回 true，否则返回 false。</returns>
    protected override bool IsFieldShown(FieldInfo field)
    {
        switch (field.Name)
        {
            default:
                return false;
            case "timeLimit":
            case "clockSpeed":
            case "moveSpeed":
            case "greed":
            case "luck":
            case "growth":
            case "maxHealth":
                return true;
        }
    }

    /// <summary>
    /// 处理字段名称的显示方式。
    /// 特别地，忽略 "extraNotes" 字段的名称显示。
    /// </summary>
    /// <param name="name">原始字段名称。</param>
    /// <param name="output">用于输出处理后名称的 StringBuilder。</param>
    /// <param name="field">对应的字段信息。</param>
    /// <returns>处理后的 StringBuilder。</returns>
    protected override StringBuilder ProcessName(string name, StringBuilder output, FieldInfo field)
    {
        if (field.Name == "extraNotes") return output;
        return base.ProcessName(name, output, field);
    }

    /// <summary>
    /// 处理字段值的格式化显示。
    /// 根据字段类型进行不同的格式化处理，如时间、百分比等。
    /// </summary>
    /// <param name="value">字段的实际值。</param>
    /// <param name="output">用于输出处理后值的 StringBuilder。</param>
    /// <param name="field">对应的字段信息。</param>
    /// <returns>处理后的 StringBuilder。</returns>
    protected override StringBuilder ProcessValue(object value, StringBuilder output, FieldInfo field)
    {
        // 如果有特定于此元素的字段需要处理，
        // 我们直接处理它们而不交给父类。
        float fval;
        switch (field.Name)
        {
            case "timeLimit":
                fval = value is int ? (int)value : (float)value;
                if (fval == 0)
                {
                    output.Append(DASH).Append('\n');
                }
                else
                {
                    string minutes = Mathf.FloorToInt(fval / 60).ToString();
                    string seconds = (fval % 60).ToString();
                    if (fval % 60 < 10)
                    {
                        seconds += "0";
                    }

                    output.Append(minutes).Append(":").Append(seconds).Append('\n');
                }

                return output;

            case "clockSpeed":
                fval = value is int ? (int)value : (float)value;
                output.Append(fval).Append("x").Append('\n');
                return output;

            case "maxHealth":
            case "moveSpeed":
            case "greed":
            case "luck":
            case "growth":
                fval = value is int ? (int)value : (float)value;
                float percentage = Mathf.Round(fval * 100);

                // 如果统计值为0，只需添加一个破折号。
                if (Mathf.Approximately(percentage, 0))
                {
                    output.Append(DASH).Append('\n');
                }
                else
                {
                    if (percentage > 0)
                        output.Append('+');
                    output.Append(percentage).Append('%').Append('\n');
                }

                return output;

            case "extraNotes":
                if (value == null) return output;
                string msg = value.ToString();
                extraStageInfo.text = string.IsNullOrWhiteSpace(msg) ? DASH : msg;
                return output;
        }

        // 将处理过程交给父类。
        return base.ProcessValue(value, output, field);
    }
    
    /// <summary>
    /// 在 Unity 编辑器中自动查找并设置 levelSelector 引用。
    /// </summary>
    private void Reset()
    {
        levelSelector = FindFirstObjectByType<UILevelSelector>();
    }
}
