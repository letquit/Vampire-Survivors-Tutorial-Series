using System;
using UnityEngine;

/// <summary>
/// 每秒饼干计时器，用于定期增加饼干数量
/// </summary>
public class CookiePerSecondTimer : MonoBehaviour
{
    /// <summary>
    /// 计时器持续时间，决定多久增加一次饼干
    /// </summary>
    public float timerDuration = 1f;
    
    /// <summary>
    /// 每秒获得的饼干数量
    /// </summary>
    public double CookiePerSecond { get; set; }

    /// <summary>
    /// 计时器计数器，用于跟踪经过的时间
    /// </summary>
    private float _counter;

    /// <summary>
    /// Unity更新方法，用于处理计时器逻辑
    /// </summary>
    private void Update()
    {
        // 累加经过的时间
        _counter += Time.deltaTime;

        // 检查是否达到计时器持续时间
        if (_counter >= timerDuration)
        {
            // 增加饼干数量
            CookieManager.Instance.SimpleCookieIncrease(CookiePerSecond);
            
            // 重置计数器
            _counter = 0f;
        }
    }
}