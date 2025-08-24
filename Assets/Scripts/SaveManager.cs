using UnityEngine;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

/// <summary>
/// 一个简单的SaveManager，用于保存玩家拥有的硬币总数。
/// 在后续部分中，这将用于存储玩家的所有存档数据，但目前我们保持简单。
/// </summary>
public class SaveManager
{
    /// <summary>
    /// 表示游戏存档数据的结构类。
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// 玩家拥有的硬币数量。
        /// </summary>
        public float coins;
    }

    /// <summary>
    /// 存档文件的名称常量。
    /// </summary>
    private const string SAVE_FILE_NAME = "SaveData.json";

    /// <summary>
    /// 上次加载的游戏数据缓存。
    /// </summary>
    private static GameData lastLoadedGameData;

    /// <summary>
    /// 获取上次加载的游戏数据。如果尚未加载，则自动调用Load方法进行加载。
    /// </summary>
    public static GameData LastLoadedGameData
    {
        get
        {
            if (lastLoadedGameData == null) Load();
            return lastLoadedGameData;
        }
    }
    
    /// <summary>
    /// 获取存档文件在设备上的完整路径。
    /// </summary>
    /// <returns>存档文件的完整路径字符串。</returns>
    public static string GetSavePath()
    {
        return string.Format("{0}/{1}", Application.persistentDataPath, SAVE_FILE_NAME);
    }

    /// <summary>
    /// 将游戏数据保存到本地文件。
    /// 当不带参数调用此函数时，它将保存到上次加载的游戏文件中（这是你应该99%的时间调用Save()的方式）。
    /// 但你也可以选择提供一个参数来完全覆盖保存。
    /// </summary>
    /// <param name="data">要保存的游戏数据。如果为null，则使用上次加载的数据。</param>
    public static void Save(GameData data = null)
    {
        // 确保保存始终有效。
        if (data == null)
        {
            // 如果没有上次加载的游戏，我们先加载游戏以填充lastLoadedGameData，然后再保存。
            if (lastLoadedGameData == null) Load();
            data = lastLoadedGameData;
        }
        File.WriteAllText(GetSavePath(), JsonUtility.ToJson(data));
    }
    
    /// <summary>
    /// 从本地文件加载游戏数据。
    /// </summary>
    /// <param name="usePreviousLoadIfAvailable">
    /// 是否优先使用上次加载的数据缓存以提高性能。
    /// 设置为true可避免重复读取磁盘文件。
    /// </param>
    /// <returns>加载的游戏数据对象。</returns>
    public static GameData Load(bool usePreviousLoadIfAvailable = false)
    {
        // usePreviousLoadIfAvailable 旨在加快加载调用的速度，
        // 因为我们不需要每次访问数据时都读取保存文件。
        if (usePreviousLoadIfAvailable && lastLoadedGameData != null)
            return lastLoadedGameData;

        // 从硬盘中检索加载的数据。
        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            lastLoadedGameData = JsonUtility.FromJson<GameData>(json);
            if (lastLoadedGameData == null) lastLoadedGameData = new GameData();
        }
        else
        {
            lastLoadedGameData = new GameData();
        }
        return lastLoadedGameData;
    }
}
