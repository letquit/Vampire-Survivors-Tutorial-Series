using UnityEngine;

/// <summary>
/// 玩家控制器类，用于控制2D平台角色的移动、跳跃和朝向翻转。
/// </summary>
public class SmlwPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    /// <summary>
    /// 角色水平移动速度。
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// 跳跃时施加的垂直力大小。
    /// </summary>
    public float jumpForce = 10f;
    
    [Header("Ground Check")]
    /// <summary>
    /// 用于检测是否接触地面的检查点Transform组件。
    /// </summary>
    public Transform groundCheck;

    /// <summary>
    /// 地面检测的圆形范围半径。
    /// </summary>
    public float groundCheckRadius = 0.2f;

    /// <summary>
    /// 可以被识别为“地面”的图层掩码。
    /// </summary>
    public LayerMask groundLayerMask = 1 << 0;
    
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isGrounded = false;
    
    /// <summary>
    /// 初始化组件引用，并在必要时自动创建地面检测点。
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 如果未指定地面检测点，则动态创建一个默认位置的检测点
        if (groundCheck == null)
        {
            groundCheck = new GameObject("GroundCheck").transform;
            groundCheck.SetParent(transform);
            groundCheck.position = new Vector2(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().bounds.extents.y);
        }
    }
    
    /// <summary>
    /// 每帧更新逻辑：处理输入跳跃指令并进行接地状态检测。
    /// </summary>
    void Update()
    {
        CheckGrounded();
        
        // 当按下跳跃键且处于地面时执行跳跃动作
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    
    /// <summary>
    /// 固定时间步长中调用：根据水平轴输入控制角色移动。
    /// </summary>
    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Move(moveInput);
    }
    
    /// <summary>
    /// 控制角色左右移动，并根据方向调整面向。
    /// </summary>
    /// <param name="direction">水平移动方向（-1 表示左，1 表示右，0 表示无输入）</param>
    void Move(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        
        // 根据移动方向判断是否需要翻转角色朝向
        if (direction > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction < 0 && isFacingRight)
        {
            Flip();
        }
    }
    
    /// <summary>
    /// 执行跳跃操作，给刚体施加向上的瞬时速度。
    /// </summary>
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    
    /// <summary>
    /// 使用物理重叠球检测当前是否站在地面上。
    /// </summary>
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }
    
    /// <summary>
    /// 翻转角色的朝向（左右镜像），通过缩放x轴实现。
    /// </summary>
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    /// <summary>
    /// 在编辑器选中该对象时绘制地面检测区域的可视化辅助线。
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}