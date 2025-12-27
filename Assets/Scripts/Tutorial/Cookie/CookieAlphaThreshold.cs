using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CookieAlphaThreshold类用于设置Cookie图像的透明度点击检测阈值
/// 该脚本会将图像的alphaHitTestMinimumThreshold设置为0.001，使得几乎透明的区域也能响应点击事件
/// </summary>
public class CookieAlphaThreshold : MonoBehaviour
{
    private Image _cookieImage;

    /// <summary>
    /// Awake函数在脚本实例被创建时调用，用于初始化组件
    /// </summary>
    private void Awake()
    {
        // 获取当前游戏对象上的Image组件
        _cookieImage = GetComponent<Image>();
        
        // 设置透明度点击检测最小阈值为0.001，使得极低透明度的区域也能响应点击
        _cookieImage.alphaHitTestMinimumThreshold = 0.001f;
    }
}