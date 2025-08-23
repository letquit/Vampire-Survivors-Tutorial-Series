using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景控制类，用于管理场景切换功能
/// </summary>
[Obsolete]
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// 切换到指定名称的场景
    /// </summary>
    /// <param name="name">要切换到的场景名称</param>
    public void SceneChange(string name)
    {
        // 加载指定名称的场景
        SceneManager.LoadScene(name);
        // 恢复游戏正常运行速度
        Time.timeScale = 1f;
    }
}

