using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人相关工具类，提供公共的敌人位置计算等方法
/// </summary>
public static class EnemyUtilities
{
    /// <summary>
    /// 获取环形区域内的随机位置
    /// </summary>
    /// <param name="minDistance">最小距离</param>
    /// <param name="maxDistance">最大距离</param>
    /// <returns>相对于中心点的偏移位置</returns>
    public static Vector3 GetRandomPositionInRange(float minDistance, float maxDistance)
    {
        // 随机生成角度 (0到360度)
        float angle = Random.Range(0f, 360f);
        
        // 随机生成距离 (在指定范围内)
        float distance = Random.Range(minDistance, maxDistance);
        
        // 将极坐标转换为直角坐标
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
        
        return new Vector3(x, y, 0f);
    }
    
    /// <summary>
    /// 获取默认的敌人生成位置
    /// </summary>
    /// <param name="minDistance">最小距离</param>
    /// <param name="maxDistance">最大距离</param>
    /// <returns>相对于中心点的偏移位置</returns>
    public static Vector3 GetRandomEnemyPosition(float minDistance, float maxDistance)
    {
        return GetRandomPositionInRange(minDistance, maxDistance);
    }
    
    /// <summary>
    /// 获取默认的敌人生成位置(5-8个单位距离)
    /// </summary>
    /// <returns>相对于中心点的偏移位置</returns>
    public static Vector3 GetRandomEnemyPosition()
    {
        return GetRandomPositionInRange(5f, 8f);
    }
}
