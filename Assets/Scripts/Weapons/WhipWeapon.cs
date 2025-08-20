using UnityEngine;

/// <summary>
/// 鞭子武器类，继承自ProjectileWeapon。用于处理鞭子类型的攻击逻辑。
/// </summary>
public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount; // 鞭子在本次迭代中攻击了多少次。
    float currentSpawnYOffset; // 如果有超过2个鞭子，我们将开始向上偏移。

    /// <summary>
    /// 执行一次或多次鞭子攻击。
    /// </summary>
    /// <param name="attackCount">需要连续执行的攻击次数，默认为1。</param>
    /// <returns>如果成功发起攻击则返回true，否则返回false。</returns>
    protected override bool Attack(int attackCount = 1)
    {
        // 如果没有分配弹道预制体，请留下警告信息。
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        // 如果没有分配弹道，请将武器设置为冷却状态。
        if (!CanAttack()) return false;

        // 如果这是第一次发动攻击，
        // 我们重置当前的生成计数。
        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }
        
        // 计算弹道生成方向和偏移位置：
        // - 根据角色最后移动方向决定基本方向；
        // - 若当前已生成奇数个弹道，则翻转方向；
        // - Y轴偏移根据当前生成计数进行调整。
        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
        );

        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }
        
        // 实例化弹道预制体并设置初始属性。
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity
        );
        prefab.owner = owner; // 设置弹道的所有者。

        // 如果是向左发射，翻转弹道精灵的朝向。
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
            );
        }

        // 设置弹道关联的武器引用。
        prefab.weapon = this;
        ActivateCooldown(true);
        attackCount--;

        // 更新下次弹道生成的位置参数。
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
            currentSpawnYOffset += 1;
        
        // 判断是否需要继续执行剩余的攻击次数。
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }
}
