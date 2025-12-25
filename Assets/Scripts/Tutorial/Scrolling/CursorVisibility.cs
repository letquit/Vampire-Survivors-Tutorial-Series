using System;
using UnityEngine;

/// <summary>
/// 光标可见性管理器，负责根据用户输入方式和菜单状态控制光标的显示和隐藏
/// </summary>
public class CursorVisibility : MonoBehaviour
{
    /// <summary>
    /// 当组件启用时调用，注册鼠标移动事件监听器
    /// </summary>
    private void OnEnable()
    {
        UserInput.OnMouseMovedAction += ShowCursor;
    }
    
    /// <summary>
    /// 当组件禁用时调用，注销鼠标移动事件监听器
    /// </summary>
    private void OnDisable()
    {
        UserInput.OnMouseMovedAction -= ShowCursor;
    }

    /// <summary>
    /// 每帧更新光标状态，根据菜单是否打开和当前控制方案决定光标的显示或隐藏
    /// </summary>
    private void Update()
    {
        // 当菜单打开时，如果使用手柄，隐藏光标
        if (MenuController.Instance.IsMenuOpen && 
            UserInput.CurrentControlScheme == UserInput.GamepadControlScheme)
        {
            HideCursor();
        }
        else
        {
            // 其他情况下显示光标
            ShowCursor();
        }
    }

    /// <summary>
    /// 隐藏光标并锁定光标状态
    /// </summary>
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// 显示光标并解除光标锁定状态
    /// </summary>
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}