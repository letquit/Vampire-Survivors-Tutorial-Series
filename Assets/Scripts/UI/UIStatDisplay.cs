using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// UIStatsDisplay 类用于在 UI 中显示玩家的统计数据。
/// 它会自动更新文本字段以反映 PlayerStats 对象中的当前值。
/// </summary>
public class UIStatDisplay : UIPropertyDisplay
{
    public PlayerStats player; // 此统计显示正在渲染的玩家统计数据。
    public CharacterData character;
    public bool displayCurrentHealth = false;
    
    /// <summary>
    /// 获取要读取的对象，根据当前场景返回玩家或角色的统计数据。
    /// </summary>
    /// <returns>返回 PlayerStats 或 CharacterData 中的 Stats 对象，如果没有则返回默认构造的 Stats 实例。</returns>
    public override object GetReadObject()
    {
        // 在游戏场景中返回玩家统计数据，在字符选择场景中返回角色统计数据，因为没有分配'player'变量。
        if (player) return player.Stats;
        else if (character) return character.stats;
        return new CharacterData.Stats();
    }

    /// <summary>
    /// 更新 UI 中显示的属性字段内容，包括统计名称和对应的值。
    /// </summary>
    public override void UpdateFields()
    {
        if (!player && !character) return;

        StringBuilder[] allStats = GetProperties(
            BindingFlags.Public | BindingFlags.Instance,
            "CharacterData+Stats"
        );

        // 获取两个Text对象的引用以渲染统计名称和统计值。
        if (!propertyNames) propertyNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!propertyValues) propertyValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // 将当前健康值添加到统计框中。
        if (displayCurrentHealth)
        {
            allStats[0].Insert(0, "Health\n");
            allStats[1].Insert(0, player.CurrentHealth + "\n");
        }

        // 使用我们构建的字符串更新字段。
        if (propertyNames) propertyNames.text = allStats[0].ToString();
        if (propertyValues) propertyValues.text = allStats[1].ToString();

        propertyValues.fontSize = propertyNames.fontSize;
    }
    
    /// <summary>
    /// 在编辑器中重置组件时调用，尝试查找场景中的第一个 PlayerStats 组件并赋值给 player 字段。
    /// </summary>
    private void Reset()
    {
        player = FindFirstObjectByType<PlayerStats>();
    }
}
