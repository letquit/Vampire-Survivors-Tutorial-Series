using System;
using UnityEngine;

/// <summary>
/// 可交互图标组件，提供图标显示/隐藏和浮动动画功能
/// </summary>
public class InteractableIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float bobSpeed = 2f;
    
    private Vector3 _startPosition;
    private SpriteRenderer _iconRenderer;
    
    /// <summary>
    /// 初始化组件，记录初始位置并隐藏图标
    /// </summary>
    private void Start()
    {
        _startPosition = transform.position;
        _iconRenderer = GetComponent<SpriteRenderer>();
        
        // 确保图标初始是隐藏的
        if (_iconRenderer != null)
            _iconRenderer.enabled = false;
    }
    
    /// <summary>
    /// 更新图标位置，实现上下浮动动画效果
    /// </summary>
    private void Update()
    {
        // 图标上下浮动效果
        float newY = _startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
    
    /// <summary>
    /// 显示图标
    /// </summary>
    public void ShowIcon()
    {
        if (_iconRenderer != null)
            _iconRenderer.enabled = true;
    }
    
    /// <summary>
    /// 隐藏图标
    /// </summary>
    public void HideIcon()
    {
        if (_iconRenderer != null)
            _iconRenderer.enabled = false;
    }
}