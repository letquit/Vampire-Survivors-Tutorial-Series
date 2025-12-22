using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// 相机目标切换器。用于在不同相机跟随目标之间进行平滑过渡。
/// </summary>
public class CameraTargetSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject cameraFollow;
    [SerializeField] private GameObject transitionObject;
    [SerializeField] private float duration = 0.75f;
    
    [SerializeField] private new CinemachineCamera camera;
    
    private GameObject _player;

    private bool _goToCameraFollow;
    private bool _goToPlayer;

    private float _timer;
    private Vector3 _startPos;

    /// <summary>
    /// 初始化组件引用和初始状态。
    /// 获取当前场景中标记为"Player"的游戏对象，并将过渡对象初始化到玩家位置。
    /// </summary>
    private void Awake()
    {
        camera = GetComponent<CinemachineCamera>();
        
        _player = GameObject.FindGameObjectWithTag("Player");
        transitionObject.transform.position = _player.transform.position;
    }
    
    /// <summary>
    /// 每帧更新逻辑，处理相机跟随目标的平滑过渡动画。
    /// 根据标志位决定是向玩家还是摄像机跟随对象移动。
    /// </summary>
    private void Update()
    {
        // 如果正在向玩家方向过渡
        if (_goToPlayer)
        {
            MoveTransitionObject(_player.transform.position);

            // 过渡完成时设置相机直接跟随玩家并重置标志
            if (_timer >= duration)
            {
                _goToPlayer = false;
                camera.Follow = _player.transform;
            }
        }
        // 否则如果正在向摄像机跟随对象过渡
        else if (_goToCameraFollow)
        {
            MoveTransitionObject(cameraFollow.transform.position);
            
            // 过渡完成时设置相机直接跟随该对象并重置标志
            if (_timer >= duration)
            {
                _goToCameraFollow = false;
                camera.Follow = cameraFollow.transform;
            }
        }
    }

    /// <summary>
    /// 移动过渡对象的位置以实现相机焦点的平滑过渡效果。
    /// 使用线性插值计算当前位置并在指定时间内从起始点移动到终点。
    /// </summary>
    /// <param name="endPos">过渡的目标位置</param>
    private void MoveTransitionObject(Vector2 endPos)
    {
        _timer += Time.deltaTime;
        Vector2 transitionPosition = Vector2.Lerp(_startPos, endPos, (_timer / duration));
        transitionObject.transform.position = transitionPosition;
    }

    /// <summary>
    /// 切换相机的跟随目标。根据当前控制方案选择不同的目标：
    /// 键盘鼠标模式下切换至cameraFollow对象；
    /// 手柄模式下切换回玩家对象。
    /// </summary>
    /// <param name="input">输入系统传递的PlayerInput实例</param>
    public void SwitchCameraFollow(PlayerInput input)
    {
        if (UserInput.CurrentControlScheme == UserInput.KeyboardAndMouseControlScheme)
        {
            camera.Follow = transitionObject.transform;
            _startPos = camera.transform.position;
            _goToCameraFollow = true;
            _goToPlayer = false;
            _timer = 0;
        }
        else if (UserInput.CurrentControlScheme == UserInput.GamepadControlScheme)
        {
            camera.Follow = transitionObject.transform;
            _startPos = camera.transform.position;
            _goToPlayer = true;
            _goToCameraFollow = false;
            _timer = 0;
        }
    }
}