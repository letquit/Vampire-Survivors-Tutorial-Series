using UnityEngine;

[CreateAssetMenu(fileName = "Wave Data", menuName = "2D Top-down Rogue-like/Wave Data")]
public class WaveData : SpawnData
{
    [Header("Wave Data")] [Tooltip("如果敌人数量少于这个数值，我们将继续生成直到达到该数量。")] [Min(0)]
    public int startingCount = 0;

    [Tooltip("这一波最多可以生成多少敌人？")] [Min(1)] public uint totalSpawns = uint.MaxValue;

    [System.Flags]
    public enum ExitCondition
    {
        waveDuration = 1,
        reachedTotalSpawns = 2
    }

    [Tooltip("设置可以触发这一波结束的条件。")] public ExitCondition exitConditions = (ExitCondition)1;

    [Tooltip("所有敌人都必须死亡才能进入下一波。")] public bool mustKillAll = false;

    [HideInInspector] public uint spawnCount; // 这一波中已经生成的敌人数量

    // 返回这一波可以生成的预制体数组。
    // 可选参数表示当前屏幕上有多少敌人。
    public override GameObject[] GetSpawns(int totalEnemies = 0)
    {
        // 确定要生成多少敌人。
        int count = Random.Range(spawnsPerTick.x, spawnsPerTick.y);

        // 如果屏幕上的敌人数量少于最小敌人数量，我们将把计数设置为需要生成的敌人数量，以填充屏幕直到达到最小敌人数量。
        if (totalEnemies + count < startingCount)
            count = startingCount - totalEnemies;

        // 生成结果。
        GameObject[] result = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            // 随机选择一个可能的生成物并将其插入到结果数组中。
            result[i] = possibleSpawnPrefabs[Random.Range(0, possibleSpawnPrefabs.Length)];
        }

        return result;
    }
}