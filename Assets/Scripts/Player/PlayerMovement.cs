using System;
using Terresquall;
using UnityEngine;

/// <summary>
/// 玩家移动控制类，用于处理玩家角色的二维移动逻辑
/// 该类通过监听输入轴来控制刚体的移动，并记录最后的移动方向
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 5f;
    
    //Movement
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

    [HideInInspector]
    public Vector2 lastMovedVector;
    
    //References
    private Rigidbody2D rb;
    private PlayerStats player;

    /// <summary>
    /// 初始化函数，在游戏对象启用时调用
    /// 获取并缓存Rigidbody2D组件引用
    /// </summary>
    private void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
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
        /// 如果游戏已结束，则不处理输入
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX, moveY;
        if (VirtualJoystick.CountActiveInstances() > 0)
        {
            moveX = VirtualJoystick.GetAxisRaw("Horizontal");
            moveY = VirtualJoystick.GetAxisRaw("Vertical");
        }
        else
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");   
        }

        moveDir = new Vector2(moveX, moveY).normalized;

        /// 如果存在水平方向输入，则更新最后水平移动方向
        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);    //Last moved x
        }

        /// 如果存在垂直方向输入，则更新最后垂直移动方向
        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);    //Last moved y
        }

        /// 如果当前有移动输入，则更新最后移动向量为当前方向
        if (moveDir != Vector2.zero)
        {
            lastMovedVector = moveDir;
        }
    }

    /// <summary>
    /// 移动函数
    /// 根据移动方向和速度设置刚体的线性速度
    /// </summary>
    private void Move()
    {
        /// 如果游戏已结束，则停止移动
        if (GameManager.instance.isGameOver)
        {
            return;
        }
        
        rb.linearVelocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
}
