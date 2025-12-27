using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 管理游戏中的饼干系统，包括饼干计数、每秒饼干生成、升级系统等核心功能
/// </summary>
public class CookieManager : MonoBehaviour
{
    public static CookieManager Instance;

    public GameObject mainGameCanvas;
    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private TextMeshProUGUI cookieCountText;
    [SerializeField] private TextMeshProUGUI cookiePerSecondText;
    [SerializeField] private GameObject cookieObj;
    public GameObject cookieTextPopup;
    [SerializeField] private GameObject backgroundObj;

    [Space] 
    public CookieUpgrade[] cookieUpgrades;
    [SerializeField] private GameObject upgradeUIToSpawn;
    [SerializeField] private Transform upgradeUIParent;
    public GameObject cookiesPerSecondObjToSpawn;

    /// <summary>
    /// 获取或设置当前饼干数量
    /// </summary>
    public double CurrentCookieCount { get; set; }
    
    /// <summary>
    /// 获取或设置当前每秒饼干生成数量
    /// </summary>
    public double CurrentCookiesPerSecond { get; set; }
    
    /// <summary>
    /// 获取或设置每次点击饼干时的升级加成
    /// </summary>
    public double CookiesPerClickUpgrade { get; set; }
    
    private InitializeUpgrades _initializeUpgrades;
    private CookieDisplay _cookieDisplay;

    /// <summary>
    /// 初始化CookieManager单例并设置相关组件和UI
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        _cookieDisplay = GetComponent<CookieDisplay>();

        UpdateCookieUI();
        UpdateCookiesPerSecondUI();

        upgradeCanvas.SetActive(false);
        mainGameCanvas.SetActive(true);
        
        _initializeUpgrades = GetComponent<InitializeUpgrades>();
        _initializeUpgrades.Initialize(cookieUpgrades, upgradeUIToSpawn, upgradeUIParent);
    }

    #region On Cookie Clicked

    /// <summary>
    /// 处理饼干被点击的事件，增加饼干数量并播放动画效果
    /// </summary>
    public void OnCookieClicked()
    {
        IncreaseCookie();

        cookieObj.transform.DOBlendableScaleBy(new Vector3(0.05f, 0.05f, 0.05f), 0.05f).OnComplete(CookieScaleBack);
        backgroundObj.transform.DOBlendableScaleBy(new Vector3(0.05f, 0.05f, 0.05f), 0.05f).OnComplete(BackgroundScaleBack);

        PopupText.Create(1 + CookiesPerClickUpgrade);
    }

    /// <summary>
    /// 重置饼干对象的缩放动画
    /// </summary>
    private void CookieScaleBack()
    {
        cookieObj.transform.DOBlendableScaleBy(new Vector3(-0.05f, -0.05f, -0.05f), 0.05f);
    }

    /// <summary>
    /// 重置背景对象的缩放动画
    /// </summary>
    private void BackgroundScaleBack()
    {
        backgroundObj.transform.DOBlendableScaleBy(new Vector3(-0.05f, -0.05f, -0.05f), 0.05f);
    }

    /// <summary>
    /// 增加当前饼干数量并更新UI显示
    /// </summary>
    private void IncreaseCookie()
    {
        CurrentCookieCount += 1 + CookiesPerClickUpgrade;
        UpdateCookieUI();
    }

    #endregion

    #region UI Updates

    /// <summary>
    /// 更新饼干数量的UI显示
    /// </summary>
    private void UpdateCookieUI()
    {
        // cookieCountText.text = CurrentCookieCount.ToString("F1");
        _cookieDisplay.UpdateCookieText(CurrentCookieCount, cookieCountText);
    }

    /// <summary>
    /// 更新每秒饼干生成数量的UI显示
    /// </summary>
    private void UpdateCookiesPerSecondUI()
    {
        // cookiePerSecondText.text = CurrentCookiesPerSecond.ToString("F1") + " P/S";
        _cookieDisplay.UpdateCookieText(CurrentCookiesPerSecond, cookiePerSecondText, " P/S");
    }

    #endregion

    #region Button Presses

    /// <summary>
    /// 处理升级按钮被点击的事件，切换到升级界面
    /// </summary>
    public void OnUpgradeButtonPressed()
    {
        mainGameCanvas.SetActive(false);
        upgradeCanvas.SetActive(true);
    }
    
    /// <summary>
    /// 处理恢复按钮被点击的事件，返回主游戏界面
    /// </summary>
    public void OnResumeButtonPressed()
    {
        mainGameCanvas.SetActive(true);
        upgradeCanvas.SetActive(false);
    }

    #endregion

    #region Simple Increases

    /// <summary>
    /// 简单增加饼干数量
    /// </summary>
    /// <param name="amount">要增加的饼干数量</param>
    public void SimpleCookieIncrease(double amount)
    {
        CurrentCookieCount += amount;
        UpdateCookieUI();
    }
    
    /// <summary>
    /// 简单增加每秒饼干生成数量
    /// </summary>
    /// <param name="amount">要增加的每秒饼干数量</param>
    public void SimpleCookiesPerSecondIncrease(double amount)
    {
        CurrentCookiesPerSecond += amount;
        UpdateCookiesPerSecondUI();
    }

    #endregion

    #region Events

    /// <summary>
    /// 处理升级按钮被点击的事件，执行升级逻辑
    /// </summary>
    /// <param name="upgrade">要执行的升级对象</param>
    /// <param name="buttonRef">升级按钮的引用组件</param>
    public void OnUpgradeButtonClick(CookieUpgrade upgrade, UpgradeButtonReferences buttonRef)
    {
        if (CurrentCookieCount >= upgrade.currentUpgradeCost)
        {
            upgrade.ApplyUpgrade();
            
            CurrentCookieCount -= upgrade.currentUpgradeCost;
            UpdateCookieUI();

            upgrade.currentUpgradeCost =
                Mathf.Round((float)(upgrade.currentUpgradeCost * (1 + upgrade.costIncreaseMultiplierPerPurchase)));

            buttonRef.upgradeCostText.text = "Cost: " + upgrade.currentUpgradeCost;
        }
    }

    #endregion
}