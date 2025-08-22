using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 光环类，继承自武器效果类。用于对进入其范围内的敌人持续造成伤害。
/// 同时也支持水元素等其他效果。
/// </summary>
public class Aura : WeaponEffect
{
    /// <summary>
    /// 存储当前受光环影响的敌人及其冷却时间的字典。
    /// 键为敌人的 EnemyStats 组件，值为剩余冷却时间（秒）。
    /// </summary>
    private Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();

    /// <summary>
    /// 存储即将从光环影响中移除的敌人列表。
    /// 用于延迟移除，确保冷却时间正确处理。
    /// </summary>
    private List<EnemyStats> targetsToUnafffect = new List<EnemyStats>();

    /// <summary>
    /// 每帧调用一次，用于更新光环对敌人的影响。
    /// 遍历所有受影响的敌人，减少其冷却时间，若冷却结束则造成伤害。
    /// </summary>
    private void Update()
    {
        // 创建一个副本以避免在遍历时修改原字典
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        // 遍历每一个受光环影响的目标，并减少其冷却时间
        // 如果冷却时间归零，则对其造成伤害
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetsToUnafffect.Contains(pair.Key))
                {
                    // 如果目标已被标记为移除，则从影响列表中删除
                    affectedTargets.Remove(pair.Key);
                    targetsToUnafffect.Remove(pair.Key);
                }
                else
                {
                    // 重置冷却时间并造成伤害
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown * Owner.Stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                    
                    weapon.ApplyBuffs(pair.Key);
                    
                    if (stats.hitEffect)
                    {
                        Destroy(Instantiate(stats.hitEffect, pair.Key.transform.position, Quaternion.identity), 5f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 当其他物体进入触发器时调用。
    /// 如果是敌人，则将其加入光环影响列表。
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats es))
        {
            // 如果敌人尚未被光环影响，则添加到影响列表中
            if (!affectedTargets.ContainsKey(es))
            {
                // 初始冷却时间为0，确保在下一帧立即造成伤害
                affectedTargets.Add(es, 0);
            }
            else
            {
                // 如果敌人之前被标记为移除，则取消该标记
                if (targetsToUnafffect.Contains(es))
                {
                    targetsToUnafffect.Remove(es);
                }
            }
        }
    }

    /// <summary>
    /// 当其他物体离开触发器时调用。
    /// 如果是敌人，则将其标记为待移除状态。
    /// </summary>
    /// <param name="other">离开触发器的碰撞体</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats es))
        {
            // 不立即移除敌人，因为还需要跟踪其冷却时间
            if (affectedTargets.ContainsKey(es))
            {
                targetsToUnafffect.Add(es);
            }
        }
    }
}
