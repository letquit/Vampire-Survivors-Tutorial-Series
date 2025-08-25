using UnityEngine;

/// <summary>
/// 充电型敌人移动控制类，继承自EnemyMovement
/// 该类实现敌人生成时朝玩家位置进行直线冲锋的移动逻辑
/// </summary>
public class ChargingEnemyMovement : EnemyMovement
{
    private Vector2 chargeDirection;

    /// <summary>
    /// 初始化敌人移动方向
    /// 在敌人生成时计算从敌人位置指向玩家位置的标准化方向向量
    /// 该方向将作为敌人冲锋的固定方向
    /// </summary>
    protected override void Start()
    {
        base.Start();
        // 计算敌人生成时玩家相对于敌人的位置方向，并标准化为单位向量
        chargeDirection = (player.transform.position - transform.position).normalized;
    }
    
    protected override void Update()
    {
        switch (currentState)
        {
            case EnemyState.Knockback:
                if (knockbackDuration > 0)
                {
                    transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
                    knockbackDuration -= Time.deltaTime;
                }
                else
                {
                    currentState = previousState;
                }
                break;
            
            case EnemyState.Moving:
            case EnemyState.Charging:
                Move();
                HandleOutOfFrameAction();
                break;
        }
    }

    /// <summary>
    /// 执行敌人移动逻辑
    /// 敌人沿着初始化时计算的固定方向进行直线移动，不跟随玩家位置变化
    /// </summary>
    public override void Move()
    {
        // 按照固定冲锋方向移动敌人，移动距离 = 速度 × 时间
        transform.position += (Vector3)chargeDirection * stats.Actual.moveSpeed * Time.deltaTime;
    }
    
    /// <summary>
    /// 充电型敌人不接受击退效果
    /// </summary>
    /// <param name="velocity">击退速度向量</param>
    /// <param name="duration">击退持续时间（秒）</param>
    public override void Knockback(Vector2 velocity, float duration)
    {
        // 充电型敌人不受击退影响，因此不执行任何操作
        return;
    }
}
