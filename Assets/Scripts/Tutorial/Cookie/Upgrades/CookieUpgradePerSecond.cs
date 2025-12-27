using UnityEngine;

/// <summary>
/// 饼干每秒生产升级脚本对象
/// 用于处理每秒生产饼干数量的升级功能
/// </summary>
[CreateAssetMenu(menuName = "Cookie Upgrade/Cookies Per Second", fileName = "Cookies Per Second")]
public class CookieUpgradePerSecond : CookieUpgrade
{
    /// <summary>
    /// 应用升级效果
    /// 创建每秒生产饼干的计时器对象并增加饼干生产速度
    /// </summary>
    public override void ApplyUpgrade()
    {
        // 创建每秒生产饼干的对象实例
        GameObject go = Instantiate(CookieManager.Instance.cookiesPerSecondObjToSpawn, Vector3.zero,
            Quaternion.identity);
        go.GetComponent<CookiePerSecondTimer>().CookiePerSecond = upgradeAmount;
        
        // 增加简单饼干每秒生产数量
        CookieManager.Instance.SimpleCookiesPerSecondIncrease(upgradeAmount);
    }
}