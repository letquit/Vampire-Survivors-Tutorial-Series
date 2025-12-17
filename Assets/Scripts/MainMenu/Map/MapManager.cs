using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 地图管理器类，负责控制小游戏中的地图显示功能，包括小地图和大地图的切换
/// </summary>
public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject largeMap;
    
    public bool IsLargeMapOpen { get; private set; }

    /// <summary>
    /// 初始化地图管理器实例，在场景加载时执行一次
    /// 设置单例模式实例，并初始化关闭大地图状态
    /// </summary>
    private void Awake()
    {
        // 实现单例模式，确保只有一个地图管理器实例存在
        if (Instance == null)
        {
            Instance = this;
        }
        
        // 初始化时关闭大地图显示
        CloseLargeMap();
    }

    /// <summary>
    /// 每帧检查用户输入，处理地图显示切换逻辑
    /// 当按下M键时，在小地图和大地图之间进行切换
    /// </summary>
    private void Update()
    {
        // 检测M键是否被按下
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            // 根据当前大地图状态进行切换
            if (!IsLargeMapOpen)
            {
                OpenLargeMap();
            }
            else
            {
                CloseLargeMap();
            }
        }
    }

    /// <summary>
    /// 打开大地图显示，同时隐藏小地图
    /// 设置大地图打开状态标志为true
    /// </summary>
    private void OpenLargeMap()
    {
        miniMap.SetActive(false);
        largeMap.SetActive(true);
        IsLargeMapOpen = true;
    }
    
    /// <summary>
    /// 关闭大地图显示，同时显示小地图
    /// 设置大地图打开状态标志为false
    /// </summary>
    private void CloseLargeMap()
    {
        miniMap.SetActive(true);
        largeMap.SetActive(false);
        IsLargeMapOpen = false;
    }
}