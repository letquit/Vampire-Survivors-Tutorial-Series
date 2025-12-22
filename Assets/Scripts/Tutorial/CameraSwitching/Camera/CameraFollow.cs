using System;
using UnityEngine;

/// <summary>
/// 相机跟随脚本类，用于实现相机平滑跟随玩家并朝向鼠标位置移动的功能
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// 跟随灵敏度，控制相机跟随鼠标的快慢程度，取值范围2f到100f，默认值3f
    /// 值越小跟随越紧密，值越大跟随越平缓
    /// </summary>
    [Range(2f, 100f), SerializeField] private float followSensitivity = 3f;

    private Camera _camera;
    private Transform _playerTransform;

    private Rect _screenRect;
    
    private Vector3 _targetPos;

    /// <summary>
    /// 初始化相机跟随组件，在游戏对象启动时执行一次
    /// 获取主相机引用、创建屏幕矩形区域、查找标记为"Player"的游戏对象的变换组件
    /// </summary>
    private void Start()
    {
        _camera = Camera.main;
        _screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// 在检视面板中属性值发生变化时调用，用于验证和初始化相关组件
    /// 重新获取玩家变换组件并设置当前对象位置与玩家位置一致
    /// </summary>
    private void OnValidate()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = _playerTransform.position;
    }

    /// <summary>
    /// 每帧更新相机位置，实现跟随逻辑
    /// 当存在玩家对象且鼠标在屏幕区域内时，计算相机应该移动到的目标位置
    /// </summary>
    private void Update()
    {
        // 检查玩家对象是否存在且鼠标在屏幕范围内
        if (_playerTransform != null && _screenRect.Contains(UserInput.MousePosition))
        {
            // 从鼠标屏幕位置发射射线到场景中
            Ray ray = _camera.ScreenPointToRay(UserInput.MousePosition);

            // 计算射线与Z=0平面的交点作为目标位置
            _targetPos = ray.origin + ray.direction * Math.Abs(_camera.transform.position.z);
            _targetPos.z = 0f;

            // 根据跟随灵敏度计算最终的相机位置，结合目标位置和玩家位置进行加权平均
            Vector3 followObjectPosition =
                (_targetPos + (followSensitivity - 1) * _playerTransform.position) / followSensitivity;
            
            // 更新相机位置
            transform.position = followObjectPosition;
        }
    }
}