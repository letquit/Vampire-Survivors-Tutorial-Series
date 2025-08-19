using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount; // 鞭子在本次迭代中攻击了多少次。
    float currentSpawnYOffset; // 如果有超过2个鞭子，我们将开始向上偏移。

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
        
        // 否则，计算生成的弹道的角度和偏移量。
        // 然后，如果<currentSpawnCount>是偶数（即超过1个弹道），
        // 我们将翻转生成的方向。
        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
        );

        // 并生成一个弹道的副本。
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity
        );
        prefab.owner = owner; // 将我们自己设置为所有者。
        
        // 翻转弹道的精灵。
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
            );
            // Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        // 分配属性。
        prefab.weapon = this;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;

        // 确定下一个弹道应该生成的位置。
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
            currentSpawnYOffset += 1;
        
        // 我们是否执行另一次攻击？
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }
}
