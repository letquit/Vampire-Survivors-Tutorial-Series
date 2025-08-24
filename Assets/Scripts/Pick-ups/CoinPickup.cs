/// <summary>
/// CoinPickup类继承自Pickup类，用于处理玩家拾取硬币的逻辑
/// 当硬币对象被销毁时，会将指定数量的硬币添加到拾取玩家的收集器中
/// </summary>
public class CoinPickup : Pickup
{
    PlayerCollector collector;
    public int coins = 1;

    /// <summary>
    /// 当对象被销毁时调用此方法
    /// 重写了基类的OnDestroy方法，用于在玩家拾取硬币后增加玩家的硬币数量
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // 检查是否有目标玩家拾取了此硬币，如果有则为其添加硬币数量
        if (target != null)
        {
            collector = target.GetComponentInChildren<PlayerCollector>();
            if (collector != null) collector.AddCoins(coins);
        }
    }
}
