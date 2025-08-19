using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 角色选择器类，用于管理游戏中角色的选择和数据存储
/// 该类实现为单例模式，确保在整个游戏生命周期中只有一个实例存在
/// </summary>
public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterData characterData;

    /// <summary>
    /// 在对象唤醒时执行单例模式初始化
    /// 确保场景中只有一个CharacterSelector实例存在
    /// </summary>
    private void Awake()
    {
        // 检查是否已存在实例
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果已存在实例，则销毁当前重复的对象
            Debug.Log("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 获取当前选择的角色数据
    /// </summary>
    /// <returns>返回当前选择的CharacterScriptableObject角色数据对象</returns>
    public static CharacterData GetData()
    {
        if (instance && instance.characterData)
            return instance.characterData;
        else
        {
            // 如果我们在Unity编辑器中运行游戏，则随机选择一个角色
            #if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new List<CharacterData>();
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

            // 如果找到了任何角色，则随机选择一个角色
            if (characters.Count > 0) return characters[Random.Range(0, characters.Count)];
            #endif
        }
        return null;
    }

    /// <summary>
    /// 选择并设置角色数据
    /// </summary>
    /// <param name="character">要选择的角色数据对象</param>
    public void SelectCharacter(CharacterData character)
    {
        characterData = character;
    }

    /// <summary>
    /// 销毁单例实例并清理资源
    /// </summary>
    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
