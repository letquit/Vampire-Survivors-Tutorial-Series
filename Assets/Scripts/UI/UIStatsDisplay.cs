using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// UIStatsDisplay 类用于在 UI 中显示玩家的统计数据。
/// 它会自动更新文本字段以反映 PlayerStats 对象中的当前值。
/// </summary>
public class UIStatsDisplay : MonoBehaviour
{
    public PlayerStats player; // 此统计显示正在渲染的玩家统计数据。
    public bool displayCurrentHealth = false;
    public bool updateInEditor = false;
    TextMeshProUGUI statNames, statValues;

    /// <summary>
    /// 当此组件被启用时调用，用于更新统计信息显示。
    /// </summary>
    private void OnEnable()
    {
        UpdateStatFields();
    }

    /// <summary>
    /// 在编辑器中选中该对象时调用，如果 updateInEditor 为 true，则更新统计信息显示。
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (updateInEditor) UpdateStatFields();
    }

    /// <summary>
    /// 更新统计名称和值的文本字段内容。
    /// 遍历 CharacterData.Stats 的所有公共实例字段，并根据属性格式化输出。
    /// </summary>
    public void UpdateStatFields()
    {
        if (!player) return;

        // 获取用于渲染统计名称和统计值的两个文本对象的引用。
        if (!statNames) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!statValues) statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // 渲染所有统计名称和值。
        // 使用StringBuilder以便字符串操作运行得更快。
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        if (displayCurrentHealth)
        {
            names.AppendLine("Health");
            values.AppendLine(player.CurrentHealth.ToString());
        }
        
        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            // 渲染统计名称。
            names.AppendLine(field.Name);

            // 获取统计值。
            object val = field.GetValue(player.Stats);
            float fval = val is int ? (int)val : (float)val;
            
            // 如果它有属性分配并且是浮点数，则以百分比形式打印。
            PropertyAttribute attribute = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));
            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                // 如果统计值为0，只需放置一个破折号。
                if (Mathf.Approximately(percentage, 0))
                {
                    values.Append('-').Append('\n'); // 如果百分比接近于0，添加破折号和换行符。
                }
                else
                {
                    if (percentage > 0)
                        values.Append('+'); // 如果百分比大于0，添加加号。
                    values.Append(percentage).Append('%').Append('\n'); // 添加百分比值、百分号和换行符。
                }
            }
            else
            {
                values.Append(fval).Append('\n');
            } 
            
            // 使用我们构建的字符串更新字段。
            statNames.text = PrettifyNames(names);
            statValues.text = values.ToString();
        }
    }
    
    /// <summary>
    /// 将输入的字符串进行美化处理，将驼峰命名转换为空格分隔的单词，并首字母大写。
    /// </summary>
    /// <param name="input">需要美化的原始字符串构建器</param>
    /// <returns>美化后的字符串</returns>
    public static string PrettifyNames(StringBuilder input)
    {
        // 如果StringBuilder为空，则返回空字符串。
        if (input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // 检查何时将字符转换为大写或添加空格。
            if (last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            }
            else if (char.IsUpper(c))
            {
                result.Append(' '); // 在大写字母前插入空格
            }
            result.Append(c);

            last = c;
        }

        return result.ToString();
    }

    /// <summary>
    /// 在编辑器中重置组件时调用，尝试查找场景中的第一个 PlayerStats 组件并赋值给 player 字段。
    /// </summary>
    private void Reset()
    {
        player = FindFirstObjectByType<PlayerStats>();
    }
}
