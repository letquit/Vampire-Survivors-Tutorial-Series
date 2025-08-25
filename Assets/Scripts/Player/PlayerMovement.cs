using System;
using Terresquall;
using UnityEngine;

/// <summary>
/// 玩家移动控制类，用于处理玩家角色的二维移动逻辑
/// 该类通过监听输入轴来控制刚体的移动，并记录最后的移动方向
/// </summary>
public class PlayerMovement : Sortable
{
    // 玩家状态枚举
    public enum PlayerState
    {
        Normal,
        Invincible,
        Knockback,
        Dead,
        LevelUp,
        Paused
    }
    
    public PlayerState currentState = PlayerState.Normal;
    public PlayerState previousState = PlayerState.Normal;
    
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
    
    // 击退相关变量
    private Vector2 knockbackVelocity;
    private float knockbackDuration;

    /// <summary>
    /// 初始化函数，在游戏对象启用时调用
    /// 获取并缓存Rigidbody2D组件引用
    /// </summary>
    protected override void Start()
    {
        base.Start();
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
        switch (currentState)
        {
            case PlayerState.Normal:
            case PlayerState.Invincible:
            case PlayerState.LevelUp:
                InputManagement();
                break;
                
            case PlayerState.Knockback:
                HandleKnockback();
                break;
                
            case PlayerState.Dead:
            case PlayerState.Paused:
                moveDir = Vector2.zero;
                break;
        }
    }

    /// <summary>
    /// 固定更新函数，每物理帧调用
    /// 负责执行实际的移动逻辑
    /// </summary>
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
            case PlayerState.Invincible:
            case PlayerState.LevelUp:
                Move();
                break;
                
            case PlayerState.Knockback:
                MoveWithKnockback();
                break;
                
            case PlayerState.Dead:
            case PlayerState.Paused:
                // 不移动
                break;
        }
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
    
    /// <summary>
    /// 处理击退效果
    /// </summary>
    private void HandleKnockback()
    {
        if (knockbackDuration > 0)
        {
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // 击退结束，恢复到之前状态
            currentState = previousState;
        }
    }
    
    /// <summary>
    /// 带击退效果的移动
    /// </summary>
    private void MoveWithKnockback()
    {
        if (rb)
        {
            rb.MovePosition(rb.position + knockbackVelocity * Time.fixedDeltaTime);
        }
        else
        {
            transform.position += (Vector3)knockbackVelocity * Time.fixedDeltaTime;
        }
        Move(); // 也可以同时保持正常移动
    }
    
    /// <summary>
    /// 应用击退效果
    /// </summary>
    /// <param name="velocity">击退速度</param>
    /// <param name="duration">击退持续时间</param>
    public void Knockback(Vector2 velocity, float duration)
    {
        previousState = currentState;
        currentState = PlayerState.Knockback;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
    
    /// <summary>
    /// 检查玩家是否处于可移动状态
    /// </summary>
    public bool CanMove()
    {
        return currentState == PlayerState.Normal || 
               currentState == PlayerState.Invincible || 
               currentState == PlayerState.LevelUp;
    }
    
    /// <summary>
    /// 检查玩家是否处于无敌状态
    /// </summary>
    public bool IsInvincible()
    {
        return currentState == PlayerState.Invincible || 
               currentState == PlayerState.Dead ||
               player != null && player.GetType().GetField("isInvincible")?.GetValue(player) as bool? == true;
    }
    
    /// <summary>
    /// 检查玩家是否已经死亡
    /// </summary>
    public bool IsDead()
    {
        return currentState == PlayerState.Dead;
    }
}
