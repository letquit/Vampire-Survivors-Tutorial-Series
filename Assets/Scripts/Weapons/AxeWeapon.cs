using UnityEngine;

/// <summary>
/// 斧头武器类，继承自投射物武器基类
/// </summary>
public class AxeWeapon : ProjectileWeapon
{
    /// <summary>
    /// 获取生成角度
    /// </summary>
    /// <returns>返回计算出的生成角度值</returns>
    protected override float GetSpawnAngle()
    {
        // 计算角度偏移量：当当前攻击次数大于0时，偏移量为武器数量减去当前攻击次数，否则为0
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        // 根据角色面向和偏移量计算最终生成角度
        return 90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * offset);
    }

    /// <summary>
    /// 获取生成位置偏移量
    /// </summary>
    /// <param name="spawnAngle">生成角度参数（本函数中未使用）</param>
    /// <returns>返回随机生成的位置偏移向量</returns>
    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
