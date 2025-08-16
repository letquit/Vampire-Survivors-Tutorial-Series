using System;
using UnityEngine;

/// <summary>
/// BobbingAnimation类用于实现物体的摆动动画效果
/// 该类通过正弦函数控制物体在指定方向上的周期性移动，创建浮动或摆动的视觉效果
/// </summary>
public class BobbingAnimation : MonoBehaviour
{
    public float frequency;
    public float magnitude;
    public Vector3 direction;
    private Vector3 initialPosition;
    private Pickup pickup;
    
    /// <summary>
    /// Start函数在物体启用时调用，用于初始化物体的初始位置
    /// 记录物体的本地坐标位置作为动画的基准点
    /// </summary>
    private void Start()
    {
        pickup = GetComponent<Pickup>();
        initialPosition = transform.localPosition;
    }

    /// <summary>
    /// Update函数在每帧调用，用于更新物体的位置实现摆动动画
    /// 通过正弦函数计算偏移量，使物体在指定方向上按频率和幅度进行周期性移动
    /// </summary>
    private void Update()
    {
        if (pickup && !pickup.hasBeenCollected)
        {
            // 根据正弦波函数计算当前位置：初始位置 + 方向向量 * 正弦值 * 幅度
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
        }
    }
}
