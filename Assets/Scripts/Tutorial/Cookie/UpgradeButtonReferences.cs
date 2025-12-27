using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 升级按钮引用组件类，用于管理升级界面中按钮及其相关文本元素的引用
/// </summary>
public class UpgradeButtonReferences : MonoBehaviour
{
    /// <summary>
    /// 升级按钮的引用
    /// </summary>
    public Button upgradeButton;
    
    /// <summary>
    /// 升级按钮文本的引用
    /// </summary>
    public TextMeshProUGUI upgradeButtonText;
    
    /// <summary>
    /// 升级描述文本的引用
    /// </summary>
    public TextMeshProUGUI upgradeDescriptionText;
    
    /// <summary>
    /// 升级成本文本的引用
    /// </summary>
    public TextMeshProUGUI upgradeCostText;
}