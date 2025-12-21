using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 地图房间管理器，负责管理和显示游戏中的各个房间
/// </summary>
public class MapRoomManager : MonoBehaviour
{
    public static MapRoomManager Instance;

    private MapContainerData[] _rooms;

    /// <summary>
    /// 初始化时调用，显示当前场景对应的房间
    /// </summary>
    private void Start()
    {
        RevealRoom();
    }
    
    /// <summary>
    /// 在对象启用时执行初始化操作，获取所有子对象中的MapContainerData组件
    /// </summary>
    private void Awake()
    {
        // 实现单例模式，确保只有一个实例存在
        if (Instance == null)
        {
            Instance = this;
        }
        
        // 获取所有子对象中的MapContainerData组件，包括未激活的对象
        _rooms = GetComponentsInChildren<MapContainerData>(true);
    }

    /// <summary>
    /// 当组件启用时注册场景加载事件监听器
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 当组件禁用时注销场景加载事件监听器
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载完成时的回调函数，用于显示新加载场景对应的房间
    /// </summary>
    /// <param name="scene">已加载的场景信息</param>
    /// <param name="mode">场景加载模式</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RevealRoom(scene.name);
    }

    /// <summary>
    /// 显示指定场景名称对应的房间
    /// </summary>
    /// <param name="sceneName">要显示的场景名称，如果为空则使用当前激活场景的名称</param>
    public void RevealRoom(string sceneName = null)
    {
        string targetScene = sceneName ?? SceneManager.GetActiveScene().name;

        // 遍历所有房间，找到对应场景且未被显示的房间进行显示
        for (int i = 0; i < _rooms.Length; i++)
        {
            if (_rooms[i].roomScene.SceneName == targetScene && !_rooms[i].HasBeenRevealed)
            {
                _rooms[i].gameObject.SetActive(true);
                _rooms[i].HasBeenRevealed = true;
                
                return;
            }
        }
    }
}