using System;
using UnityEngine;

/// <summary>
/// 相机移动控制类，用于控制相机跟随目标物体移动
/// </summary>
public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    /// <summary>
    /// 每帧更新相机位置，使相机跟随目标物体
    /// </summary>
    private void Update()
    {
        // 将相机位置设置为目标位置加上偏移量
        transform.position = target.position + offset;
    }
}

