using UnityEngine;

/// <summary>
/// 表示每点击增加饼干数量的升级配置
/// </summary>
[CreateAssetMenu(menuName = "Cookie Upgrade/Cookies Per Click", fileName = "Cookies Per Click")]
public class CookieUpgradePerClick : CookieUpgrade
{
    /// <summary>
    /// 应用当前升级效果
    /// </summary>
    /// <remarks>
    /// 此方法会增加CookieManager中每次点击获得的饼干数量
    /// </remarks>
    public override void ApplyUpgrade()
    {
        CookieManager.Instance.CookiesPerClickUpgrade += upgradeAmount;
    }
}