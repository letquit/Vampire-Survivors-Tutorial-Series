using UnityEngine;

/// <summary>
/// 生成数据抽象类，用于定义游戏对象生成的配置和行为
/// 继承自ScriptableObject，可在Unity编辑器中创建资产文件进行配置
/// </summary>
public abstract class SpawnData : ScriptableObject
{
    [Tooltip("所有可能生成的GameObject列表。")] public GameObject[] possibleSpawnPrefabs = new GameObject[1];

    [Tooltip("每次生成之间的时间（以秒为单位）。将在X和Y之间取一个随机数。")]
    public Vector2 spawnInterval = new Vector2(2, 3);

    [Tooltip("每个间隔生成多少敌人？")] public Vector2Int spawnsPerTick = new Vector2Int(1, 1);

    [Tooltip("这将生成敌人多长时间（以秒为单位）。")] [Min(0.1f)]
    public float duration = 60;

    /// <summary>
    /// 获取应该生成的游戏对象数组
    /// 根据配置随机选择指定数量的预制体
    /// </summary>
    /// <param name="totalEnemies">当前屏幕上的敌人总数，默认为0</param>
    /// <returns>包含随机选择的预制体的游戏对象数组</returns>
    public virtual GameObject[] GetSpawns(int totalEnemies = 0)
    {
        // 确定要生成多少敌人
        int count = Random.Range(spawnsPerTick.x, spawnsPerTick.y);

        // 生成结果数组
        GameObject[] result = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            // 随机选择一个可能的生成物并将其插入结果中
            result[i] = possibleSpawnPrefabs[Random.Range(0, possibleSpawnPrefabs.Length)];
        }

        return result;
    }

    /// <summary>
    /// 获取随机生成间隔时间
    /// 在配置的最小值和最大值之间随机选择一个时间间隔
    /// </summary>
    /// <returns>随机生成间隔时间（秒）</returns>
    public virtual float GetSpawnInterval()
    {
        return Random.Range(spawnInterval.x, spawnInterval.y);
    }
}
