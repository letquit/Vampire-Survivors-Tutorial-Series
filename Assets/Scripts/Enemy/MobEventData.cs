using UnityEngine;

/// <summary>
/// 怪物事件数据类，用于定义和处理游戏中怪物生成的事件
/// 继承自EventData基类，可通过Unity编辑器的Assets/Create/2D Top-down Rogue-like/Event Data/Mob菜单创建
/// </summary>
[CreateAssetMenu(fileName = "Mob Event Data", menuName = "2D Top-down Rogue-like/Event Data/Mob")]
public class MobEventData : EventData
{
    [Header("怪物数据")]
    [Range(0f, 360f)] public float possibleAngles = 360f;
    [Min(0)] public float spawnRadius = 2f, spawnDistance = 20f;

    /// <summary>
    /// 激活怪物事件，在玩家周围生成怪物
    /// </summary>
    /// <param name="player">玩家状态对象，用于确定怪物生成位置和方向。如果为null则不执行生成</param>
    /// <returns>返回false表示事件激活后不需要继续处理</returns>
    public override bool Activate(PlayerStats player = null)
    {
        // 只在玩家存在时激活。
        if (player)
        {
            // 否则，我们在屏幕外生成一个怪物并向玩家移动。
            float randomAngle = Random.Range(0, possibleAngles) * Mathf.Deg2Rad;
            
            // 根据随机角度和距离在玩家周围生成所有配置的怪物对象
            foreach (GameObject o in GetSpawns())
            {
                Instantiate(o, player.transform.position + new Vector3(
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Cos(randomAngle),
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Sin(randomAngle)
                ), Quaternion.identity);
            }
        }

        return false;
    }
}
