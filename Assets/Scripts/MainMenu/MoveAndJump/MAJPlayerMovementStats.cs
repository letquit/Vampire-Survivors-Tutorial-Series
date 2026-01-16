using System;
using UnityEngine;

/// <summary>
/// 玩家移动状态脚本对象，用于存储和管理玩家的各种移动参数
/// </summary>
[CreateAssetMenu(menuName = "Player Movement")]
public class MAJPlayerMovementStats : ScriptableObject
{
    [Header("Walk")] 
    /// <summary>
    /// 最大行走速度
    /// </summary>
    [Range(1f, 100f)] public float MaxWalkSpeed = 12.5f;
    /// <summary>
    /// 地面加速
    /// </summary>
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    /// <summary>
    /// 地面减速
    /// </summary>
    [Range(0.25f, 50f)] public float GroundDeceleration = 5f;
    /// <summary>
    /// 空中加速
    /// </summary>
    [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    /// <summary>
    /// 空中减速
    /// </summary>
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;
    
    [Header("Run")]
    /// <summary>
    /// 最大跑步速度
    /// </summary>
    [Range(1f, 100f)] public float MaxRunSpeed = 20f;

    [Header("Grounded/Collision Checks")] 
    /// <summary>
    /// 地面层遮罩
    /// </summary>
    public LayerMask GroundLayer;
    /// <summary>
    /// 地面检测射线长度
    /// </summary>
    public float GroundDetectionRayLength = 0.02f;
    /// <summary>
    /// 头部检测射线长度
    /// </summary>
    public float HeadDetectionRayLength = 0.02f;
    /// <summary>
    /// 头部宽度
    /// </summary>
    [Range(0f, 1f)] public float HeadWidth = 0.75f;
    
    [Header("Jump")]
    /// <summary>
    /// 跳跃高度
    /// </summary>
    public float JumpHeight = 6.5f;
    /// <summary>
    /// 跳跃高度补偿因子
    /// </summary>
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    /// <summary>
    /// 到达跳跃顶点的时间
    /// </summary>
    public float TimeTillJumpApex = 0.35f;
    /// <summary>
    /// 释放跳跃时重力倍增器
    /// </summary>
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    /// <summary>
    /// 最大下落速度
    /// </summary>
    public float MaxFallSpeed = 26f;
    /// <summary>
    /// 允许的跳跃次数
    /// </summary>
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    /// <summary>
    /// 向上取消跳跃的时间
    /// </summary>
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    /// <summary>
    /// 顶点阈值
    /// </summary>
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    /// <summary>
    /// 顶点悬停时间
    /// </summary>
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    /// <summary>
    /// 跳跃缓冲时间
    /// </summary>
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    /// <summary>
    /// 跳跃延迟时间（边缘跳跃时间）
    /// </summary>
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Debug")]
    /// <summary>
    /// 是否显示地面检测框
    /// </summary>
    public bool DebugShowIsGroundedBox;
    /// <summary>
    /// 是否显示头部碰撞框
    /// </summary>
    public bool DebugShowHeadBumpBox;

    [Header("JumpVisualization Tool")]
    /// <summary>
    /// 是否显示行走跳跃弧线
    /// </summary>
    public bool ShowWalkJumpArc = false;
    /// <summary>
    /// 是否显示跑步跳跃弧线
    /// </summary>
    public bool ShowRunJumpArc = false;
    /// <summary>
    /// 是否在碰撞时停止
    /// </summary>
    public bool StopOnCollision = true;
    /// <summary>
    /// 是否向右绘制
    /// </summary>
    public bool DrawRight = true;
    /// <summary>
    /// 弧线分辨率
    /// </summary>
    [Range(5, 100)] public int ArcResolution = 20;
    /// <summary>
    /// 可视化步数
    /// </summary>
    [Range(0, 500)] public int VisualizationSteps = 90;
    
    /// <summary>
    /// 获取计算后的重力值
    /// </summary>
    public float Gravity { get; private set; }
    /// <summary>
    /// 获取初始跳跃速度
    /// </summary>
    public float InitialJumpVelocity { get; private set; }
    
    /// <summary>
    /// 获取调整后的跳跃高度
    /// </summary>
    public float AdjustedJumpHeight { get; private set; }

    /// <summary>
    /// 验证时调用，重新计算相关数值
    /// </summary>
    private void OnValidate()
    {
        CalculateValues();
    }

    /// <summary>
    /// 脚本启用时调用，重新计算相关数值
    /// </summary>
    private void OnEnable()
    {
        CalculateValues();
    }

    /// <summary>
    /// 计算重力、初始跳跃速度和调整后跳跃高度等派生值
    /// </summary>
    private void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }
}