using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 可交互对象的基类，提供基础的交互功能实现
/// </summary>
public abstract class InteractableBase : MonoBehaviour
{
    [Header("Interactable Settings")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] public GameObject interactableFloatingIcon;

    protected bool CanStillInteract = true;
    protected Transform PlayerTransform;
    private InteractableIcon _icon;

    /// <summary>
    /// 初始化方法，在对象创建时调用
    /// </summary>
    protected virtual void Awake()
    {
        // 获取交互图标组件
        if (interactableFloatingIcon != null)
            _icon = interactableFloatingIcon.GetComponent<InteractableIcon>();
    }

    /// <summary>
    /// 启动方法，在对象开始运行时调用
    /// </summary>
    protected virtual void Start()
    {
        // 查找玩家对象并获取其Transform组件
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            PlayerTransform = player.transform;
    }

    /// <summary>
    /// 更新方法，每帧调用以检测玩家距离和交互输入
    /// </summary>
    protected virtual void Update()
    {
        if (PlayerTransform != null && CanStillInteract)
        {
            float distance = Vector2.Distance(transform.position, PlayerTransform.position);
            if (distance <= interactionDistance)
            {
                ShowInteractionIcon();
                
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Interact();
                }
            }
            else
                HideInteractionIcon();
        }
    }

    /// <summary>
    /// 定义交互行为的抽象方法，子类必须实现具体的交互逻辑
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// 显示交互图标
    /// </summary>
    protected virtual void ShowInteractionIcon()
    {
        if (_icon != null)
        {
            _icon.ShowIcon();
        }
    }

    /// <summary>
    /// 隐藏交互图标
    /// </summary>
    protected virtual void HideInteractionIcon()
    {
        if (_icon != null)
            _icon.HideIcon();
    }
}