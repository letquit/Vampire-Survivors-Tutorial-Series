using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 用户输入管理类，负责处理玩家的输入控制和鼠标位置跟踪
/// </summary>
public class UserInput : MonoBehaviour
{
    /// <summary>
    /// 玩家输入组件实例
    /// </summary>
    public static PlayerInput PlayerInput;
    
    /// <summary>
    /// 当前鼠标在屏幕上的位置
    /// </summary>
    public static Vector2 MousePosition;
    
    /// <summary>
    /// 鼠标位置输入动作
    /// </summary>
    private InputAction _mousePositionAction;

    private Vector2 _lastMousePos;

    public delegate void MouseMovedAction();
    public static event MouseMovedAction OnMouseMovedAction;

    /// <summary>
    /// 游戏手柄控制方案名称
    /// </summary>
    public static string GamepadControlScheme = "Gamepad";
    
    /// <summary>
    /// 键盘和鼠标控制方案名称
    /// </summary>
    public static string KeyboardAndMouseControlScheme = "Keyboard&Mouse";
    
    /// <summary>
    /// 当前使用的控制方案
    /// </summary>
    public static string CurrentControlScheme { get; private set; }
    
    /// <summary>
    /// 在对象唤醒时初始化玩家输入组件和鼠标位置动作
    /// </summary>
    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        
        _mousePositionAction = PlayerInput.actions["MousePosition"];
    }
    
    /// <summary>
    /// 每帧更新鼠标位置信息
    /// </summary>
    private void Update()
    { 
        MousePosition = _mousePositionAction.ReadValue<Vector2>();
        
        if (MousePosition != _lastMousePos)
        {
            MouseMovedEvent();
        }
        
        _lastMousePos = MousePosition;
    }

    public void MouseMovedEvent()
    {
        OnMouseMovedAction?.Invoke();
    }

    /// <summary>
    /// 停用玩家控制输入
    /// </summary>
    public static void DeactivatePlayerControls()
    {
        PlayerInput.currentActionMap.Disable();
    }
    
    /// <summary>
    /// 激活玩家控制输入
    /// </summary>
    public static void ActivatePlayerControls()
    {
        PlayerInput.currentActionMap.Enable();
    }

    /// <summary>
    /// 切换控制方案
    /// </summary>
    /// <param name="input">玩家输入组件，用于获取当前控制方案</param>
    public void SwitchControls(PlayerInput input)
    {
        CurrentControlScheme = input.currentControlScheme;
    }
    
    /// <summary>
    /// UI移动输入方向（用于菜单导航）
    /// </summary>
    public static Vector2 MoveInput
    {
        get
        {
            Vector2 input = Vector2.zero;
            
            // 读取键盘 WASD 和方向键输入
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey. isPressed || Keyboard.current. upArrowKey.isPressed)
                    input. y = 1;
                else if (Keyboard. current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    input.y = -1;
                
                if (Keyboard.current. dKey.isPressed || Keyboard.current. rightArrowKey. isPressed)
                    input.x = 1;
                else if (Keyboard.current.aKey.isPressed || Keyboard.current. leftArrowKey. isPressed)
                    input.x = -1;
            }
            
            // 读取游戏手柄左摇杆输入
            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad. current.leftStick. ReadValue();
                if (stick.magnitude > 0.5f)
                {
                    input = stick. normalized;
                }
            }
            
            return input;
        }
    }
}