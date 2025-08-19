using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 闪电环武器类，继承自ProjectileWeapon。
/// 该武器会随机选择屏幕上的敌人进行攻击，并在一定范围内造成伤害。
/// 目前伤害不会随着力量属性而增加。
/// </summary>
public class LightningRingWeapon : ProjectileWeapon
{
    List<EnemyStats> allSelectedEnemies = new List<EnemyStats>();

    /// <summary>
    /// 执行一次攻击操作。
    /// </summary>
    /// <param name="attackCount">本次攻击的次数，默认为1。</param>
    /// <returns>如果成功执行攻击则返回true，否则返回false。</returns>
    protected override bool Attack(int attackCount = 1)
    {
        // 如果没有分配命中效果预制体，请留下警告信息。
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning(string.Format("Hit effect prefab has not been set for {0}", name));
            currentCooldown = currentStats.cooldown;
            return false;
        }

        // 如果没有分配弹道，请将武器设置为冷却状态。
        if (!CanAttack()) return false;

        // 如果冷却时间小于等于0，这是武器的第一次发射。
        // 刷新选定敌人的数组。
        if (currentCooldown <= 0)
        {
            allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());
            currentCooldown += currentStats.cooldown;
            currentAttackCount = attackCount;
        }

        // 在地图中找到一个敌人进行闪电攻击。
        EnemyStats target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position, currentStats.area, GetDamage());

            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }
        
        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }

        // 如果我们有超过1次的攻击次数。
        if (attackCount > 0)
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }
    
    /// <summary>
    /// 随机从当前存活且可见的敌人列表中选取一个敌人作为攻击目标。
    /// </summary>
    /// <returns>选中的敌人组件，如果没有可选敌人则返回null。</returns>
    EnemyStats PickEnemy()
    {
        EnemyStats target = null;
        while (!target && allSelectedEnemies.Count > 0)
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            // 如果目标已经死亡，移除它并跳过。
            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue;
            }

            // 检查敌人是否在屏幕上。
            // 如果敌人缺少渲染器，无法被攻击，因为我们无法检查它是否在屏幕上。
            Renderer r = target.GetComponent<Renderer>();
            if (!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                target = null;
                continue;
            }
        }
        
        allSelectedEnemies.Remove(target);
        return target;
    }

    /// <summary>
    /// 对指定位置周围圆形区域内的所有敌人造成伤害。
    /// </summary>
    /// <param name="position">伤害区域的中心点坐标。</param>
    /// <param name="radius">伤害区域的半径。</param>
    /// <param name="damage">造成的伤害值。</param>
    void DamageArea(Vector2 position, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D t in targets)
        {
            EnemyStats es = t.GetComponent<EnemyStats>();
            if (es) es.TakeDamage(damage, transform.position);
        }
    }
}
