using UnityEngine;

/// <summary>
/// 事件数据抽象类，继承自SpawnData，用于定义游戏中各种事件的基础数据和行为
/// </summary>
public abstract class EventData : SpawnData
{
    [Header("事件数据")]
    [Range(0f, 1f)] public float probability = 1f; // 此事件是否发生。
    [Range(0f, 1f)] public float luckFactor = 1f; // 幸运值对事件概率的影响程度。

    [Tooltip("如果指定了一个值，此事件仅在关卡运行指定秒数后才会发生。")]
    public float activeAfter = 0;

    /// <summary>
    /// 激活事件的抽象方法，由子类实现具体的事件激活逻辑
    /// </summary>
    /// <param name="player">玩家状态对象，可为空</param>
    /// <returns>事件激活是否成功</returns>
    public abstract bool Activate(PlayerStats player = null);

    /// <summary>
    /// 检查此事件当前是否处于活动状态
    /// </summary>
    /// <returns>如果事件处于活动状态返回true，否则返回false</returns>
    public bool IsActive()
    {
        if (!GameManager.instance) return false;
        if (GameManager.instance.GetElapsedTime() > activeAfter) return true;
        return false;
    }
    
    /// <summary>
    /// 计算此事件发生的随机概率
    /// </summary>
    /// <param name="s">玩家状态对象，用于获取幸运值等属性</param>
    /// <returns>如果事件应该发生返回true，否则返回false</returns>
    public bool CheckIfWillHappen(PlayerStats s)
    {
        // 概率为1表示总是发生。
        if (probability >= 1) return true;

        // 否则，获取一个随机数并检查是否通过概率测试。
        if (probability / Mathf.Max(1, (s.Stats.luck * luckFactor)) >= Random.Range(0f, 1f))
            return true;

        return false;
    }
}
