using System;
using UnityEngine;

/// <summary>
/// 玩家移动控制类，用于处理玩家角色的二维移动逻辑
/// 该类通过监听输入轴来控制刚体的移动，并记录最后的移动方向
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    //Movement
    /// <summary>
    /// 移动速度，用于控制玩家移动的快慢
    /// </summary>
    public float moveSpeed;
    private Rigidbody2D rb;
    /// <summary>
    /// 隐藏字段，记录最后的水平移动方向
    /// </summary>
    [HideInInspector]
    public float lastHorizontalVector;
    /// <summary>
    /// 隐藏字段，记录最后的垂直移动方向
    /// </summary>
    [HideInInspector]
    public float lastVerticalVector;
    /// <summary>
    /// 隐藏字段，存储当前的移动方向向量
    /// </summary>
    [HideInInspector]
    public Vector2 moveDir;

    /// <summary>
    /// 初始化函数，在游戏对象启用时调用
    /// 获取并缓存Rigidbody2D组件引用
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 每帧更新函数
    /// 负责处理输入管理逻辑
    /// </summary>
    private void Update()
    {
        InputManagement();
    }

    /// <summary>
    /// 固定更新函数，每物理帧调用
    /// 负责执行实际的移动逻辑
    /// </summary>
    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// 输入管理函数
    /// 获取玩家的输入轴值，计算移动方向向量，并更新最后的移动方向记录
    /// </summary>
    private void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        
        moveDir = new Vector2(moveX, moveY).normalized;

        // 当存在水平移动时，更新最后的水平移动方向
        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
        }
        
        // 当存在垂直移动时，更新最后的垂直移动方向
        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
        }
    }

    /// <summary>
    /// 移动函数
    /// 根据移动方向和速度设置刚体的线性速度
    /// </summary>
    private void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
}

