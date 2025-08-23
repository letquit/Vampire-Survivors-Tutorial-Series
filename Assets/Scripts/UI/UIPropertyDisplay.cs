using UnityEngine;
using TMPro;
using System.Reflection;
using System.Text;

/// <summary>
/// UIPropertyDisplay 是一个抽象基类，用于在 Unity UI 中显示对象的字段信息。
/// 它使用 TextMeshProUGUI 来渲染字段名称和值，并支持在编辑器中实时更新。
/// 子类需要实现 GetReadObject 和 UpdateFields 方法来定义具体行为。
/// </summary>
public abstract class UIPropertyDisplay : MonoBehaviour
{
    public bool updateInEditor = false;
    protected TextMeshProUGUI propertyNames, propertyValues;
    public const string DASH = "-";

    /// <summary>
    /// 当组件启用时调用，用于初始化并更新字段显示。
    /// </summary>
    protected virtual void OnEnable() { UpdateFields(); }

    /// <summary>
    /// 在编辑器中选中该对象时调用，如果 updateInEditor 为 true，则更新字段显示。
    /// </summary>
    protected virtual void OnDrawGizmosSelected() { if (updateInEditor) UpdateFields(); }

    /// <summary>
    /// 获取当前要读取的对象。每个子类必须实现此方法以提供要显示其字段的对象。
    /// </summary>
    /// <returns>要读取字段的对象实例。</returns>
    public abstract object GetReadObject();

    /// <summary>
    /// 判断是否应显示指定字段。
    /// 默认实现返回 true，表示所有字段都应显示。
    /// 子类可以重写此方法以根据特定条件过滤字段。
    /// </summary>
    /// <param name="field">要判断的字段信息。</param>
    /// <returns>如果字段应显示则返回 true，否则返回 false。</returns>
    protected virtual bool IsFieldShown(FieldInfo field) { return true; }

    /// <summary>
    /// 处理字段名称并将其添加到输出 StringBuilder 中。
    /// 如果字段不应显示，则不进行处理。
    /// </summary>
    /// <param name="name">字段名称。</param>
    /// <param name="output">用于存储处理后名称的 StringBuilder。</param>
    /// <param name="field">对应的字段信息。</param>
    /// <returns>处理后的 StringBuilder 实例。</returns>
    protected virtual StringBuilder ProcessName(string name, StringBuilder output, FieldInfo field)
    {
        if (!IsFieldShown(field)) return output;
        return output.AppendLine(name);
    }
    
    /// <summary>
    /// 处理字段值并将其格式化后添加到输出 StringBuilder 中。
    /// 默认只处理整数和浮点数类型。
    /// 如果字段具有 RangeAttribute 或 MinAttribute 并且是浮点数，则将其作为百分比显示。
    /// </summary>
    /// <param name="value">字段的实际值。</param>
    /// <param name="output">用于存储处理后值的 StringBuilder。</param>
    /// <param name="field">对应的字段信息。</param>
    /// <returns>处理后的 StringBuilder 实例。</returns>
    protected virtual StringBuilder ProcessValue(object value, StringBuilder output, FieldInfo field)
    {
        if (!IsFieldShown(field)) return output;

        float fval = value is int ? (int)value : value is float ? (float)value : 0;

        // 如果它具有分配的范围或最小属性并且是浮点数，则将其作为百分比打印。
        PropertyAttribute attribute = (PropertyAttribute)field.GetCustomAttribute<RangeAttribute>() ?? field.GetCustomAttribute<MinAttribute>();
        if (attribute != null && field.FieldType == typeof(float))
        {
            float percentage = Mathf.Round(fval * 100 - 100);

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
        }
        else
        {
            output.Append(value).Append('\n');
        }
        return output;
    }
    
    /// <summary>
    /// 获取指定类型的字段信息，并生成两个 StringBuilder：
    /// 一个用于字段名称，另一个用于字段值。
    /// </summary>
    /// <param name="flags">用于获取字段的 BindingFlags。</param>
    /// <param name="targetedType">目标类型的完整名称。</param>
    /// <returns>包含两个 StringBuilder 的数组：[0] 为字段名称，[1] 为字段值。</returns>
    protected virtual StringBuilder[] GetProperties(BindingFlags flags, string targetedType)
    {
        // 渲染所有统计名称和值。
        // 使用StringBuilder以便字符串操作运行得更快。
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        FieldInfo[] fields = System.Type.GetType(targetedType).GetFields(flags);
        foreach (FieldInfo field in fields)
        {
            // 渲染统计名称。
            ProcessName(field.Name, names, field);
            ProcessValue(field.GetValue(GetReadObject()), values, field);
        }

        // 使用我们构建的字符串更新字段。
        return new StringBuilder[2] { PrettifyNames(names), values };
    }

    /// <summary>
    /// 更新 UI 显示内容。子类必须实现此方法以将处理后的字段信息显示到 UI 上。
    /// </summary>
    public abstract void UpdateFields();
    
    /// <summary>
    /// 对字段名称进行美化处理，例如将驼峰命名转换为空格分隔的可读格式。
    /// </summary>
    /// <param name="input">原始字段名称字符串。</param>
    /// <returns>美化后的字段名称字符串。</returns>
    public static StringBuilder PrettifyNames(StringBuilder input)
    {
        // 如果StringBuilder为空，则返回空字符串。
        if (input.Length <= 0) return null;

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
        return result;
    }
}
