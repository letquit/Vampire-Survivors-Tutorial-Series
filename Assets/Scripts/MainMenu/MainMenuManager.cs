using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 主菜单管理器类，负责处理游戏主菜单的逻辑，包括场景加载和进度条显示
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject loadingBarObject;
    [SerializeField] private Image loadingBar;
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Scenes to Load")]
    [SerializeField] private SceneField persistentGameplay;
    [SerializeField] private SceneField levelScene;
    
    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();
    
    /// <summary>
    /// 在对象唤醒时执行初始化操作，隐藏加载进度条对象
    /// </summary>
    private void Awake()
    {
        loadingBarObject.SetActive(false);
    }

    /// <summary>
    /// 开始游戏流程，隐藏菜单界面并异步加载游戏场景
    /// </summary>
    public void StartGame()
    {
        // 隐藏主菜单界面元素
        HideMenu();
        
        // 显示加载进度条
        loadingBarObject.SetActive(true);
        
        // 异步加载持久化游戏场景和关卡场景
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(persistentGameplay));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(levelScene, LoadSceneMode.Additive));

        // 启动进度条更新协程
        StartCoroutine(ProgressLoadingBar());
    }

    /// <summary>
    /// 隐藏所有需要隐藏的菜单对象
    /// </summary>
    private void HideMenu()
    {
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            objectsToHide[i].SetActive(false);
        }
    }

    /// <summary>
    /// 更新加载进度条的协程，根据场景加载进度更新进度条显示
    /// </summary>
    /// <returns>IEnumerator迭代器对象</returns>
    private IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;
                loadingBar.fillAmount = loadProgress / _scenesToLoad.Count;
                yield return null;
            }
        }
    }
}