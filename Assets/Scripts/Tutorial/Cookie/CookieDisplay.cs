using TMPro;
using UnityEngine;

/// <summary>
/// 饼干显示组件，用于格式化显示饼干数量
/// </summary>
public class CookieDisplay : MonoBehaviour
{
    /// <summary>
    /// 更新饼干文本显示
    /// </summary>
    /// <param name="cookieCount">饼干数量</param>
    /// <param name="textToChange">要更新的TextMeshProUGUI文本组件</param>
    /// <param name="optionalEndText">可选的后缀文本，默认为null</param>
    public void UpdateCookieText(double cookieCount, TextMeshProUGUI textToChange, string optionalEndText = null)
    {
        // 定义数量级后缀数组：无、千、百万、十亿、万亿、千万亿
        string [] suffixes = { "", "k", "M", "B", "T", "Q"};
        int index = 0;

        // 循环除以1000直到数值小于1000或达到最大后缀
        while (cookieCount >= 1000 && index < suffixes.Length - 1)
        {
            cookieCount /= 1000;
            index++;

            if (index >= suffixes.Length - 1 && cookieCount >= 1000)
            {
                break;
            }
        }

        string formattedText;

        // 根据数量级索引决定格式化方式
        if (index == 0)
        {
            formattedText = cookieCount.ToString();
        }
        else
        {
            // 保留一位小数并添加对应后缀
            formattedText = cookieCount.ToString("F1") + suffixes[index];
        }

        textToChange.text = formattedText + optionalEndText;
    }
}