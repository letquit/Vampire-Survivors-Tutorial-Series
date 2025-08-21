using UnityEngine;

/// <summary>
/// 投射物武器类，继承自 Weapon。用于处理发射投射物的武器逻辑。
/// </summary>
public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval; // 当前攻击间隔计时器
    protected int currentAttackCount; // 当前攻击次数计数器，表示本次攻击还将发生多少次

    /// <summary>
    /// 每帧更新逻辑。继承自基类 Update，并处理攻击间隔计时和触发攻击。
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 如果当前攻击间隔大于0，则递减时间，若减到0或以下则触发攻击
        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    /// <summary>
    /// 判断当前是否可以进行攻击。
    /// </summary>
    /// <returns>如果可以攻击返回 true，否则返回 false。</returns>
    public override bool CanAttack()
    {
        // 如果当前还有剩余攻击次数，则可以攻击
        if (currentAttackCount > 0) return true;
        // 否则调用基类的 CanAttack 方法判断
        return base.CanAttack();
    }

    /// <summary>
    /// 执行一次攻击逻辑，包括生成投射物、设置冷却等。
    /// </summary>
    /// <param name="attackCount">本次攻击的剩余次数，默认为1。</param>
    /// <returns>如果成功执行攻击返回 true，否则返回 false。</returns>
    protected override bool Attack(int attackCount = 1)
    {
        // 如果没有设置投射物预制体，则输出警告并重置冷却时间
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("{0} 的投射物预制体未设置", name));
            ActivateCooldown(true);
            return false;
        }
        
        // 判断是否可以攻击
        if (!CanAttack()) return false;

        // 计算投射物生成时的角度
        float spawnAngle = GetSpawnAngle();

        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }
        
        // 实例化投射物预制体，并设置位置和旋转
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );

        // 设置投射物的武器和拥有者
        prefab.weapon = this;
        prefab.owner = owner;

        ActivateCooldown(true);

        attackCount--;

        // 如果还有剩余攻击次数，则设置下次攻击的相关参数
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }

        return true;
    }
    
    /// <summary>
    /// 获取投射物生成时应朝向的角度。
    /// </summary>
    /// <returns>投射物的生成角度（单位：度）。</returns>
    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }
    
    /// <summary>
    /// 获取投射物生成时的位置偏移，并根据角度进行旋转。
    /// </summary>
    /// <param name="spawnAngle">投射物的生成角度。</param>
    /// <returns>生成点的偏移向量。</returns>
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
