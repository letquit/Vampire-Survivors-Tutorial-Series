using UnityEngine;

/// <summary>
/// 初始化升级系统的脚本，负责创建升级按钮并设置其初始状态
/// </summary>
public class InitializeUpgrades : MonoBehaviour
{
    /// <summary>
    /// 初始化升级系统，为每个升级项创建UI按钮并设置初始属性
    /// </summary>
    /// <param name="upgrades">升级数组，包含所有可购买的升级项数据</param>
    /// <param name="uiToSpawn">要实例化的UI预制体对象</param>
    /// <param name="spawnParent">新创建的UI对象的父级变换组件</param>
    public void Initialize(CookieUpgrade[] upgrades, GameObject uiToSpawn, Transform spawnParent)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            int currentIndex = i;

            GameObject go = Instantiate(uiToSpawn, spawnParent);
            
            upgrades[currentIndex].currentUpgradeCost = upgrades[currentIndex].originalUpgradeCost;
            
            // 配置升级按钮的文本内容
            UpgradeButtonReferences buttonRef = go.GetComponent<UpgradeButtonReferences>();
            buttonRef.upgradeButtonText.text = upgrades[currentIndex].upgradeButtonText;
            buttonRef.upgradeDescriptionText.SetText(upgrades[currentIndex].upgradeButtonDescription, upgrades[currentIndex].upgradeAmount);
            buttonRef.upgradeCostText.text = "Cost: " + upgrades[currentIndex].currentUpgradeCost;
            
            // 绑定按钮点击事件
            CookieUpgrade upgrade = upgrades[currentIndex];
            buttonRef.upgradeButton.onClick.AddListener(() =>
            {
                CookieManager.Instance.OnUpgradeButtonClick(upgrade, buttonRef);
            });
        }
    }
}