using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 角色选择器类，用于管理游戏中角色的选择和数据存储
/// 该类实现为单例模式，确保在整个游戏生命周期中只有一个实例存在
/// </summary>
public class UICharacterSelector : MonoBehaviour
{
    public CharacterData defaultCharacter;
    public static CharacterData selected;
    public UIStatDisplay statsUI;

    [Header("模板")]
    public Toggle toggleTemplate;
    public string characterNamePath = "Character Name";
    public string weaponIconPath = "Weapon Icon";
    public string characterIconPath = "Character Icon";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("描述框")]
    public TextMeshProUGUI characterFullName;
    public TextMeshProUGUI characterDescription;
    public Image selectedCharacterIcon;
    public Image selectedCharacterWeapon;

    /// <summary>
    /// 在场景加载时调用。如果设置了默认角色，则自动选择该角色。
    /// </summary>
    void Start()
    {
        // 如果指定了默认角色，在场景加载时选择它。
        if (defaultCharacter) Select(defaultCharacter);
    }

    /// <summary>
    /// 获取项目中所有的 CharacterData 资源文件。
    /// 注意：此方法只能在 Unity 编辑器环境中使用，构建版本中调用将输出警告信息。
    /// </summary>
    /// <returns>返回包含所有 CharacterData 资源的数组。</returns>
    public static CharacterData[] GetAllCharacterDataAssets()
    {
        List<CharacterData> characters = new List<CharacterData>();

        // 在编辑器中填充所有角色数据资产的列表。
#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".asset"))
            {
                CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                if (characterData != null)
                {
                    characters.Add(characterData);
                }
            }
        }
#else
    Debug.LogWarning("此函数不能在构建中调用。");
#endif
        return characters.ToArray();
    }
    
    /// <summary>
    /// 获取当前选中的角色数据。
    /// 如果没有手动选择角色，并且当前处于编辑器运行状态，则随机返回一个角色数据。
    /// </summary>
    /// <returns>返回当前选中的 CharacterData 实例，如果没有可用角色则返回 null。</returns>
    public static CharacterData GetData()
    {
        // 使用在 Select() 函数中选择的角色。
        if (selected)
            return selected;
        else
        {
            // 如果我们正在从编辑器中运行，则随机选择一个角色。
            CharacterData[] characters = GetAllCharacterDataAssets();
            if (characters.Length > 0) return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }

    /// <summary>
    /// 选择指定的角色并更新 UI 显示内容。
    /// 包括角色统计数据、全名、描述以及图标等信息。
    /// </summary>
    /// <param name="character">要选择的角色数据对象。</param>
    public void Select(CharacterData character)
    {
        // 更新角色选择屏幕中的统计字段。
        selected = statsUI.character = character;
        statsUI.UpdateStatFields();

        // 更新描述框内容。
        characterFullName.text = character.FullName;
        characterDescription.text = character.CharacterDescription;
        selectedCharacterIcon.sprite = character.Icon;
        selectedCharacterWeapon.sprite = character.StartingWeapon.icon;
    }
}
