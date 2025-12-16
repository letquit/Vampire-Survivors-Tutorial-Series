using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// 场景加载触发器类，用于在玩家进入触发区域时加载和卸载指定场景
/// </summary>
public class SceneLoadTrigger : MonoBehaviour
{
    [FormerlySerializedAs("sceneToLoad")] [SerializeField] private SceneField[] scenesToLoad;
    [FormerlySerializedAs("sceneToUnload")] [SerializeField] private SceneField[] scenesToUnload;
    
    private GameObject _player;

    /// <summary>
    /// 在对象启用时调用，获取标签为"Player"的游戏对象引用
    /// </summary>
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// 当2D碰撞体进入触发器时调用，如果碰撞的对象是玩家，则执行场景加载和卸载操作
    /// </summary>
    /// <param name="other">进入触发器的碰撞体对象</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            LoadScenes();
            UnloadScenes();
        }
    }

    /// <summary>
    /// 加载指定的场景列表，避免重复加载已存在的场景
    /// </summary>
    private void LoadScenes()
    {
        // 遍历所有需要加载的场景
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            // 检查场景是否已经加载
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            // 如果场景未加载，则异步加载场景
            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
            }
        }
    }
    
    /// <summary>
    /// 卸载指定的场景列表
    /// </summary>
    private void UnloadScenes()
    {
        // 遍历所有需要卸载的场景
        for (int i = 0; i < scenesToUnload.Length; i++)
        {
            // 查找当前已加载的场景中是否存在需要卸载的场景
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToUnload[i].SceneName)
                {
                    SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                    break;
                }
            }
        }
    }
}