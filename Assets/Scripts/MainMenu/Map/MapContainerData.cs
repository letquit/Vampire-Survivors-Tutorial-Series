using UnityEngine;

/// <summary>
/// 地图容器数据类，用于存储房间场景信息和揭示状态
/// </summary>
public class MapContainerData : MonoBehaviour
{
    /// <summary>
    /// 房间场景字段，用于引用特定的场景资源
    /// </summary>
    public SceneField roomScene;

    /// <summary>
    /// 获取或设置房间是否已被揭示的状态
    /// </summary>
    public bool HasBeenRevealed { get; set; }
}