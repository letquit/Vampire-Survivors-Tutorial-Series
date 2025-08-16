using UnityEngine;

/// <summary>
/// 拾取物品类，用于处理玩家拾取物品的逻辑
/// </summary>
public class Pickup : MonoBehaviour, ICollectible
{
    public bool hasBeenCollected = false;

    /// <summary>
    /// 收集物品的方法，将物品标记为已收集状态
    /// </summary>
    public virtual void Collect()
    {
        hasBeenCollected = true; 
    }
    
    /// <summary>
    /// 当触发器碰撞发生时调用此方法，用于检测玩家拾取物品
    /// </summary>
    /// <param name="other">与当前对象发生碰撞的另一个碰撞器对象</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的对象是否为玩家标签
        if (other.CompareTag("Player"))
        {
            // 销毁当前游戏对象（表示物品已被拾取）
            Destroy(gameObject);
        }
    }
}

