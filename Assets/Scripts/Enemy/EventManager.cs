using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理游戏中的事件系统，包括事件的触发、持续时间控制和冷却逻辑。
/// </summary>
public class EventManager : MonoBehaviour
{
    float currentEventCooldown = 0;

    public EventData[] events;

    [Tooltip("在激活之前需要等待的时间。")]
    public float firstTriggerDelay = 180f;

    [Tooltip("每次事件之间的等待时间。")]
    public float triggerInterval = 30f;

    public static EventManager instance;

    /// <summary>
    /// 表示一个正在运行的游戏事件，包含事件数据、持续时间和冷却时间。
    /// </summary>
    [System.Serializable]
    public class Event
    {
        public EventData data;
        public float duration, cooldown = 0;
    }
    List<Event> runningEvents = new List<Event>(); // 这些是已激活并正在运行的事件。

    PlayerStats[] allPlayers;

    /// <summary>
    /// 在第一次帧更新前被调用，用于初始化事件管理器实例和玩家统计信息。
    /// </summary>
    void Start()
    {
        if (instance) Debug.LogWarning("场景中有多个事件管理器！请移除多余的实例。");
        instance = this;
        currentEventCooldown = firstTriggerDelay > 0 ? firstTriggerDelay : triggerInterval;
        allPlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
    }

    /// <summary>
    /// 每帧调用一次，用于处理事件的冷却、触发和更新逻辑。
    /// 包括检查是否应该触发新事件以及更新当前运行事件的状态。
    /// </summary>
    void Update()
    {
        // 添加另一个事件到计划中的冷却时间。
        currentEventCooldown -= Time.deltaTime;
        if (currentEventCooldown <= 0)
        {
            // 获取一个事件并尝试执行它。
            EventData e = GetRandomEvent();
            if (e && e.CheckIfWillHappen(allPlayers[Random.Range(0, allPlayers.Length)]))
                runningEvents.Add(new Event
                {
                    data = e,
                    duration = e.duration
                });

            // 设置事件的冷却时间。
            currentEventCooldown = triggerInterval;
        }

        // 我们想要移除的事件。
        List<Event> toRemove = new List<Event>();

        // 现有事件的冷却时间，以查看它们是否应该继续运行。
        foreach (Event e in runningEvents)
        {
            // 减少当前持续时间。
            e.duration -= Time.deltaTime;
            if (e.duration <= 0)
            {
                toRemove.Add(e);
                continue;
            }

            // 减少当前冷却时间。
            e.cooldown -= Time.deltaTime;
            if (e.cooldown <= 0)
            {
                // 随机选择一个玩家来激活这个事件，
                // 然后重置冷却时间。
                e.data.Activate(allPlayers[Random.Range(0, allPlayers.Length)]);
                e.cooldown = e.data.GetSpawnInterval();
            }
        }

        // 移除所有已过期的事件。
        foreach (Event e in toRemove) runningEvents.Remove(e);
    }

    /// <summary>
    /// 从可用事件列表中随机获取一个可以激活的事件。
    /// </summary>
    /// <returns>可激活的事件数据，如果没有可用事件则返回null。</returns>
    public EventData GetRandomEvent()
    {
        // 如果没有分配事件，则不返回任何内容。
        if (events.Length <= 0) return null;

        // 获取所有可能事件的列表。
        List<EventData> possibleEvents = new List<EventData>(events);

        // 随机选择一个事件并检查是否可以使用。
        // 一直这样做，直到我们找到一个合适的事件。
        EventData result = possibleEvents[Random.Range(0, possibleEvents.Count)];
        while (!result.IsActive())
        {
            possibleEvents.Remove(result);
            if (possibleEvents.Count > 0)
                result = events[Random.Range(0, possibleEvents.Count)];
            else
                return null;
        }
        return result;
    }
}
