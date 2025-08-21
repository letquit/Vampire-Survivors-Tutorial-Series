using UnityEngine;

/// <summary>
/// 环形事件数据类，用于在玩家周围环形生成怪物的事件
/// 继承自EventData基类，可在Unity编辑器中通过菜单创建对应的ScriptableObject资源
/// </summary>
[CreateAssetMenu(fileName = "Ring Event Data", menuName = "2D Top-down Rogue-like/Event Data/Ring")]
public class RingEventData : EventData
{
    [Header("怪物数据")] public ParticleSystem spawnEffectPrefab;
    public Vector2 scale = new Vector2(1, 1);
    [Min(0)] public float spawnRadius = 10f, lifespan = 15f;

    /// <summary>
    /// 激活环形生成事件，在玩家周围环形生成怪物
    /// </summary>
    /// <param name="player">玩家状态对象，用于确定生成位置的中心点</param>
    /// <returns>始终返回false，表示事件激活后不需要额外处理</returns>
    public override bool Activate(PlayerStats player = null)
    {
        // 只在玩家存在时激活。
        if (player)
        {
            GameObject[] spawns = GetSpawns();
            
            // 计算每个生成点之间的角度间隔，确保怪物均匀分布
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, spawns.Length);
            float currentAngle = 0;
            
            // 遍历所有要生成的怪物预制体
            foreach (GameObject g in spawns)
            {
                // 计算生成位置。
                Vector3 spawnPosition = player.transform.position + new Vector3(
                    spawnRadius * Mathf.Cos(currentAngle) * scale.x,
                    spawnRadius * Mathf.Sin(currentAngle) * scale.y
                );

                // 如果指定了粒子效果，在该位置播放粒子效果。
                if (spawnEffectPrefab)
                    Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);

                // 然后生成敌人。
                GameObject s = Instantiate(g, spawnPosition, Quaternion.identity);

                // 如果怪物有生命周期，设置它们在生命周期结束后被销毁。
                if (lifespan > 0) Destroy(s, lifespan);

                currentAngle += angleOffset;
            }
        }

        return false;
    }
}
