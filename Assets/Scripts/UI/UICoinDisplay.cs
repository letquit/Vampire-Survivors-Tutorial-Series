using TMPro;
using UnityEngine;

/// <summary>
/// 附加到GameObject上的组件，用于显示玩家的硬币。
/// 无论是在游戏中还是玩家拥有的总硬币数，这取决于collector变量是否已设置。
/// </summary>
public class UICoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI displayTarget;
    /// <summary>
    /// 玩家收集器组件，用于获取游戏中当前收集的硬币数量
    /// </summary>
    public PlayerCollector collector;

    /// <summary>
    /// 组件初始化方法，在游戏对象启动时调用
    /// 获取子对象中的TextMeshProUGUI组件引用并更新显示内容
    /// </summary>
    void Start()
    {
        displayTarget = GetComponentInChildren<TextMeshProUGUI>();
        UpdateDisplay();
    }
    
    /// <summary>
    /// 更新硬币显示内容
    /// 根据是否分配了收集器来决定显示游戏中的硬币数量还是保存的总硬币数量
    /// </summary>
    public void UpdateDisplay()
    {
        // 如果分配了收集器，我们将显示收集器拥有的硬币数量
        if (collector != null)
        {
            displayTarget.text = Mathf.RoundToInt(collector.GetCoins()).ToString();
        }
        else
        {
            // 否则，我们将获取当前保存的硬币数量
            float coins = SaveManager.LastLoadedGameData.coins;
            displayTarget.text = Mathf.RoundToInt(coins).ToString();
        }
    }
}
